using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    private const int MAX_SHOOT_DISTNACE = 7;
    private const float AIMING_STATE_TIME = 1f;
    private const float SHOOTING_STATE_TIME = 0.1f;
    private const float COOLOFF_STATE_TIME = 0.5f;
    private const float ROTATION_SPEED = 10f;

    private Unit targetUnit;
    private State state;
    private float stateTimer;
    private bool canShootBullet;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * ROTATION_SPEED);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer < 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = SHOOTING_STATE_TIME;
                break;

            case State.Shooting:
                state = State.Cooloff;
                stateTimer = COOLOFF_STATE_TIME;
                break;

            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        targetUnit.Damage();
    }

    public override string GetActionName() => "Shoot";

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -MAX_SHOOT_DISTNACE; x <= MAX_SHOOT_DISTNACE; x++)
        {
            for (int z = -MAX_SHOOT_DISTNACE; z <= MAX_SHOOT_DISTNACE; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;

                // UnValid GridPosition
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > MAX_SHOOT_DISTNACE)
                {
                    continue;
                }

                // Myself Unit
                if (unitGridPosition == testGridPosition)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                // Both Units on same 'team'
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition mouseGridPosition, Action onCompleteAction)
    {
        ActionStart(onCompleteAction);

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);

        canShootBullet = true;
        state = State.Aiming;
        stateTimer = AIMING_STATE_TIME;
    }
}
