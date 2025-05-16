using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // get the base stats.
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private Vector3 offsetY = new(0, 0.4f, 0);

    // Velocidad de movimiento entre tiles
    [SerializeField] private float movementSpeed = 5f;

    private readonly List<Tile> _highlightedTiles = new();
    private int _currentMovementPoints;
    private GridSpawn _gridSpawn;

    // Indicador de sí la unidad está en movimiento
    private bool _isMoving;
    private bool _isSelected;
    private int _maxMovementPoints;

    // Diccionario para almacenar los caminos a cada tile
    private Dictionary<Tile, List<Tile>> _pathsToTiles = new();
    private Tile _previousTile;
    private Unit _unit;


    private void Start()
    {
        // check for Unit
        _unit = GetComponent<Unit>();
        if (!_unit) return;

        // check for GridSpawn
        _gridSpawn = FindFirstObjectByType<GridSpawn>();
        if (!_gridSpawn) return;

        // check for TurnSystem
        if (!TurnSystem.Instance) return;
        TurnSystem.Instance.OnPlayerTurnStarted += OnTurnStart;
        Unit.OnUnitSelected += HandleUnitSelected;

        // assign pm at the start.
        _maxMovementPoints = baseStats.movementPoints;
        _currentMovementPoints = _maxMovementPoints;

        // if the unit it's in the grid initialized as _previousTile
        if (!_unit.CurrentTile) return;

        _previousTile = _unit.CurrentTile;
        _previousTile.SetOccupied(true);
        _previousTile.SetUnit(_unit);
    }

    private void OnDestroy()
    {
        if (!TurnSystem.Instance) return;
        TurnSystem.Instance.OnPlayerTurnStarted -= OnTurnStart;
        Unit.OnUnitSelected -= HandleUnitSelected;
    }


    private void HandleUnitSelected(Unit selectedUnit)
    {
        // deselect the current unit if we are selecting another.
        if (_isSelected && selectedUnit != _unit)
        {
            ClearHighlightedTiles();
            _isSelected = false;
            return;
        }

        if (selectedUnit != _unit) return;
        _isSelected = true;
        HighlightAvailableTiles();
    }


    /// <summary>
    ///     Highlight the available tiles
    /// </summary>
    private void HighlightAvailableTiles()
    {
        ClearHighlightedTiles();

        if (!_unit.CurrentTile) return;

        // Use the PathFinder
        _pathsToTiles = PathFinder.FindAllAccessibleTiles(_unit.CurrentTile, _currentMovementPoints, _gridSpawn);

        // Highlight tiles
        foreach (var tile in _pathsToTiles.Keys)
        {
            tile.HighlightAsMoveTarget(true);
            _highlightedTiles.Add(tile);
        }
    }

    /// <summary>
    ///     Clear the highlight from the tiles.
    /// </summary>
    private void ClearHighlightedTiles()
    {
        foreach (var tile in _highlightedTiles) tile.HighlightAsMoveTarget(false);
        _highlightedTiles.Clear();
    }


    /// <summary>
    ///     Mote the player to the target Tile
    /// </summary>
    /// <param name="targetTile"></param>
    public void MoveToTile(Tile targetTile)
    {
        if (!TurnSystem.Instance.IsPlayerTurn) return;

        // Check if the tile is valid
        if (!_highlightedTiles.Contains(targetTile)) return;
        if (_isMoving) return;

        // Get the path.
        if (!_pathsToTiles.TryGetValue(targetTile, out var path) || path.Count <= 1)
            return;

        // Get the cost of the movement.
        var totalCost = 0;
        for (var i = 1; i < path.Count; i++)
        {
            var moveCost = PathFinder.IsDiagonalMove(path[i - 1], path[i]) ? 2 : 1;
            if (path[i].MovementCost > 0)
                moveCost = path[i].MovementCost;
            totalCost += moveCost;
        }

        // Check if enough PM
        if (totalCost > _currentMovementPoints)
            return;

        // Start the Move Coroutine
        StartCoroutine(MoveAlongPath(path));

        // Use the PM
        UsePm(totalCost);

        // Clear the highlight tiles.
        ClearHighlightedTiles();
    }

    /// <summary>
    ///     Allow the movement from and handle the states.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        _isMoving = true;

        // clear the starting tile
        if (_previousTile)
        {
            _previousTile.SetOccupied(false);
            _previousTile.SetUnit(null);
        }

        // move along the tiles
        for (var i = 1; i < path.Count; i++)
        {
            var targetTile = path[i];
            var startPosition = transform.position;
            var targetPosition = targetTile.transform.position + offsetY;

            // rotation for models
            var direction = targetPosition - startPosition;
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            }

            var journeyLength = Vector3.Distance(startPosition, targetPosition);
            var startTime = Time.time;

            // move to the position gradually
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                var distCovered = (Time.time - startTime) * movementSpeed;
                var fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
                yield return null;
            }

            // update the position
            transform.position = targetPosition;

            // update the current tile
            _unit.CurrentTile = targetTile;
        }

        // Make the target tile as Occupied
        var finalTile = path[^1];
        _unit.CurrentTile = finalTile;
        finalTile.SetUnit(_unit);
        finalTile.SetOccupied(true);
        _previousTile = finalTile;

        // update the boolean, here I can change animation for the walk.
        _isMoving = false;

        // highlight the tiles if we continue to have PM
        if (_currentMovementPoints > 0) HighlightAvailableTiles();
    }


    /// <summary>
    ///     Allow the use of the Unit's PM.
    /// </summary>
    /// <param name="pointsUsed"></param>
    public void UsePm(int pointsUsed)
    {
        if (pointsUsed < 0) return; // pointUsed only can be a positive number
        if (_currentMovementPoints < pointsUsed) return; // if the player doesn't have enough pm
        if (_currentMovementPoints < 0) _currentMovementPoints = 0; // no negatives pm
        _currentMovementPoints -= pointsUsed;
    }


    /// <summary>
    ///     Handle the restart of the PM at the beginning of the player turn.
    /// </summary>
    private void OnTurnStart()
    {
        _currentMovementPoints = _maxMovementPoints;
        if (_isSelected) HighlightAvailableTiles();
    }

    /// <summary>
    ///     Call when the player Unit Gains PM to use ONLY this turn.
    /// </summary>
    /// <param name="pm"></param>
    public void GainPm(int pm)
    {
        _currentMovementPoints += pm;
    }

    /// <summary>
    ///     Get the Current PM of the Unit.
    /// </summary>
    /// <returns>Int with the current PM of the Unit.</returns>
    public int GetCurrentPm()
    {
        return _currentMovementPoints;
    }

    /// <summary>
    ///     Get the max Pm that the unit have.
    /// </summary>
    /// <returns>Int with the max PM.</returns>
    public int GetMaxPm()
    {
        return _maxMovementPoints;
    }

    /// <summary>
    ///     Only for changes in max Movement Points, like buff or debuffs.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="isAdd"></param>
    /// <returns>Int with the next turn max PM</returns>
    public int ChangeMaxPm(int points, bool isAdd = true)
    {
        if (isAdd) _maxMovementPoints += points;
        else _maxMovementPoints -= points;

        return _maxMovementPoints;
    }
}