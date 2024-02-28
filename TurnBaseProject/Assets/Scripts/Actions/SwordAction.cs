using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }

    private const float ROTATION_SPEED = 10;
    private const float AFTER_HIT_TIME = 0.5f;
    private const float BEFORE_HIT_TIME = 1f;

    public static event EventHandler OnAnySwordHit;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    [SerializeField] private TrailRenderer trailRenderer;

    private int maxSwordDistance = 1;
    private float stateTimer;
    private State state;
    private Unit targetUnit;

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

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * ROTATION_SPEED);
                break;
            case State.SwingingSwordAfterHit:
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
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                stateTimer = AFTER_HIT_TIME;
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;

            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName() => "Sword";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction 
        {
            gridPosition = gridPosition,  
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
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
        stateTimer = BEFORE_HIT_TIME;
        state = State.SwingingSwordBeforeHit;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        trailRenderer.Clear();

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onCompleteAction);
    }

    public int GetMaxSwordDistance() => maxSwordDistance;
}
