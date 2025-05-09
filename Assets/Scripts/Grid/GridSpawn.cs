using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSpawn : MonoBehaviour
{
    // Size of the grid.
    private const int Width = 10;
    private const int Height = 10;

    // Prefab (tile) to spawn.
    [SerializeField] private GameObject tilePrefab;

    // Gap between tiles.
    [SerializeField] private float tileSpacing = 1.0f;

    // Materials for the tiles (checkbox pattern).
    [Header("Tile Materials")] [SerializeField]
    private Material[] tileMaterials;

    private Tile[,] _tiles; // references to all tiles


    // TODO to implement special tiles to spawn before combat start.

    // Special tiles for player and enemy
    // [SerializeField] private Material playerSpawnMaterial;
    // [SerializeField] private Material enemySpawnMaterial;


    private void Awake()
    {
        _tiles = new Tile[Width, Height];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (var x = 0; x < Width; x++)
        for (var z = 0; z < Height; z++)
        {
            // calculates the position of each tile based on its index and the spacing.
            Vector3 position = new(x * tileSpacing, 0, z * tileSpacing);

            // Instantiate the tile prefab at the calculated position.
            var go = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            var tile = go.GetComponent<Tile>();

            // Assign a material from the array to the tile in a checkbox pattern. 
            if (!go.TryGetComponent<Renderer>(out var tileRenderer)) return;

            var materialIndex = (x + z) % tileMaterials.Length;
            tileRenderer.material = tileMaterials[materialIndex];

            _tiles[x, z] = tile;
        }

        // Después de crear el grid, crear obstáculos y terrenos variados
        // CreateRandomObstacles();
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= Width || pos.y >= Height) return null;
        return _tiles[pos.x, pos.y];
    }

    public List<Tile> GetAllTiles()
    {
        return _tiles.Cast<Tile>().ToList();
    }

    #region Obstacle and Random Objects

    //
    // private void CreateRandomObstacles()
    // {
    //     // Asegúrate de que esto se ejecute después de que se crea el grid
    //     if (_tiles == null || _tiles.Length == 0) return;
    //
    //
    //     // Por ejemplo, convertir el 10% de las casillas en obstáculos
    //     var obstacleCount = Mathf.FloorToInt(_tiles.Length * 0.1f);
    //
    //     for (var i = 0; i < obstacleCount; i++)
    //     {
    //         // Elegir una casilla aleatoria
    //         var randomX = Random.Range(0, Width);
    //         var randomZ = Random.Range(0, Height);
    //         var randomTile = _tiles[randomX, randomZ];
    //
    //         // No convertir en obstáculo si ya está ocupada o ya es un obstáculo
    //         if (randomTile.IsOccupied || randomTile.IsObstacle)
    //         {
    //             // Intentar de nuevo
    //             i--;
    //             continue;
    //         }
    //
    //         // Configurar como obstáculo
    //         randomTile.SetAsObstacle(true);
    //     }
    //
    //     // create varied terrain
    //     // CreateVariedTerrain();
    // }
    //
    // private void CreateVariedTerrain()
    // {
    //     // Ejemplo: crear algunas casillas con mayor costo de movimiento (como "terreno difícil")
    //     var variedTerrainCount = Mathf.FloorToInt(_tiles.Length * 0.2f);
    //
    //     for (var i = 0; i < variedTerrainCount; i++)
    //     {
    //         // Elegir una casilla aleatoria
    //         var randomX = Random.Range(0, Width);
    //         var randomZ = Random.Range(0, Height);
    //         var randomTile = _tiles[randomX, randomZ];
    //
    //         // No modificar obstáculos o casillas ocupadas
    //         if (randomTile.IsOccupied || randomTile.IsObstacle)
    //         {
    //             i--;
    //             continue;
    //         }
    //
    //         // Asignar un costo mayor (2 o 3 PM para atravesar)
    //         var difficultTerrainCost = Random.Range(2, 4);
    //         randomTile.SetMovementCost(difficultTerrainCost);
    //
    //         // Opcional: cambiar visualmente para indicar terreno diferente
    //         SetTerrainVisualByMovementCost(randomTile, difficultTerrainCost);
    //     }
    // }
    //
    // private void SetTerrainVisualByMovementCost(Tile tile, int cost)
    // {
    //     // Cambiar color según costo (solo un ejemplo)
    //     var goRrenderer = tile.GetComponent<Renderer>();
    //     if (goRrenderer != null)
    //         switch (cost)
    //         {
    //             case 2:
    //                 // Terreno moderado (por ejemplo, arena) - color amarillento
    //                 goRrenderer.material.color = new Color(0.9f, 0.9f, 0.5f);
    //                 break;
    //             case 3:
    //                 // Terreno difícil (por ejemplo, pantano) - color verdoso
    //                 goRrenderer.material.color = new Color(0.5f, 0.7f, 0.3f);
    //                 break;
    //         }
    // }
    //

    #endregion
}