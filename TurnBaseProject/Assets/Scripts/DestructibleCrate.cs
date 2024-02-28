using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{

    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public void Damage()
    {
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
        
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToCrateDestoryed(crateDestroyedTransform, 150f, transform.position, 10f);

        Destroy(gameObject);
    }

    private void ApplyExplosionToCrateDestoryed(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRanged)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRanged);
            }

            ApplyExplosionToCrateDestoryed(child, explosionForce, explosionPosition, explosionRanged);
        }
    }

    public GridPosition GetGridPosition() => gridPosition;
}
