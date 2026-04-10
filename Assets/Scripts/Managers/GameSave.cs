using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;


public static class GameSave
{
    private static GameSettings currentSettings;
    private static string settingsFilePath;

    public const int MaxLevel = 15; 

    public static event Action OnChamgeSettings;
    
    public static void Init()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "Settings.json");
        LoadSettings();
    }
    
    public static void Save()
    {
        OnChamgeSettings?.Invoke();
        string json = JsonUtility.ToJson(currentSettings);
        File.WriteAllText(settingsFilePath, json);
    }
    
    private static void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            currentSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            currentSettings = new GameSettings();
        }
        Save();
    }

    public static GameSettings GetSettings() => currentSettings;

    public static void SetSettings(GameSettings settings)
    {
        currentSettings = settings;
    }
}

[Serializable]
public class GameSettings
{
    public int Level = 0;
    public int Coin = 0;
    public bool Vibration = true;
    public bool SFX = true; 
}
