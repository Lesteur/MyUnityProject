using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles tile-based pathfinding using A* and reachability logic, with support for vertical movement constraints.
/// </summary>
public class Pathfinding : MonoBehaviour
{
    public GridManager gridManager;

    private void Start()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();
    }

    /// <summary>
    /// Finds an optimal path from the start tile to the target tile using the A* algorithm.
    /// </summary>
    /// <param name="start">Starting tile grid position.</param>
    /// <param name="target">Target tile grid position.</param>
    /// <param name="maxJump">Maximum height difference the unit can jump.</param>
    /// <returns>List of tiles representing the path, or null if no path found.</returns>
    public List<Tile> FindPath(Vector2Int start, Vector2Int target, int maxJump, int maxFallHeight)
    {
        var openSet = new PriorityQueue<TileNode>();
        var closedSet = new HashSet<Vector2Int>();
        var bestCosts = new Dictionary<Vector2Int, int>();

        Tile startTile = gridManager.GetTileAt(start);
        Tile targetTile = gridManager.GetTileAt(target);

        var startNode = new TileNode(startTile, 0, GetHeuristic(start, target));
        openSet.Enqueue(startNode);
        bestCosts[start] = 0;

        while (openSet.Count > 0)
        {
            TileNode current = openSet.Dequeue();

            // Target reached
            if (current.Position == target)
                return ReconstructPath(current);

            closedSet.Add(current.Position);

            foreach (Tile neighbor in GetNeighbors(current.Tile))
            {
                Vector2Int pos = neighbor.gridPosition;

                if (closedSet.Contains(pos))
                    continue;

                int heightDelta = neighbor.height - current.Tile.height;

                if (heightDelta > maxJump || heightDelta < -maxFallHeight)
                    continue;

                int climbPenalty = Mathf.Max(0, heightDelta);
                int tentativeG = current.G + neighbor.GetMovementCost() + climbPenalty;

                if (bestCosts.TryGetValue(pos, out int existingG) && tentativeG >= existingG)
                    continue;

                bestCosts[pos] = tentativeG;

                var neighborNode = new TileNode(
                    neighbor,
                    tentativeG,
                    GetHeuristic(pos, target),
                    current
                );

                openSet.EnqueueOrUpdate(neighborNode);
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all tiles reachable from a start position within movement and jump limits.
    /// </summary>
    /// <param name="start">Starting tile grid position.</param>
    /// <param name="maxMovement">Maximum movement points available.</param>
    /// <param name="maxJump">Maximum jump height.</param>
    /// <returns>List of reachable tiles.</returns>
    public List<Tile> GetReachableTiles(Vector2Int start, int maxMovement, int maxJump, int maxFallHeight)
    {
        var reachableTiles = new List<Tile>();
        var visited = new Dictionary<Vector2Int, int>(); // position -> cost
        var queue = new Queue<TileNode>();

        Tile startTile = gridManager.GetTileAt(start);
        queue.Enqueue(new TileNode(startTile, 0, 0));

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            // Skip if visited with better or equal cost
            if (visited.ContainsKey(current.Position) && visited[current.Position] <= current.G)
                continue;

            visited[current.Position] = current.G;
            reachableTiles.Add(current.Tile);

            foreach (Tile neighbor in GetNeighbors(current.Tile))
            {
                int heightDelta = neighbor.height - current.Tile.height;

                if (heightDelta > maxJump || heightDelta < -maxJump)
                    continue;

                int moveCost = current.G + neighbor.GetMovementCost();

                if (moveCost > maxMovement)
                    continue;

                queue.Enqueue(new TileNode(neighbor, moveCost, 0));
            }
        }

        return reachableTiles;
    }

    public List<PathResult> GetAllPathsFrom(Vector2Int start, int maxMovement, int maxJump, int maxFallHeight)
    {
        /*
        var allPaths = new List<PathResult>();
        var reachableTiles = GetReachableTiles(start, maxMovement, maxJump);

        foreach (Tile tile in reachableTiles)
        {
            List<Tile> path = FindPath(start, tile.gridPosition, maxJump);
            if (path != null && path.Count > 0)
            {
                allPaths.Add(new PathResult(tile, path));
            }
        }

        return allPaths;
        */

        var results = new List<PathResult>();
        var visited = new Dictionary<Vector2Int, int>();
        var paths = new Dictionary<Vector2Int, TileNode>();
        var queue = new PriorityQueue<TileNode>();

        Tile startTile = gridManager.GetTileAt(start);
        var startNode = new TileNode(startTile, 0, 0);
        queue.Enqueue(startNode);
        paths[start] = startNode;

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (visited.ContainsKey(current.Position) && visited[current.Position] <= current.G)
                continue;

            visited[current.Position] = current.G;

            foreach (Tile neighbor in GetNeighbors(current.Tile))
            {
                int heightDelta = neighbor.height - current.Tile.height;

                if (heightDelta > maxJump || heightDelta < -maxFallHeight)
                    continue;

                int moveCost = current.G + neighbor.GetMovementCost();

                if (moveCost > maxMovement)
                    continue;

                if (visited.TryGetValue(neighbor.gridPosition, out int existingCost) && moveCost >= existingCost)
                    continue;

                var neighborNode = new TileNode(neighbor, moveCost, 0, current);
                queue.Enqueue(neighborNode);
                paths[neighbor.gridPosition] = neighborNode;
            }
        }

        // Build all paths from start to each reachable tile
        foreach (var kvp in paths)
        {
            if (kvp.Key == start) continue; // skip origin

            var node = kvp.Value;
            var path = new List<Tile>();
            while (node != null)
            {
                path.Add(node.Tile);
                node = node.Parent;
            }

            path.Reverse();
            results.Add(new PathResult(path[^1], path));
        }

        return results;
    }

    /// <summary>
    /// Reconstructs the path from the goal node to the start node.
    /// </summary>
    private List<Tile> ReconstructPath(TileNode endNode)
    {
        List<Tile> path = new List<Tile>();
        TileNode current = endNode;

        while (current != null)
        {
            path.Add(current.Tile);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Manhattan distance heuristic for A*.
    /// </summary>
    private int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// Gets walkable neighbor tiles in the four cardinal directions.
    /// </summary>
    private List<Tile> GetNeighbors(Tile tile)
    {
        var neighbors = new List<Tile>();
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Tile neighbor = gridManager.GetTileAt(tile.gridPosition + dir);

            if (neighbor != null && neighbor.IsWalkable())
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    /// <summary>
    /// Represents a tile with associated pathfinding data (g-cost, h-cost, parent).
    /// Used in the A* algorithm.
    /// </summary>
    private class TileNode : System.IComparable<TileNode>
    {
        public Tile Tile;
        public Vector2Int Position => Tile.gridPosition;
        public int G; // Cost from start to current
        public int H; // Heuristic to target
        public int F => G + H; // Total estimated cost
        public TileNode Parent;

        public TileNode(Tile tile, int g, int h, TileNode parent = null)
        {
            Tile = tile;
            G = g;
            H = h;
            Parent = parent;
        }

        public int CompareTo(TileNode other)
        {
            return F.CompareTo(other.F);
        }

        public override bool Equals(object obj)
        {
            return obj is TileNode other && Position == other.Position;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}