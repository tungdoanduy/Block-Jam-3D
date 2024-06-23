using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    protected override IEnumerator Cor_WaitForLoadScene(string sceneName)
    {
        MobMenuSpawner.Instance.ClearMat();
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneName);
    }

    protected override void BeforeStart()
    {
        SaveLoadManager.Instance.Load();
        for (int i = 0; i < levelConfig.LevelUnlocked; i++)
        {
            levelButtons[i].Unlocked();
        }
    }

    protected override void AfterStart()
    {        
        StartCoroutine(MobMenuSpawner.Instance.Cor_ActivateMob());
    }

    protected override bool CheckInstance()
    {
        return SoundManager.Instance != null && SaveLoadManager.Instance != null;
    }
}
