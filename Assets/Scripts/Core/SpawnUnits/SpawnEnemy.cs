using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : BaseSpawn
{
    [Tooltip("Coordinates where the Enemies can Spawn")] [SerializeField]
    private List<Vector2Int> enemySpawnPoints = new();

    private void Start()
    {
        Initialize();
        foreach (var pos in enemySpawnPoints) SpawnUnitAt(pos, Factions.Enemy);
    }
}