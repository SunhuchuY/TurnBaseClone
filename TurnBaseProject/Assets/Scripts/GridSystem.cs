using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private const float DEBUG_DRAWLINE_DURATION = 1000f;

    private int width;
    private int height;
    private float cellSize;

    private TGridObject[,] TgridObjectArray;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        TgridObjectArray = new TGridObject[width, height];    

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                TgridObjectArray[x, z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x * cellSize, 0, gridPosition.z * cellSize);

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
            );
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), debugPrefab.transform.rotation);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetTGridObject(GetTGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetTGridObject(GridPosition gridPosition) => TgridObjectArray[gridPosition.x, gridPosition.z];
    public int GetWidth() => width;
    public int GetHeight() => height;
    public bool IsValidGridPosition(GridPosition gridPosition) 
    {
        return 
            gridPosition.x >= 0 && 
            gridPosition.x < width && 
            gridPosition.z >= 0 && 
            gridPosition.z < height;
    }
}