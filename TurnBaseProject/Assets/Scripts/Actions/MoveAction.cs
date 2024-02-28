using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private const float STOPPING_DISTANCE = 0.1f;
    private const float MOVE_SPEED = 4f;
    private const float ROTATE_SPEED = 10f;

    [SerializeField] private int MAX_MOVE_DISTNACE = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPos = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        
        if (Vector3.Distance(targetPos, transform.position) > STOPPING_DISTANCE)
        {
            OnStartMoving?.Invoke(this, EventArgs.Empty);
            transform.position += moveDirection * MOVE_SPEED * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;

            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * ROTATE_SPEED);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -MAX_MOVE_DISTNACE; x <= MAX_MOVE_DISTNACE; x++)
        {
            for (int z = -MAX_MOVE_DISTNACE; z <= MAX_MOVE_DISTNACE; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;

                // UnValid GridPosition
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // Myself Unit
                if (unitGridPosition == testGridPosition)
                {
                    continue;
                }

                // GridPosition aleady occupied with another unit
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > MAX_MOVE_DISTNACE * Pathfinding.MOVE_STRAIGHT_COST)
                {
                    // PathLength is too long.
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() => "Move";

    public override void TakeAction(GridPosition endGridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), endGridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            Vector3 pathWorldPosition = LevelGrid.Instance.GetWorldPosition(pathGridPosition);
            positionList.Add(pathWorldPosition);
        }

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition();

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
