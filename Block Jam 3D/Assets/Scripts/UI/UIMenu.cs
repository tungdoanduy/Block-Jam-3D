using System.Collections.Generic;
using UnityEngine;

public class UIMenu : UIController
{
    public static new UIMenu Instance { get; private set; }

    [SerializeField] List<MenuButton> levelButtons = new List<MenuButton>();
    

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LoadLevel(string sceneName)
    {
        LoadScene(sceneName);
    }

    protected override void Start()
    {
        base.Start();
        SaveLoadManager.Instance.Load();
        for (int i = 0; i < levelConfig.LevelUnlocked; i++)
        {
            levelButtons[i].Unlocked();
        }
    }
}
