using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("State Control")]
        public GameState CurrentGameState { get; private set; }

        [Header("System References")]
        public GameplayUIManager uiManager;
        public MatchHistorySaver historySaver;

        [Tooltip("Drag the object containing UnitSpawner2D here to read player money")]
        public UnitSpawner2D unitSpawner;

        [Header("Player Tracking")]
        public int activePlayerUnits = 0; // Tracks currently living units

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CurrentGameState = GameState.Playing;
            Debug.Log("[GameManager] System initialized. Current State: Playing.");
        }

        // Called automatically by the Unit script when it spawns
        public void RegisterUnitSpawned()
        {
            activePlayerUnits++;
        }

        // Called automatically by the Unit script when it dies
        public void RegisterUnitDied()
        {
            activePlayerUnits--;
            CheckDefeatCondition();
        }

        // Core logic to determine if the player has lost
        private void CheckDefeatCondition()
        {
            if (CurrentGameState != GameState.Playing) return;

            if (unitSpawner != null)
            {
                // Condition: Money is 0 (or less) AND no units are alive on the map
                if (unitSpawner.playerMoney <= 0 && activePlayerUnits <= 0)
                {
                    Debug.Log("[GameManager] No money and no units left! Triggering Defeat.");
                    SetGameOver();
                }
            }
        }

        public void SetGameOver()
        {
            if (CurrentGameState != GameState.Playing) return;

            CurrentGameState = GameState.GameOver;
            Debug.Log("[GameManager] Activating Game Over UI.");

            if (uiManager != null) uiManager.ShowResult("DEFEAT");
            if (historySaver != null) historySaver.SaveMatchResult("Defeat - Out of resources");
        }

        public void SetVictory()
        {
            if (CurrentGameState != GameState.Playing) return;

            CurrentGameState = GameState.Victory;
            Debug.Log("[GameManager] Enemy building destroyed! Activating Victory UI.");

            if (uiManager != null) uiManager.ShowResult("VICTORY");
            if (historySaver != null) historySaver.SaveMatchResult("Victory - Enemy Defeated");
        }
    }
}