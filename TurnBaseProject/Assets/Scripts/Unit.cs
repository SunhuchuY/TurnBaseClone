using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int MAX_ACTION_POINTS = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private MoveAction moveAction;  
    private SpinAction spinAction;
    private ShootAction shootAction;
    private BaseAction[] baseActionArray;
    private HealthSystem healthSystem;
    private GridPosition gridPosition;  
    private int actionPoints = MAX_ACTION_POINTS;
    
    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        shootAction = GetComponent<ShootAction>();
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();    
    }   

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Changed gridPosition
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition; 

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
       
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) 
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    } 

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            // Refill Action Points
            actionPoints = MAX_ACTION_POINTS;
            
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        
        Destroy(gameObject);
    
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;
    public ShootAction GetShootAction() => shootAction;
    public BaseAction[] GetBaseActionArray() => baseActionArray; 
    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) => actionPoints >= baseAction.GetActionPointsCost();
    public int GetActionPoints() => actionPoints;
    public float GetHealthNormalized() => healthSystem.GetHealthNormalized();
    public bool IsEnemy() => isEnemy;
}
