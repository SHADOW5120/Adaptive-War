using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;    // Drag 'PausePnl' here
    public GameObject resultPanel;   // Drag 'ResultPnl' here

    [Header("Buttons")]
    public Button pauseBtn;          // Drag 'PauseBtn' here
    public Button continueBtn;       // Drag 'ContinueBtn' here
    public Button pauseHomeBtn;      // Drag 'HomeBtn' (from PausePnl) here
    public Button resultHomeBtn;     // Drag 'HomeBtn' (from ResultPnl) here

    [Header("Text Elements")]
    public TextMeshProUGUI resultTxt; // Drag 'ResultTxt' here

    [Header("Configuration")]
    public string homeSceneName = "HomeScene"; // Type your actual home scene name here

    void Start()
    {
        Debug.Log("[GameplayUIManager] Initializing UI Manager...");

        // 1. Auto-assign button click events via code (no need to click '+' in Inspector)
        if (pauseBtn != null) pauseBtn.onClick.AddListener(PauseGame);
        if (continueBtn != null) continueBtn.onClick.AddListener(ResumeGame);
        if (pauseHomeBtn != null) pauseHomeBtn.onClick.AddListener(GoHome);
        if (resultHomeBtn != null) resultHomeBtn.onClick.AddListener(GoHome);

        // 2. Set default states when gameplay starts
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Hide popup panels at the start
        if (pausePanel != null) pausePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        // Ensure game time is running normally
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);

        // Freeze game physics and updates
        Time.timeScale = 0f;
        Debug.Log("[GameplayUIManager] Game Paused.");
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

        // Unfreeze game physics and updates
        Time.timeScale = 1f;
        Debug.Log("[GameplayUIManager] Game Resumed.");
    }

    // Call this method from your Game Manager when the player wins or loses
    // Example: uiManager.ShowResult("Victory!");
    public void ShowResult(string resultMessage)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            if (resultTxt != null)
            {
                resultTxt.text = resultMessage;
            }
        }

        // Freeze game when result is shown
        Time.timeScale = 0f;
        Debug.Log($"[GameplayUIManager] Match Ended. Result: {resultMessage}");
    }

    private void GoHome()
    {
        // We MUST reset time scale to 1 before changing scenes, 
        // otherwise the next scene will be frozen!
        Time.timeScale = 1f;
        Debug.Log($"[GameplayUIManager] Returning to scene: {homeSceneName}");

        SceneManager.LoadScene(homeSceneName);
    }
}