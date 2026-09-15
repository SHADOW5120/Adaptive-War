using Core;
using UnityEngine;

public class Building : Entity
{
    [Tooltip("Check this if the building is the main base")]
    public bool IsMainBase = false;

    protected override void Start()
    {
        // Call the parent Start method to initialize health
        base.Start();
        Type = EntityType.Building;
    }

    protected override void Die()
    {
        // Execute death logic and destroy object from the base Entity class
        base.Die();

        // If this is the main base, trigger the end-game logic
        // Since only the opponent has buildings, destroying their Main Base 
        // will always result in a Player Victory.
        if (IsMainBase)
        {
            GameManager.Instance.SetVictory();
        }
    }
}