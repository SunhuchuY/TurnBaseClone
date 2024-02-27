using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PathNode
{

    private GridPosition gridPosition;
    
    /// <summary> Heuristic Cost to reach End Node </summary>
    private int hCost;

    /// <summary> Walking Cost from the Start Node </summary>
    private int gCost;

    /// <summary> hCost + hCost </summary>
    private int fCost;

    private PathNode cameFromPathNode;

    private bool isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;   
    }


    public override string ToString()
    {
        return gridPosition.ToString();
    }

    /// <summary> Set Walking Cost from the Start Node </summary>
    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }

    /// <summary> Set Heuristic Cost to reach End Node </summary>
    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    /// <summary> Calculate hCost + hCost </summary>
    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public int GetFCost() => fCost;
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public bool IsWalkable() => isWalkable;
    public GridPosition GetGridPosition() => gridPosition;
    public PathNode GetCameFromPathNode() => cameFromPathNode;
}
