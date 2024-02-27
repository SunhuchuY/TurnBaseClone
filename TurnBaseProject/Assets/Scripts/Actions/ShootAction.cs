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

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    public event EventHandler<OnShootEventArgs> OnShoot;

    private const float AIMING_STATE_TIME = 1f;
    private const float SHOOTING_STATE_TIME = 0.1f;
    private const float COOLOFF_STATE_TIME = 0.5f;
    private const float ROTATION_SPEED = 10f;

    [SerializeField] private int maxShootDistance = 2;
    [SerializeField] private LayerMask obstacleLayerMask;

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
        OnShoot?.Invoke(this, new OnShootEventArgs() {
            shootingUnit = unit,    
            targetUnit = targetUnit
        });
        
        targetUnit.Damage(40);
    }

    public override string GetActionName() => "Shoot";

    public Unit GetTargetUnit() => targetUnit;

    public int GetMaxShootDistance() => maxShootDistance;

    public int GetTargetCountAtPosition() => GetValidActionGridPositionList().Count;

    public override List<GridPosition> GetValidActionGridPositionList() => GetValidActionGridPositionList(unit.GetGridPosition());

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = - maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = - maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;

                // UnValid GridPosition
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
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

                Vector3 unitWorldPosition = unit.GetWorldPosition();
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(targetUnit.GetWorldPosition(), unitWorldPosition),
                    obstacleLayerMask
                    ))
                {
                    // Blocked the Obstacle.
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition mouseGridPosition, Action onCompleteAction)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);

        canShootBullet = true;
        state = State.Aiming;
        stateTimer = AIMING_STATE_TIME;

        ActionStart(onCompleteAction);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100)
        };
    }
}
