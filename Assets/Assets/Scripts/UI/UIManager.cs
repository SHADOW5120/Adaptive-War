using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading level scenes

public class UIManager : MonoBehaviour
{
    [Header("Main Panels Reference")]
    [SerializeField] private GameObject _homePanel;
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _historyPanel;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _dictionaryPanel;
    [SerializeField] private GameObject _levelPanel;

    private void Start()
    {
        // Initialize the UI by showing ONLY the Home Menu when the Main Menu scene starts
        ShowOnly(_homePanel);
    }

    /// <summary>
    /// Core function: Turns ON the target panel and turns OFF all others.
    /// </summary>
    private void ShowOnly(GameObject panelToShow)
    {
        if (_homePanel != null) _homePanel.SetActive(panelToShow == _homePanel);
        if (_settingPanel != null) _settingPanel.SetActive(panelToShow == _settingPanel);
        if (_historyPanel != null) _historyPanel.SetActive(panelToShow == _historyPanel);
        if (_shopPanel != null) _shopPanel.SetActive(panelToShow == _shopPanel);
        if (_dictionaryPanel != null) _dictionaryPanel.SetActive(panelToShow == _dictionaryPanel);
        if (_levelPanel != null) _levelPanel.SetActive(panelToShow == _levelPanel);
    }

    #region Button Click Events

    public void OnPlayButtonClicked()
    {
        ShowOnly(_levelPanel);
    }

    public void OnSettingsButtonClicked()
    {
        ShowOnly(_settingPanel);
    }

    public void OnHistoryButtonClicked()
    {
        ShowOnly(_historyPanel);
    }

    public void OnShopButtonClicked()
    {
        ShowOnly(_shopPanel);
    }

    public void OnDictionaryButtonClicked()
    {
        ShowOnly(_dictionaryPanel);
    }

    public void OnBackToHomeClicked()
    {
        ShowOnly(_homePanel);
    }

    /// <summary>
    /// Dynamic level selection event. 
    /// Pass the exact Scene Name (e.g., "Lv1", "Lv2") directly from the Unity Button Inspector.
    /// </summary>
    public void OnLevelSelected(string levelSceneName)
    {
        Debug.Log($"[UIManager] Loading Level Scene: {levelSceneName}");

        // Directly transition to the selected level scene.
        // Each scene will handle its own gameplay UI automatically.
        SceneManager.LoadScene(levelSceneName);
    }

    #endregion
}