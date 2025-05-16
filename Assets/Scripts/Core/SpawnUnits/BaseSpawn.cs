using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Base class for unit spawning systems in the game.
///     Provides common functionality for spawning units on a grid-based system.
/// </summary>
public abstract class BaseSpawn : MonoBehaviour
{
    /// <summary>
    ///     The prefab used to instantiate new units.
    /// </summary>
    [SerializeField] protected GameObject unitPrefab;

    /// <summary>
    ///     Reference to the grid system where units will be spawned.
    /// </summary>
    protected GridSpawn Grid;

    /// <summary>
    ///     List of all units spawned by this spawner.
    /// </summary>
    protected List<Unit> Units = new();

    /// <summary>
    ///     Vertical offset applied to spawned units to prevent z-fighting.
    /// </summary>
    protected float YOffset = 0.4f;

    /// <summary>
    ///     Initializes the spawner by finding and storing a reference to the GridSpawn system.
    /// </summary>
    protected virtual void Initialize()
    {
        Grid = FindFirstObjectByType<GridSpawn>();
    }

    /// <summary>
    ///     Checks if a unit can be spawned at the specified position.
    /// </summary>
    /// <param name="position">The grid position to check.</param>
    /// <returns>True if the position is valid and unoccupied, false otherwise.</returns>
    protected virtual bool CanSpawnAt(Vector2Int position)
    {
        var tile = Grid.GetTileAt(position);
        return tile && !tile.OccupyingUnit;
    }

    /// <summary>
    ///     Spawns a unit at the specified position with the given faction.
    /// </summary>
    /// <param name="spawnPosition">The grid position where the unit should be spawned.</param>
    /// <param name="faction">The faction to assign to the spawned unit.</param>
    /// <returns>The spawned Unit component if successful, null otherwise.</returns>
    protected virtual Unit SpawnUnitAt(Vector2Int spawnPosition, Factions faction)
    {
        var tile = Grid.GetTileAt(spawnPosition);
        if (!CanSpawnAt(spawnPosition)) return null;

        var spawnWorld = tile.transform.position + Vector3.up * YOffset;
        var unitObject = Instantiate(unitPrefab, spawnWorld, Quaternion.identity);
        var unit = unitObject.GetComponent<Unit>();

        if (unit)
        {
            unit.AssignFaction(faction);
            unit.CurrentTile = tile;
            tile.SetUnit(unit);
            Units.Add(unit);
        }

        return unit;
    }
}