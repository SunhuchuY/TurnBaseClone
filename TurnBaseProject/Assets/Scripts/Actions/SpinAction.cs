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
            isActive = false;
            totalSpinAmount = 0;
        }
    }

    public void Spin()
    {
        isActive = true;
    }
}