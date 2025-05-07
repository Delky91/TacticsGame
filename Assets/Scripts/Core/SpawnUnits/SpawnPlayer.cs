using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;

    [Tooltip("Coordinates where the player can Spawn")]
    public List<Vector2Int> spawnPositions = new();

    private readonly List<Unit> _units = new();

    private GridSpawn _grid;

    private void Start()
    {
        _grid = FindFirstObjectByType<GridSpawn>();
        if (_grid == null) return;

        foreach (var pos in spawnPositions) SpawnPlayerAt(pos);
    }

    private void SpawnPlayerAt(Vector2Int pos)
    {
        var tile = _grid.GetTileAt(pos);
        if (tile == null || tile.OccupyingUnit != null) return;

        var spawnWorld = tile.transform.position + Vector3.up * 0.4f;
        var unitObject = Instantiate(unitPrefab, spawnWorld, Quaternion.identity);
        var unit = unitObject.GetComponent<Unit>();

        unit.CurrentTile = tile;
        tile.SetUnit(unit);
        _units.Add(unit);
    }
}