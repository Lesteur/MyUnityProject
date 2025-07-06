using System.Collections.Generic;
using UnityEngine;

public class PathResult
{
    public Tile destination;
    public List<Tile> path;

    public PathResult(Tile destination, List<Tile> path)
    {
        this.destination = destination;
        this.path = path;
    }
}