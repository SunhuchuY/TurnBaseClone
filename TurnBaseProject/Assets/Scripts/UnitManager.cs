using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance;

    private List<Unit> unitList;
    private List<Unit> enemyUnitList;
    private List<Unit> friendUnitList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;   
        }
        Instance = this;
        
        unitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
        friendUnitList = new List<Unit>();  
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit; 
        
        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);    
        }
        else
        {
            friendUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList() => unitList;
    public List<Unit> GetFriendUnitList() => friendUnitList;
    public List<Unit> GetEnemyUnitList() => enemyUnitList;
}
