using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ScreenShake.Instance.Shake(5f);
        }
    }

    private void TestToDrawPathLine()
    {
        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        GridPosition startGridPosition = new GridPosition(0, 0);

        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition, out int pathLength);

        for (int i = 0; i < pathGridPositionList.Count - 1; i++)
        {
            Debug.DrawLine(
                LevelGrid.Instance.GetWorldPosition(pathGridPositionList[i]),
                LevelGrid.Instance.GetWorldPosition(pathGridPositionList[i + 1]),
                Color.white,
                10f
                );
        }
    }
}
