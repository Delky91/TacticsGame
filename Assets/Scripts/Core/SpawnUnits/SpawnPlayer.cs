using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<Vector3> spawnPositions = new();

    private void Start()
    {
        if (!ValidateSpawnPositions()) return;
        SpawnPlayerAtRandomPosition();
    }

    private bool ValidateSpawnPositions()
    {
        return spawnPositions != null && spawnPositions.Count is > 0 and <= 3;
    }

    private void SpawnPlayerAtRandomPosition()
    {
        var randomIndex = Random.Range(0, spawnPositions.Count);
        var spawnPosition = spawnPositions[randomIndex];
        Instantiate(player, spawnPosition, Quaternion.identity);
    }
}