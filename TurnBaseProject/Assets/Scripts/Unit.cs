using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const float STOPPING_DISTANCE = 0.1f;
    private const float MOVE_SPEED = 4f;
    private const float ROTATE_SPEED = 10f;

    [SerializeField] private Animator unitAnimator;

    private Vector3 targetPos;

    private void Awake()
    {
        targetPos = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(targetPos, transform.position) > STOPPING_DISTANCE)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            
            transform.position += dir * MOVE_SPEED * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * ROTATE_SPEED);

            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking", false);
        }
    }
     
    public void Move(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }
}
