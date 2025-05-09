using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static List<Tile> FindPath(Tile startTile, Tile targetTile, int maxMovementPoints, GridSpawn gridSpawn)
    {
        List<PathNode> openSet = new();
        HashSet<Tile> closedSet = new();

        PathNode startNode = new(startTile, 0, CalculateDistance(startTile, targetTile), null);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // looking for the node with less Final Cost
            var currentNode = openSet[0];
            for (var i = 1; i < openSet.Count; i++)
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    currentNode = openSet[i];
            // remove the actual node from the openSet and add it to the closedSet
            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Tile);

            // check if the destiny was reach and rebuild the path.
            if (currentNode.Tile == targetTile) return ReconstructPath(currentNode);

            // check the neighbors
            var neighbours = GetNeighborTiles(currentNode.Tile, gridSpawn);
            foreach (var neighbour in neighbours)
            {
                // ignore the neighbors invalid or processed
                if (neighbour.IsObstacle || closedSet.Contains(neighbour)) continue;

                // Calculate PM Cost (1 for ortogonal moves 2 for diagonal moves)
                var moveCost = IsDiagonalMove(currentNode.Tile, neighbour) ? 2 : 1;
                if (neighbour.MovementCost > 0) moveCost = neighbour.MovementCost;
                var newGCost = currentNode.GCost + moveCost;

                // check if the path exceeds the Pms available
                if (newGCost > maxMovementPoints) continue;

                // Check if the neighbor is already in openSet
                var existingNode = openSet.Find(n => n.Tile == neighbour);

                if (existingNode == null || newGCost < existingNode.GCost)
                {
                    var hCost = CalculateDistance(neighbour, targetTile);
                    var neighborNode = new PathNode(neighbour, newGCost, hCost, currentNode);

                    if (existingNode == null)
                    {
                        openSet.Add(neighborNode);
                    }
                    else
                    {
                        existingNode.GCost = newGCost;
                        existingNode.Parent = currentNode;
                    }
                }
            }
        }

        // can't find a path
        return null;
    }

    /// <summary>
    ///     Calculate all tiles accessible from an origin with a fix number of movement points
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="movementPoints"></param>
    /// <param name="gridSpawn"></param>
    /// <returns></returns>
    public static Dictionary<Tile, List<Tile>> FindAllAccessibleTiles(Tile startTile, int movementPoints,
        GridSpawn gridSpawn)
    {
        Dictionary<Tile, List<Tile>> accessibleTilesWithPaths = new();
        var allTiles = gridSpawn.GetAllTiles();

        foreach (var tile in allTiles)
        {
            if (tile.IsObstacle) continue;
            if (tile.IsOccupied && tile != startTile) continue;

            var path = FindPath(startTile, tile, movementPoints, gridSpawn);
            if (path != null && path.Count > 0) accessibleTilesWithPaths[tile] = path;
        }

        return accessibleTilesWithPaths;
    }

    /// <summary>
    ///     Calculate Manhattan distance between two tiles (heuristic for A*)
    /// </summary>
    /// <param name="tileA"></param>
    /// <param name="tileB"></param>
    private static int CalculateDistance(Tile tileA, Tile tileB)
    {
        var posA = tileA.transform.position;
        var posB = tileB.transform.position;

        var xDist = Mathf.RoundToInt(Mathf.Abs(posA.x - posB.x));
        var zDist = Mathf.RoundToInt(Mathf.Abs(posA.z - posB.z));

        return xDist + zDist;
    }

    /// <summary>
    ///     Check if the movement is diagonal
    /// </summary>
    public static bool IsDiagonalMove(Tile from, Tile to)
    {
        var fromPos = from.transform.position;
        var toPos = to.transform.position;

        var xDiff = Mathf.RoundToInt(Mathf.Abs(fromPos.x - toPos.x));
        var zDiff = Mathf.RoundToInt(Mathf.Abs(fromPos.z - toPos.z));

        return xDiff == 1 && zDiff == 1;
    }

    /// <summary>
    ///     Reconstruct the path from the end node
    /// </summary>
    /// <param name="endNode"></param>
    /// <returns>The path in a List of Tile</returns>
    private static List<Tile> ReconstructPath(PathNode endNode)
    {
        List<Tile> path = new();
        var currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Tile);
            currentNode = currentNode.Parent;
        }

        // invert the path
        path.Reverse();
        return path;
    }

    private static List<Tile> GetNeighborTiles(Tile tile, GridSpawn gridSpawn)
    {
        var position = tile.transform.position;
        var x = Mathf.RoundToInt(position.x);
        var z = Mathf.RoundToInt(position.z);

        List<Tile> neighbors = new();

        // Directions orthogonal
        Vector2Int[] directions =
        {
            // orthogonal
            new(1, 0), // derecha
            new(-1, 0), // izquierda
            new(0, 1), // arriba
            new(0, -1) // abajo

            // diagonal to my future self
            // new(1, 1), // arriba-derecha
            // new(-1, 1), // arriba-izquierda
            // new(1, -1), // abajo-derecha
            // new(-1, -1) // abajo-izquierda
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = new(x + dir.x, z + dir.y);
            var neighborTile = gridSpawn.GetTileAt(neighborPos);

            if (neighborTile) neighbors.Add(neighborTile);
        }

        // Linq of the foreach
        // return directions.Select(dir => new Vector2Int(x + dir.x, z + dir.y)).Select(gridSpawn.GetTileAt)
        //.Where(neighborTile => neighborTile).ToList();

        return neighbors;
    }


    public class PathNode
    {
        public int GCost; // from the beginnings
        public int HCost; // estimated cost to the destiny (heuristic)
        public PathNode Parent; // to rebuild the path
        public Tile Tile;

        public PathNode(Tile tile, int gCost, int hCost, PathNode parent)
        {
            Tile = tile;
            GCost = gCost;
            HCost = hCost;
            Parent = parent;
        }

        public int FCost => GCost + HCost; // Final cost or total cost
    }
}