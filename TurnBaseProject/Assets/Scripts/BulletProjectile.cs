using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private const float MOVE_SPEED = 80f;

    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;   
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float moveBeforeDistance = Vector3.Distance(targetPosition, transform.position);

        transform.position += moveDir * MOVE_SPEED * Time.deltaTime;

        float moveAfterDistance = Vector3.Distance(targetPosition, transform.position);

        if (moveBeforeDistance < moveAfterDistance)
        {
            trailRenderer.transform.parent = null;

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
            
            Destroy(trailRenderer.gameObject, 0.08f);
            Destroy(gameObject);
        }
    }
}
