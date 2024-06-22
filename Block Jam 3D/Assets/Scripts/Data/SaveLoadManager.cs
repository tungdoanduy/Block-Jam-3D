using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    [SerializeField] LevelConfig config;

    [Button]
    public void Clear()
    {
        SaveSystem.SaveData(new GameData());
        config.ForceSetLevelUnlocked(SaveSystem.LoadData().levelUnlocked);
    }

    [Button]
    public void OpenAllLevel()
    {
        SaveSystem.SaveData(new GameData(3));
        config.ForceSetLevelUnlocked(SaveSystem.LoadData().levelUnlocked);
    }

    [Button]
    public void Save()
    {
        GameData gameData = new GameData(config.LevelUnlocked);
        SaveSystem.SaveData(gameData);
    }

    [Button]
    public void Load()
    {
        GameData gameData = SaveSystem.LoadData();
        config.LevelUnlocked = gameData.levelUnlocked;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
