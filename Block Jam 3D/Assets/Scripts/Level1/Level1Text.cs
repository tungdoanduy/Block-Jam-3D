using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level1Text : MonoBehaviour
{
    [SerializeField] Color blue, yellow;
    [SerializeField] TMP_Text text;
    private void Start()
    {
        text.color = yellow;
        RunText();
    }

    void RunText()
    {
        text.DOColor(blue, 0.2f);
        transform.DOScale(0.95f, 3).SetEase(Ease.Linear).OnComplete(() =>
        {
            text.DOColor(yellow, 0.2f);
            transform.DOScale(1, 3).SetEase(Ease.Linear).OnComplete(() =>
            {
                RunText();
            });
        });
    }
}
