using System.Collections.Generic;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;

        unitList = new List<Unit>();    
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (var unit in unitList)
        {
            unitString += unit + "\n";
        }
        

        return gridPosition.ToString() + "\n" + unitString;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()  => unitList;
    public bool HasAnyUnit() => unitList.Count > 0;

    public Unit GetUnit() => unitList[0];
}
