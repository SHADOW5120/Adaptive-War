using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro; // Required for TextMeshPro UI

public class MatchHistoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public List<TextMeshProUGUI> historyTexts;

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/MatchHistory.txt";

        // Automatically load and display history when the Home scene starts
        LoadAndDisplayHistory();
    }

    private void LoadAndDisplayHistory()
    {
        List<string> currentHistory = new List<string>();

        // 1. Read the data if the file exists
        if (File.Exists(saveFilePath))
        {
            currentHistory = new List<string>(File.ReadAllLines(saveFilePath));
            Debug.Log("[MatchHistoryDisplay] History data loaded.");
        }
        else
        {
            Debug.LogWarning("[MatchHistoryDisplay] No save file found. Displaying default text.");
        }

        // 2. Update the UI text elements
        for (int i = 0; i < historyTexts.Count; i++)
        {
            if (historyTexts[i] == null) continue; // Safety check

            if (i < currentHistory.Count)
            {
                // Assign recorded match data
                historyTexts[i].text = currentHistory[i];
            }
            else
            {
                // Fill empty slots if fewer than 3 matches have been played
                historyTexts[i].text = "No Record";
            }
        }
    }
}