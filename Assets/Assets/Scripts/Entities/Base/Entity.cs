using Core;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Setup")]
    public EntityType Type;
    public float MaxHealth = 100f;

    public float CurrentHealth { get; protected set; }
    public bool IsDead => CurrentHealth <= 0;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");

        Destroy(gameObject);
    }
}