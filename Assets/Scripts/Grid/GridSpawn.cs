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

    private void Start()
    {
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
            var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);


            // Assign a material from the array to the tile in a checkbox pattern. 
            if (!tile.TryGetComponent<Renderer>(out var tileRenderer)) return;

            var materialIndex = (x + z) % tileMaterials.Length;
            tileRenderer.material = tileMaterials[materialIndex];
        }
    }
}