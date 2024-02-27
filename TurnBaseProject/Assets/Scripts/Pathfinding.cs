using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    public const int MOVE_STRAIGHT_COST = 10;
    public const int MOVE_DIAGNOL_COST = 14;
    public const float RAYCAST_OFFSET_DISTANCE = 5f;

    [SerializeField] private Transform gridDebugPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width; 
        this.height = height;   
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize
            , ((GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition)));
        gridSystem.CreateDebugObjects(gridDebugPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                if (Physics.Raycast(
                    worldPosition + Vector3.down * RAYCAST_OFFSET_DISTANCE, 
                    Vector3.up, 
                    RAYCAST_OFFSET_DISTANCE * 2, 
                    obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetTGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetTGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetTGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowerFCostPathNode(openList);
                
            if (currentNode == endNode)
            {
                // Reached Final Node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                if (!neighborNode.IsWalkable())
                {
                    closedList.Add(neighborNode);   
                    continue;
                }

                int tantativeGCost =
                    currentNode.GetGCost() + CalculateDistance(neighborNode.GetGridPosition(), endGridPosition);

                if (tantativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tantativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remain = Mathf.Abs(zDistance - xDistance);
        return (MOVE_DIAGNOL_COST * Mathf.Min(xDistance, zDistance) + (MOVE_STRAIGHT_COST * remain));
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetTGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return neighbourList;

    }

    private PathNode GetLowerFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowerPathNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowerPathNode.GetFCost())
            {
                lowerPathNode = pathNodeList[i];
            }
        }

        return lowerPathNode;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            PathNode cameFromPathNode = currentNode.GetCameFromPathNode();
            pathNodeList.Add(cameFromPathNode);
            currentNode = cameFromPathNode;
        }

        pathNodeList.Reverse();

        List<GridPosition> pathGridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            pathGridPositionList.Add(pathNode.GetGridPosition());
        }
        
        return pathGridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) => gridSystem.GetTGridObject(gridPosition).IsWalkable();

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) => FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    
    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
