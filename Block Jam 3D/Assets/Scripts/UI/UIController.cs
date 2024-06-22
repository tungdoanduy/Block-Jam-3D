using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    [SerializeField] Image bg;
    [SerializeField] Transform settingPanel;
    [SerializeField] TMP_Text endLevelText;
    [SerializeField] int nextLevel;
    [SerializeField] Transform nextButton,retryButton,homeButton;
    [SerializeField] protected LevelConfig levelConfig;

    //[SerializeField] List<Sprite> clouds = new List<Sprite>();
    //[SerializeField] int num;
    [SerializeField] RectTransform cloudLeft, cloudRight;
    //[SerializeField] Image template;
    //[Button]
    //void SpawnLeft()
    //{
    //    while(cloudLeft.childCount > 0)
    //        DestroyImmediate(cloudLeft.GetChild(0).gameObject);
    //    for (int i = 0; i < num; i++)
    //    {
    //        Image clone = Instantiate(template, cloudLeft);
    //        clone.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-600, 0), Random.Range(-1080, 1080));
    //        clone.sprite = clouds[Random.Range(0, 3)];
    //    }
    //}
    //[Button]
    //void SpawnRight()
    //{
    //    while (cloudRight.childCount > 0)
    //        DestroyImmediate(cloudRight.GetChild(0).gameObject);
    //    for (int i = 0; i < num; i++)
    //    {
    //        Image clone = Instantiate(template, cloudRight);
    //        clone.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(0,600), Random.Range(-1080, 1080));
    //        clone.sprite = clouds[Random.Range(0, 3)];
    //    }
    //}


    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    protected virtual void Start()
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_TRANSITION_OUT);
        Sequence seq = DOTween.Sequence();
        seq.Join(cloudLeft.DOAnchorPosX(-800, 1).SetEase(Ease.Linear));
        seq.Join(cloudRight.DOAnchorPosX(800, 1).SetEase(Ease.Linear));
        seq.Play().OnComplete(() =>
        {
            if (LevelController.Instance != null)
            {
                SoundManager.Instance.PlayLoopSound(SoundType.GAMEPLAY_THEME);
                LevelController.Instance.Interactable = true;
            }
            else
            {
                SoundManager.Instance.PlayLoopSound(SoundType.HOME_THEME);
            }
        });
    }

    public void OpenSettingPanel()
    {
        settingPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        bg.raycastTarget = true;
        bg.DOFade(150f / 255, 0.25f);
    }

    public void CloseSettingPanel()
    {
        settingPanel.DOScale(0, 0.5f).SetEase(Ease.InBack);
        bg.DOFade(0, 0.25f).OnComplete(()=> bg.raycastTarget = false);
    }

    protected void LoadScene(string sceneName)
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_TRANSITION_IN);
        Sequence seq = DOTween.Sequence();
        seq.Join(cloudLeft.DOAnchorPosX(0, 1).SetEase(Ease.Linear));
        seq.Join(cloudRight.DOAnchorPosX(0, 1).SetEase(Ease.Linear));
        seq.Play().OnComplete(() => StartCoroutine(Cor_WaitForLoadScene(sceneName)));
    }
    IEnumerator Cor_WaitForLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    public void Next()
    {
        LoadScene(levelConfig.LevelDict[(SceneType)nextLevel]);
    }

    public void Retry()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Home()
    {
        LoadScene("Home");
    }

    public void Victory()
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_WIN);
        levelConfig.LevelUnlocked = nextLevel;
        endLevelText.text = SceneManager.GetActiveScene().name.ToUpper() + " PASSED";
        endLevelText.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        nextButton.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        bg.raycastTarget = true;
        bg.DOFade(150f / 255, 0.25f);
    }

    public void Lose()
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_LOSE);
        endLevelText.text = SceneManager.GetActiveScene().name.ToUpper() + " FAILED";
        endLevelText.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        retryButton.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        homeButton.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        bg.raycastTarget = true;
        bg.DOFade(150f / 255, 0.25f);
    }
}
