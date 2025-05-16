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
}