using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRegdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform unitRegdollPrefab;
    [SerializeField] private Transform originalRootBone;
    
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(unitRegdollPrefab, transform.position, transform.rotation);
        UnitRegdoll ragdoll = ragdollTransform.GetComponent<UnitRegdoll>();

        ragdoll.Setup(originalRootBone);
    }
}
