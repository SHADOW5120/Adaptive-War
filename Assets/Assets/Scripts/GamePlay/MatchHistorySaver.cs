using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MatchHistorySaver : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        // Define the save path early in Awake
        saveFilePath = Application.persistentDataPath + "/MatchHistory.txt";
    }

    // Call this method when the match ends
    public void SaveMatchResult(string newResult)
    {
        List<string> currentHistory = new List<string>();

        // 1. Read existing data if the file exists
        if (File.Exists(saveFilePath))
        {
            currentHistory = new List<string>(File.ReadAllLines(saveFilePath));
        }

        // 2. Insert the newest match at the top
        currentHistory.Insert(0, newResult);

        // 3. Keep ONLY the last 3 matches
        if (currentHistory.Count > 3)
        {
            currentHistory.RemoveAt(currentHistory.Count - 1);
        }

        // 4. Save back to the .txt file
        File.WriteAllLines(saveFilePath, currentHistory);

        Debug.Log("[MatchHistorySaver] Match saved successfully to: " + saveFilePath);
    }
}