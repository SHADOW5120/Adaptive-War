using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDataSO", menuName = "Scriptable Objects/BuildingDataSO")]
public class BuildingDataSO : EntityDataSO
{
    [Header("Building Specific Stats")]
    public float buildTime = 5f;
    public bool providesPopulation = false;
    public int populationBonus = 5;

    private void Awake()
    {
        entityType = Core.EntityType.Building;
    }

}
