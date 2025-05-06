using UnityEngine;

/// <summary>
///     Represents a single tile in the game grid system, managing unit placement and position information.
///     This component handles the occupation state and provides methods to manipulate units on the tile.
/// </summary>
public class Tile : MonoBehaviour
{
    /// <summary>
    ///     The prefab used to create the visual representation of the tile.
    /// </summary>
    [SerializeField] private GameObject tilePrefab;

    /// <summary>
    ///     Reference to the unit currently occupying this tile.
    /// </summary>
    private Unit _occupyingUnit;

    /// <summary>
    ///     Indicates whether the tile is currently occupied by a unit.
    /// </summary>
    public bool IsOccupied => _occupyingUnit != null;

    private void Awake()
    {
        if (tilePrefab == null) return;
    }

    /// <summary>
    ///     Gets the world position of the specified tile.
    /// </summary>
    /// <param name="tile">The tile whose position to retrieve.</param>
    /// <returns>The world position of the tile, or Vector3.zero if the tile is null.</returns>
    public Vector3 GetTileWorldPosition(Tile tile)
    {
        return tile == null ? Vector3.zero : tile.transform.position;
    }

    /// <summary>
    ///     Sets the unit occupying this tile.
    /// </summary>
    /// <param name="unit">The unit to place on this tile.</param>
    public void SetUnit(Unit unit)
    {
        _occupyingUnit = unit;
    }

    /// <summary>
    ///     Gets the unit currently occupying this tile.
    /// </summary>
    /// <returns>The occupying unit, or null if the tile is empty.</returns>
    public Unit GetUnit()
    {
        return _occupyingUnit;
    }

    /// <summary>
    ///     Removes the current unit from this tile.
    /// </summary>
    public void ClearUnit()
    {
        _occupyingUnit = null;
    }
}