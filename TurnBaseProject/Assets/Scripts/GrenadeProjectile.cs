using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private const float REACHED_TARGET_DISTANCE = 0.2f;
    private const float MOVE_SPEED = 15f;
    private const float DAMAGE_RADIUS = 4f;
    private const int DAMAGE = 30; // temp

    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodedVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Vector3 positionXZ;
    private float totalDistance;
    private Action onGrenadeBehaviorComplete;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;

        positionXZ += moveDir * MOVE_SPEED * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeigth = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeigth;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        if (Vector3.Distance(positionXZ, targetPosition) < REACHED_TARGET_DISTANCE)
        {
            Collider[] colliderArray = Physics.OverlapSphere(positionXZ, DAMAGE_RADIUS);
            foreach (Collider collider in colliderArray) 
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(DAMAGE);
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            Instantiate(grenadeExplodedVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            trailRenderer.transform.parent = null;
            Destroy(trailRenderer.gameObject, 0.2f);
            Destroy(gameObject);

            onGrenadeBehaviorComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete)
    {
        this.onGrenadeBehaviorComplete = onGrenadeBehaviorComplete; 
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
