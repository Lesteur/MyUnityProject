using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the result of a pathfinding query: a destination tile and the path leading to it.
/// </summary>
public class PathResult
{
    /// <summary> The destination tile reached by this path. </summary>
    public Tile Destination { get; }

    /// <summary> Ordered list of tiles from start to destination. </summary>
    public List<Tile> Path { get; }

    /// <summary> Number of steps in the path. </summary>
    public int Length => Path.Count;

    /// <summary> Whether this result contains a valid, non-empty path. </summary>
    public bool IsValid => Destination != null && Path.Count > 0;

    public PathResult(Tile destination, List<Tile> path)
    {
        Destination = destination;
        Path = path ?? new List<Tile>();
    }

    public override string ToString()
    {
        return $"PathResult -> Destination: {Destination?.name ?? "null"}, Length: {Length}";
    }

    public bool ContainsTile(Tile tile)
    {
        return Path.Contains(tile);
    }
}

/*
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the result of a pathfinding query: a destination tile and the path leading to it.
/// </summary>
public readonly struct PathResult
{
    /// <summary> The destination tile reached by this path. </summary>
    public Tile Destination { get; }

    /// <summary> Ordered list of tiles from start to destination. </summary>
    public IReadOnlyList<Tile> Path { get; }

    /// <summary> Number of steps in the path. </summary>
    public int Length => Path.Count;

    /// <summary> Whether this result contains a valid, non-empty path. </summary>
    public bool IsValid => Destination != null && Path.Count > 0;

    public PathResult(Tile destination, List<Tile> path)
    {
        Destination = destination;
        Path = path ?? new List<Tile>();
    }

    public override string ToString()
    {
        return $"PathResult -> Destination: {Destination?.name ?? "null"}, Length: {Length}";
    }
}
*/