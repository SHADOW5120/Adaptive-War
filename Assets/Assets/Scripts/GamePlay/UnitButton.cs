using UnityEngine;

public class UnitButton : MonoBehaviour
{
    [Header("Configuration")]
    public UnitDataSO unitData;
    public UnitSpawner2D spawner;

    public void OnButtonClicked()
    {
        if (unitData != null && spawner != null)
        {
            spawner.SelectUnit(unitData);
        }
    }
}