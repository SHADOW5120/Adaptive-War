using UnityEngine;
using Core;

namespace Entities.Units
{
    public class Unit : Entity
    {
        [Header("Unit Alignment")]
        [Tooltip("Check this if the unit belongs to the player. Uncheck for enemy units.")]
        public bool isPlayerOwned = true;

        public UnitState CurrentState { get; set; }

        private UnitAnimator _animator;

        protected override void Start()
        {
            // Call Start() from the base Entity class to initialize Health
            base.Start();

            Type = EntityType.Unit;
            CurrentState = UnitState.Idle;

            _animator = GetComponent<UnitAnimator>();

            // Register player units with the GameManager to track defeat conditions
            if (isPlayerOwned && GameManager.Instance != null)
            {
                GameManager.Instance.RegisterUnitSpawned();
            }
        }

        protected override void Die()
        {
            if (CurrentState == UnitState.Die) return;

            CurrentState = UnitState.Die;

            Debug.Log($"[{gameObject.name}] has been defeated!");

            // Notify the GameManager that a player unit has died (-1 unit)
            // The GameManager will automatically check if this causes a Game Over
            if (isPlayerOwned && GameManager.Instance != null)
            {
                GameManager.Instance.RegisterUnitDied();
            }

            // Trigger the death animation instead of destroying the object immediately
            if (_animator != null)
            {
                _animator.TriggerDeath();
            }

            // Disable collider so dead units don't block A* pathfinding or get clicked
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Note: Destroy(gameObject) is removed from here. 
            // You should add an Animation Event at the last frame of the "Die" animation 
            // to call a function that actually destroys or pools this GameObject.
        }

        /// <summary>
        /// This method is called via an Animation Event at the very last frame of the "Die" animation.
        /// </summary>
        public void OnDeathAnimationComplete()
        {
            Debug.Log($"[{gameObject.name}] Death animation finished. Destroying object.");
            Destroy(gameObject);
        }
    }
}