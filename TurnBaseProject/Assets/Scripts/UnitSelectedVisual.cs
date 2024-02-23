using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnit += UnitActionSystem_ChangeSelectedUnit;
        UpdateVisual();
    }

    private void UnitActionSystem_ChangeSelectedUnit(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        
        if (unit == selectedUnit)
            meshRenderer.enabled = true;
        else
            meshRenderer.enabled = false;
    }
}
