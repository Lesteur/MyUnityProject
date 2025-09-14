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
    public TacticalController tacticalController;

    private void Start()
    {
        if (tacticalController == null)
            tacticalController = FindFirstObjectByType<TacticalController>();
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
        int maxJumpHeight = unit.JumpHeight;
        int maxFallHeight = unit.MaxFallHeight;

        Tile startTile  = tacticalController.GetTileAt(start);
        Tile targetTile = tacticalController.GetTileAt(target);

        var queue       = new PriorityQueue<TileNode>();
        var closedSet   = new HashSet<Vector2Int>();
        var visited     = new Dictionary<Vector2Int, int>();
        var startNode   = new TileNode(startTile, 0, GetHeuristic(start, target));

        queue.Enqueue(startNode);
        visited[start] = 0;

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (current.Position == target)
                return ReconstructPath(current); // Path found

            closedSet.Add(current.Position);

            // Expand jump paths
            ExpandJumpPaths(current, unit, visited, queue, null, target, closedSet);

            // Expand standard neighbors
            ExpandStandardNeighbors(current, unit, visited, queue, null, target, closedSet);
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
        int maxMovementPoints   = unit.MovementPoints;
        Tile startTile          = tacticalController.GetTileAt(start);

        var reachableTiles  = new List<Tile>();
        var visited         = new Dictionary<Vector2Int, int>();
        var queue           = new PriorityQueue<TileNode>();
        
        queue.Enqueue(new TileNode(startTile, 0, 0));

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (visited.ContainsKey(current.Position) && visited[current.Position] <= current.G)
                continue;

            visited[current.Position] = current.G;
            reachableTiles.Add(current.Tile);

            // Expand jump paths
            ExpandJumpPaths(current, unit, visited, queue, null);

            // Expand standard neighbors
            ExpandStandardNeighbors(current, unit, visited, queue, null);
        }

        return reachableTiles;
    }

    public List<PathResult> GetAllPathsDebug(Vector2Int start, Unit unit)
    {
        var allTiles = GetReachableTiles(start, unit);
        var results = new List<PathResult>();

        foreach (var tile in allTiles)
        {
            var path = FindPath(start, tile.GridPosition, unit);
            if (path != null && path.Count > 0)
            {
                results.Add(new PathResult(tile, path));
            }
        }

        return results;
    }

    /// <summary>
    /// Finds all paths from a start position to every reachable tile.
    /// </summary>
    /// <param name="start">Starting grid position.</param>
    /// <param name="unit">Unit for movement constraints.</param>
    /// <returns>List of reachable paths to each tile.</returns>
    public List<PathResult> GetAllPathsFrom(Vector2Int start, Unit unit)
    {
        int maxMovementPoints = unit.MovementPoints;
        Tile startTile = tacticalController.GetTileAt(start);

        var results     = new List<PathResult>();
        var visited     = new Dictionary<Vector2Int, int>();
        var paths       = new Dictionary<Vector2Int, TileNode>();
        var queue       = new PriorityQueue<TileNode>();
        var startNode   = new TileNode(startTile, 0, 0);

        queue.Enqueue(startNode);
        paths[start] = startNode;

        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (visited.ContainsKey(current.Position) && visited[current.Position] <= current.G)
                continue;

            visited[current.Position] = current.G;

            // Expand jump paths
            ExpandJumpPaths(current, unit, visited, queue, paths);

            // Expand standard neighbors
            ExpandStandardNeighbors(current, unit, visited, queue, paths);
        }

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
    /// Expands the neighbors of the current tile node for standard movement,
    /// considering the unit's movement points and traversability.
    /// </summary>
    /// <param name="current">Current tile node being processed.</param>
    /// <param name="unit">Unit for which the path is being calculated.</param>
    /// <param name="visited">Dictionary tracking visited tiles and their costs.</param>
    /// <param name="queue">Priority queue for processing tile nodes.</param>
    /// <param name="paths">Dictionary mapping grid positions to their corresponding tile nodes.</param>
    /// <param name="target">Optional target position for heuristic calculation.</param>
    /// <param name="closedSet">Optional set of closed positions to avoid revisiting.</param>
    private void ExpandStandardNeighbors(TileNode current, Unit unit, Dictionary<Vector2Int, int> visited, PriorityQueue<TileNode> queue, Dictionary<Vector2Int, TileNode> paths, Vector2Int? target = null, HashSet<Vector2Int> closedSet = null)
    {
        int maxMovementPoints = unit.MovementPoints;
        Vector2Int unitPos = unit.GridPosition;

        foreach (Tile neighbor in GetNeighbors(current.Tile))
        {
            if (closedSet != null && closedSet.Contains(neighbor.GridPosition))
                continue; // Skip if already visited

            if (!CanTraverse(current.Tile, neighbor, unit, false))
                continue;

            int moveCost = current.G + neighbor.GetMovementCost();

            if (moveCost > maxMovementPoints)
                continue;

            if (visited.TryGetValue(neighbor.GridPosition, out int existingCost) && moveCost >= existingCost)
                continue;

            if (target != null)
                visited[neighbor.GridPosition] = moveCost;

            int heuristic = target.HasValue ? GetHeuristic(unitPos, target.Value) : 0;

            TileNode neighborNode = new TileNode(neighbor, moveCost, heuristic, current);

            if (target != null)
                queue.EnqueueOrUpdate(neighborNode);
            else
                queue.Enqueue(neighborNode);

            if (paths != null)
                paths[neighbor.GridPosition] = neighborNode;
        }
    }

    /// <summary>
    /// Expands the jump paths from the current tile node, allowing for vertical movement
    /// within the unit's jump height and fall distance limits.
    /// </summary>
    /// <param name="current">Current tile node being processed.</param>
    /// <param name="unit">Unit for which the jump paths are being calculated.</param>
    /// <param name="visited">Dictionary tracking visited tiles and their costs.</param>
    /// <param name="queue">Priority queue for processing tile nodes.</param>
    /// <param name="paths">Dictionary mapping grid positions to their corresponding tile nodes.</param>
    /// <param name="target">Optional target tile grid position for heuristic calculation.</param>
    /// <param name="closedSet">Optional set of closed tiles to avoid revisiting.</
    private void ExpandJumpPaths(TileNode current, Unit unit, Dictionary<Vector2Int, int> visited, PriorityQueue<TileNode> queue, Dictionary<Vector2Int, TileNode> paths, Vector2Int? target = null, HashSet<Vector2Int> closedSet = null)
    {
        Tile startTile          = current.Tile;
        int initialHeight       = startTile.Height;
        int maxMovementPoints   = unit.MovementPoints;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        Vector2Int unitPos = unit.GridPosition;

        foreach (var dir in directions)
        {
            int moveSteps           = 1;
            int jumpCount           = 0;
            Vector2Int currentPos   = startTile.GridPosition + dir;
            Tile jumpTile           = tacticalController.GetTileAt(currentPos);

            if (closedSet != null && closedSet.Contains(currentPos))
                continue; // Skip if already visited

            if (jumpTile == null || jumpTile.Height >= initialHeight || 1 == initialHeight - jumpTile.Height)
                continue; // Skip if the first jump tile is invalid or at the same height or one step lower

            while (current.G + moveSteps <= maxMovementPoints && jumpCount <= unit.JumpHeight)
            {
                if (jumpTile == null)
                {
                    Debug.LogWarning("Jump path went out of bounds.");
                    break; // Out of bounds
                }

                int heightDelta = jumpTile.Height - initialHeight;

                if (unit.MaxFallHeight >= heightDelta * -1)
                {
                    // If the player can land here, we allow it
                    int totalMoveCost = current.G + moveSteps;

                    if (!jumpTile.IsWalkable() || (visited.TryGetValue(currentPos, out int existingCost) && totalMoveCost >= existingCost))
                    {
                        // If we found a more expensive path, we skip it
                        moveSteps ++;
                        jumpCount ++;
                        currentPos += dir;

                        jumpTile = tacticalController.GetTileAt(currentPos);

                        continue;
                    }
                    
                    if (target != null)
                            visited[currentPos] = totalMoveCost;

                    int heuristic = target.HasValue ? GetHeuristic(unitPos, target.Value) : 0;

                    TileNode jumpNode = new TileNode(jumpTile, totalMoveCost, heuristic, current);

                    if (target != null)
                        queue.EnqueueOrUpdate(jumpNode);
                    else
                        queue.Enqueue(jumpNode);

                    if (paths != null)
                        paths[currentPos] = jumpNode;
                }

                moveSteps ++;
                jumpCount ++;
                currentPos += dir;

                jumpTile = tacticalController.GetTileAt(currentPos);
            }
        }
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

        if (to.OccupyingUnit != null)
            return false; // Skip if occupied by another unit

        int heightDelta = to.Height - from.Height;

        if (heightDelta > unit.JumpHeight || heightDelta < -unit.MaxFallHeight)
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
            Tile neighbor = tacticalController.GetTileAt(tile.GridPosition + dir);
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
        public Vector2Int Position => Tile.GridPosition;
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