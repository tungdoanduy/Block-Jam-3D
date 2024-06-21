using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] Animator anim;
    public Animator Anim => anim;
    [SerializeField] List<MeshRenderer> parts = new List<MeshRenderer>();
    [SerializeField] Vector2Int pos;
    public Vector2Int Pos 
    {   get => pos;
        set => pos = value; 
    }

    [SerializeField] ColorType colorType;
    public ColorType ColorType => colorType;
    Tween moveTween;
    Coroutine corMove;
    [SerializeField] bool initActivate;
    Material mobMat;

    private void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        mobMat = Instantiate(LevelController.Instance.ColorConfig.ColorDict[colorType]);
        foreach (MeshRenderer part in parts)
            part.material = mobMat;

    }

    public void Activate()
    {

    }

    public void Move(Vector3 destination)
    {
        if (corMove!=null)
            StopCoroutine(corMove);
        corMove = StartCoroutine(Cor_Move(destination));
    }

    IEnumerator Cor_Move(Vector3 destination)
    {
        anim.SetBool("move", true);
        float angle = Vector3.Angle(destination - transform.position, new Vector3(0, 0, -1));
        if (destination.x > transform.position.x)
            angle *= -1;
        moveTween?.Kill();
        moveTween = transform.DORotate(new Vector3(0, angle, 0), 0.1f).SetEase(Ease.OutFlash);
        yield return moveTween.WaitForCompletion();
        moveTween = transform.DOMove(destination, 0.5f);
        yield return moveTween.WaitForCompletion();
        moveTween = transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutFlash);
        anim.SetBool("move", false);
    }

    public void Disappear()
    {
        transform.DOMoveY(0.12f, 0.25f);
        transform.DOScale(0.25f, 0.25f).OnComplete(() => transform.DOScale(0, 0.25f).SetEase(Ease.OutFlash).OnComplete(() => Destroy(gameObject)));
    }

    private void OnDestroy()
    {
        if (LevelController.Instance.MobContainer.childCount == 1)
            LevelController.Instance.Victory();
    }
}
