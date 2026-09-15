using Core;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "Scriptable Objects/UnitDataSO")]
public class UnitDataSO : EntityDataSO
{
    [Header("Unit Specific Stats")]
    public float moveSpeed = 5f;
    public float damage = 15f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;

    private void Awake()
    {
        entityType = Core.EntityType.Unit;
    }

}
