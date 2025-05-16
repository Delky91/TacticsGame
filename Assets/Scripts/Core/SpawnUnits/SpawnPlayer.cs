using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : BaseSpawn
{
    [Tooltip("Coordinates where the player can Spawn")] [SerializeField]
    private List<Vector2Int> spawnPositions = new();

    private void Start()
    {
        Initialize();
        foreach (var pos in spawnPositions) SpawnUnitAt(pos, Factions.Player);
    }
}