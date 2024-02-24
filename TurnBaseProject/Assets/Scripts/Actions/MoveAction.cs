using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    private const float STOPPING_DISTANCE = 0.1f;
    private const float MOVE_SPEED = 4f;
    private const float ROTATE_SPEED = 10f;

    [SerializeField] private int MAX_MOVE_DISTNACE = 4;
    [SerializeField] private Animator unitAnimator;

    private Vector3 targetPos;

    protected override void Awake()
    {
        base.Awake();
        targetPos = transform.position;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 moveDirection = (targetPos - transform.position).normalized;
        
        if (Vector3.Distance(targetPos, transform.position) > STOPPING_DISTANCE)
        {
            transform.position += moveDirection * MOVE_SPEED * Time.deltaTime;
            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            isActive = false;
            unitAnimator.SetBool("IsWalking", false);
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * ROTATE_SPEED);
    }

    public void Move(GridPosition gridPosition)
    {
        isActive = true;
        targetPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList()
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }
}
