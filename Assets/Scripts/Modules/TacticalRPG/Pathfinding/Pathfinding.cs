using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Handles tile-based pathfinding using the A* algorithm and reachability checks,
/// with support for vertical movement constraints such as jump height and fall distance.
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
    /// Finds the optimal path from a start tile to a target tile using the A* algorithm.
    /// </summary>
    /// <param name="start">Starting tile grid position.</param>
    /// <param name="target">Target tile grid position.</param>
    /// <param name="unit">The unit for which the path is calculated, used for movement constraints.</param>
    /// <returns>List of tiles representing the path, or null if no path found.</returns>
    public List<Tile> FindPath(Vector2Int start, Vector2Int target, Unit unit)
    {
        int maxJumpHeight = unit.jumpHeight;
        int maxFallHeight = unit.maxFallHeight;

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

            if (current.Position == target)
                return ReconstructPath(current); // Path found

            closedSet.Add(current.Position);

            foreach (Tile neighbor in GetNeighbors(current.Tile))
            {
                Vector2Int pos = neighbor.gridPosition;

                if (closedSet.Contains(pos))
                    continue;

                if (!CanTraverse(current.Tile, neighbor, unit, false))
                    continue;

                int heightDelta = neighbor.height - current.Tile.height;
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

        return null; // No path found
    }

    /// <summary>
    /// Finds all tiles reachable from a start position within the unit's movement limits.
    /// </summary>
    /// <param name="start">Starting tile grid position.</param>
    /// <param name="unit">Unit for movement constraints.</param>
    /// <returns>List of reachable tiles.</returns>
    public List<Tile> GetReachableTiles(Vector2Int start, Unit unit)
    {
        int maxMovementPoints = unit.movementPoints;

        var reachableTiles = new List<Tile>();
        var visited = new Dictionary<Vector2Int, int>();
        var queue = new Queue<TileNode>();

        Tile startTile = gridManager.GetTileAt(start);
        queue.Enqueue(new TileNode(startTile, 0, 0));

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (visited.ContainsKey(current.Position) && visited[current.Position] <= current.G)
                continue;

            visited[current.Position] = current.G;
            reachableTiles.Add(current.Tile);

            foreach (Tile neighbor in GetNeighbors(current.Tile))
            {
                if (!CanTraverse(current.Tile, neighbor, unit, false))
                    continue;

                int moveCost = current.G + neighbor.GetMovementCost();

                if (moveCost > maxMovementPoints)
                    continue;

                queue.Enqueue(new TileNode(neighbor, moveCost, 0));
            }
        }

        return reachableTiles;
    }

    /// <summary>
    /// Finds all paths from a start position to every reachable tile.
    /// </summary>
    /// <param name="start">Starting grid position.</param>
    /// <param name="unit">Unit for movement constraints.</param>
    /// <returns>List of reachable paths to each tile.</returns>
    public List<PathResult> GetAllPathsFrom(Vector2Int start, Unit unit)
    {
        int maxMovementPoints = unit.movementPoints;

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
                if (!CanTraverse(current.Tile, neighbor, unit, false))
                    continue;

                int moveCost = current.G + neighbor.GetMovementCost();

                if (moveCost > maxMovementPoints)
                    continue;

                if (visited.TryGetValue(neighbor.gridPosition, out int existingCost) && moveCost >= existingCost)
                    continue;

                var neighborNode = new TileNode(neighbor, moveCost, 0, current);
                queue.Enqueue(neighborNode);
                paths[neighbor.gridPosition] = neighborNode;
            }

            // TileNode node = kvp.Value;
            Tile startJumpTile = current.Tile;
            int initialHeight = startJumpTile.height;

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                int moveSteps = 1;
                int jumpCount = 0;
                Vector2Int currentPos = startJumpTile.gridPosition + dir;

                while (current.G + moveSteps <= maxMovementPoints && jumpCount <= unit.jumpHeight)
                {
                    Tile jumpTile = gridManager.GetTileAt(currentPos);

                    if (jumpTile == null)
                        break; // Stop if we hit an invalid tile

                    int heightDelta = jumpTile.height - initialHeight;

                    if (heightDelta >= 0)// || heightDelta <= unit.maxFallHeight)
                    {
                        if (heightDelta > unit.jumpHeight)
                            break; // Can't jump higher than allowed

                        int totalMoveCost = current.G + moveSteps;

                        if (visited.TryGetValue(currentPos, out int existingCost) && totalMoveCost >= existingCost)
                            break;

                        // Create new direct jump node
                        TileNode jumpNode = new TileNode(jumpTile, totalMoveCost, 0, current);

                        queue.Enqueue(jumpNode);
                        paths[currentPos] = jumpNode;
                    }
                    else
                    {
                        if (heightDelta < -unit.maxFallHeight)
                            break; // Can't fall further than allowed

                        int totalMoveCost = current.G + moveSteps;

                        if (!(visited.TryGetValue(currentPos, out int existingCost) && totalMoveCost >= existingCost))
                        {
                            //break;

                            // Create new direct jump node
                            TileNode jumpNode = new TileNode(jumpTile, totalMoveCost, 0, current);

                            queue.Enqueue(jumpNode);
                            paths[currentPos] = jumpNode;
                        }
                    }

                    moveSteps++;
                    jumpCount++;
                    currentPos += dir;
                }
            }
        }

        /*
        // LONG JUMPS (performed after normal moves)
        foreach (var kvp in paths.ToList())
        {
            TileNode node = kvp.Value;
            Tile startJumpTile = node.Tile;

            int initialHeight = startJumpTile.height;

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                int moveSteps = 1;
                int jumpCount = 0;
                Vector2Int currentPos = startJumpTile.gridPosition + dir;

                while (node.G + moveSteps <= maxMovementPoints && jumpCount <= unit.jumpHeight)
                {
                    Tile jumpTile = gridManager.GetTileAt(currentPos);

                    if (jumpTile == null)
                        break; // Stop if we hit an invalid tile

                    int heightDelta = jumpTile.height - initialHeight;

                    if (heightDelta >= 0)// || heightDelta <= unit.maxFallHeight)
                    {
                        if (heightDelta > unit.jumpHeight)
                            break; // Can't jump higher than allowed

                        int totalMoveCost = node.G + moveSteps;

                        if (visited.TryGetValue(currentPos, out int existingCost) && totalMoveCost >= existingCost)
                            break;

                        // Create new direct jump node
                        TileNode jumpNode = new TileNode(jumpTile, totalMoveCost, 0, node);

                        queue.Enqueue(jumpNode);
                        paths[currentPos] = jumpNode;
                        visited[currentPos] = totalMoveCost;
                    }
                    else
                    {
                        if (heightDelta < -unit.maxFallHeight)
                            break; // Can't fall further than allowed

                        int totalMoveCost = node.G + moveSteps;

                        if (!(visited.TryGetValue(currentPos, out int existingCost) && totalMoveCost >= existingCost))
                        {
                            //break;

                            // Create new direct jump node
                            TileNode jumpNode = new TileNode(jumpTile, totalMoveCost, 0, node);

                            queue.Enqueue(jumpNode);
                            paths[currentPos] = jumpNode;
                            visited[currentPos] = totalMoveCost;
                        }
                    }

                    moveSteps++;
                    jumpCount++;
                    currentPos += dir;
                }
            }
        }
        */

        // Rebuild paths from start to each reachable tile
        foreach (var kvp in paths)
        {
            if (kvp.Key == start)
                continue; // Skip start tile

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
    /// Reconstructs the path from the target back to the start.
    /// </summary>
    private List<Tile> ReconstructPath(TileNode endNode)
    {
        var path = new List<Tile>();
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
    /// Calculates Manhattan distance heuristic between two points.
    /// </summary>
    private int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// Checks whether a unit can move from one tile to another, considering vertical limits.
    /// </summary>
    private bool CanTraverse(Tile from, Tile to, Unit unit, bool isFinalTile)
    {
        if (to == null)
            return false;

        int heightDelta = to.height - from.height;

        if (heightDelta > unit.jumpHeight || heightDelta < -unit.maxFallHeight)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the four orthogonal neighboring tiles of a given tile.
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
            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    /// <summary>
    /// Internal node representation for tiles used during pathfinding.
    /// </summary>
    private class TileNode : System.IComparable<TileNode>
    {
        public Tile Tile;
        public Vector2Int Position => Tile.gridPosition;
        public int G; // Cost from start
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

        public int CompareTo(TileNode other) => F.CompareTo(other.F);

        public override bool Equals(object obj) => obj is TileNode other && Position == other.Position;

        public override int GetHashCode() => Position.GetHashCode();
    }
}