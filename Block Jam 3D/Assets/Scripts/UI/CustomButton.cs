using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    [SerializeField] protected List<Image> parts = new List<Image>();
    [SerializeField] RectTransform sun, tree;
    [SerializeField] protected TMP_Text text;
    Sequence seq;
    [SerializeField] float interactTime;

    public void PointerDown()
    {
        seq.Kill();
        seq = DOTween.Sequence();
        foreach (Image part in parts)
        {
            seq.Join(part.DOFade(0.8f, interactTime));
        }
        seq.Join(text?.DOFade(0.8f, interactTime));
        seq.Join(transform.DOScale(0.8f, interactTime).SetEase(Ease.OutBack));
        sun?.DOAnchorPosX(66, interactTime);
        tree?.DOAnchorPosX(-66, interactTime);
        seq.Play();
        
    }

    public void PointerUp()
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_CLICK);
        seq.Kill();
        seq = DOTween.Sequence();
        foreach (Image part in parts)
        {
            seq.Join(part.DOFade(1, interactTime));
        }
        seq.Join(text?.DOFade(1, interactTime));
        seq.Join(transform.DOScale(1, interactTime).SetEase(Ease.OutBack));
        sun?.DOAnchorPosX(-407, interactTime);
        tree?.DOAnchorPosX(407, interactTime);
        seq.Play();
    }

    public void OpenSettingPanel()
    {
        UIController.Instance.OpenSettingPanel();
    }

    public void CloseSettingPanel()
    {
        UIController.Instance.CloseSettingPanel();
    }

    public void Next()
    {
        UIController.Instance.Next();
    }

    public void Retry()
    {
        UIController.Instance.Retry();
    }

    public void Home()
    {
        UIController.Instance.Home();   
    }
}
