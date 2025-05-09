using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*
     * Pasos del sistema de movimiento del player:
     * 1) saber cuantos PM tiene el player por default (crear scriptable Object con estadísticas). OK
     * 2) crear variable para mantener los PM que tiene actualmente. (por buff y debuff). OK
     * 3) Pm máximos actuales para mantener después del final de turno. OK
     * 4) método para gastar PM. OK
     * 5) método para que al finalizar el turno los pm vuelvan a su valor maximo actual.
     * 6) Eventos para el clic tanto en selección como para mover a un tile.
     * 7) Generar una utilities para el highlight de los tiles a los cuales es permitido moverse.
     */
    // TODO usar systema de object pool para el highlight en vez de cambiar el material, lo mismo con el hover del tile

    // get the base stats.
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private Vector3 offsetY = new(0, 0.4f, 0);

// Velocidad de movimiento entre tiles
    [SerializeField] private float movementSpeed = 5f;

    private readonly List<Tile> _highlightedTiles = new();
    private int _currentMovementPoints;
    private GridSpawn _gridSpawn;

// Indicador de si la unidad está en movimiento
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
        if (_unit.CurrentTile != null)
        {
            _previousTile = _unit.CurrentTile;
            _previousTile.SetOccupied(true);
            _previousTile.SetUnit(_unit);
        }
    }

    private void OnDestroy()
    {
        if (!TurnSystem.Instance) return;
        TurnSystem.Instance.OnPlayerTurnStarted -= OnTurnStart;
        Unit.OnUnitSelected -= HandleUnitSelected;
    }


    private void HandleUnitSelected(Unit selectedUnit)
    {
        // Sí estamos seleccionando otra unidad, deseleccionamos la actual.
        if (_isSelected && selectedUnit != _unit)
        {
            ClearHighlightedTiles();
            _isSelected = false;
            return;
        }

        // Sí estamos seleccionando esta unidad
        if (selectedUnit != _unit) return;
        _isSelected = true;
        HighlightAvailableTiles();
    }


    // Método para resaltar las casillas disponibles para moverse
    private void HighlightAvailableTiles()
    {
        ClearHighlightedTiles();

        if (_unit.CurrentTile == null) return;

        // Usar PathFinder para encontrar todos los tiles accesibles
        _pathsToTiles = PathFinder.FindAllAccessibleTiles(_unit.CurrentTile, _currentMovementPoints, _gridSpawn);

        // Resaltar todos los tiles accesibles
        foreach (var tile in _pathsToTiles.Keys)
        {
            tile.HighlightAsMoveTarget(true);
            _highlightedTiles.Add(tile);
        }
    }


// Método auxiliar para obtener casillas vecinas en las cuatro direcciones ortogonales
    private List<Tile> GetNeighborTiles(Tile tile)
    {
        var position = tile.transform.position;
        var x = Mathf.RoundToInt(position.x);
        var z = Mathf.RoundToInt(position.z);

        // Direcciones: derecha, izquierda, arriba, abajo
        Vector2Int[] directions =
        {
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(0, -1)
        };

        return directions.Select(dir => new Vector2Int(x + dir.x, z + dir.y))
            .Select(neighborPos => _gridSpawn.GetTileAt(neighborPos)).Where(neighborTile => neighborTile is not null)
            .ToList();
    }


    private void ClearHighlightedTiles()
    {
        foreach (var tile in _highlightedTiles) tile.HighlightAsMoveTarget(false);
        _highlightedTiles.Clear();
    }


    public void MoveToTile(Tile targetTile)
    {
        if (!TurnSystem.Instance.IsPlayerTurn) return;

        // Check if the tile is valid
        if (!_highlightedTiles.Contains(targetTile)) return;
        if (_isMoving) return;

        // Obtener el camino al tile objetivo
        if (!_pathsToTiles.TryGetValue(targetTile, out var path) || path.Count <= 1)
            return;

        // Calcular el costo total del movimiento
        var totalCost = 0;
        for (var i = 1; i < path.Count; i++)
        {
            var moveCost = PathFinder.IsDiagonalMove(path[i - 1], path[i]) ? 2 : 1;
            if (path[i].MovementCost > 0)
                moveCost = path[i].MovementCost;
            totalCost += moveCost;
        }

        // Verificar si tenemos suficientes PM
        if (totalCost > _currentMovementPoints)
            return;

        // Iniciar la corrutina de movimiento
        StartCoroutine(MoveAlongPath(path));

        // Gastar PM
        _currentMovementPoints -= totalCost;

        // Limpiar el resaltado pero NO deseleccionar la unidad
        ClearHighlightedTiles();
    }

    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        _isMoving = true;

        // clear the starting tile
        if (_previousTile != null)
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
        var finalTile = path[path.Count - 1];
        _unit.CurrentTile = finalTile;
        finalTile.SetUnit(_unit);
        finalTile.SetOccupied(true);
        _previousTile = finalTile;

        // update the boolean, here I can change animation for the walk.
        _isMoving = false;

        // highlight the tiles if we continue to have PM
        if (_currentMovementPoints > 0) HighlightAvailableTiles();
    }


    public void UsePm(int pointsUsed)
    {
        if (pointsUsed < 0) return; // pointUsed only can be a positive number
        if (_currentMovementPoints < pointsUsed) return; // if the player doesn't have enough pm
        if (_currentMovementPoints < 0) _currentMovementPoints = 0; // no negatives pm
        _currentMovementPoints -= pointsUsed;
    }


    private void OnTurnStart()
    {
        _currentMovementPoints = _maxMovementPoints;
        if (_isSelected) HighlightAvailableTiles();
    }

    #region A* calculate distance

    // Clase para almacenar información del nodo para A*
    private class PathNode
    {
        public readonly int GCost; // Costo desde el inicio
        public readonly int HCost; // Costo estimado hasta el destino (heurística)
        public PathNode Parent; // Para reconstruir el camino
        public Tile Tile;

        public PathNode(Tile tile, int gCost, int hCost, PathNode parent)
        {
            Tile = tile;
            GCost = gCost;
            HCost = hCost;
            Parent = parent;
        }

        public int FCost => GCost + HCost; // Costo total
    }

    #endregion

    #region Event (TODO: pass this to a utilities file)

    public void GainPm(int pm)
    {
        _currentMovementPoints += pm;
    }

    public int GetCurrentPm()
    {
        return _currentMovementPoints;
    }

    public int GetMaxPm()
    {
        return _maxMovementPoints;
    }

    /// <summary>
    ///     Only for changes in max Movement Points, like buff or debuffs.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="isAdd"></param>
    /// <returns></returns>
    public int ChangeMaxPm(int points, bool isAdd = true)
    {
        if (isAdd) _maxMovementPoints += points;
        else _maxMovementPoints -= points;

        return _maxMovementPoints;
    }

    #endregion
}