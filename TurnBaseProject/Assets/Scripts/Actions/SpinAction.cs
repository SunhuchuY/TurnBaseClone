using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount > 360f)
        {
            ActionComplete();
        }
    }

    public override int GetActionPointsCost() => 2;

    public override string GetActionName() => "Spin";

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition currentGridPosition = unit.GetGridPosition();

        return new List<GridPosition>()
        {   
            currentGridPosition
        };
    }

    public override void TakeAction(GridPosition mouseGridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        totalSpinAmount = 0;
    }
}
