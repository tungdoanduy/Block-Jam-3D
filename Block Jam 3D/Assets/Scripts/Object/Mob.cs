using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] BoxCollider col;
    public Animator Anim => anim;
    [SerializeField] protected List<MeshRenderer> parts = new List<MeshRenderer>();
    [SerializeField] GameObject eyes;
    [SerializeField] Vector2Int pos;
    public Vector2Int Pos 
    {   get => pos;
        set => pos = value; 
    }

    [SerializeField] ColorType colorType;
    public ColorType ColorType => colorType;
    Tween moveTween;
    Coroutine corMove;
    [SerializeField] bool activate = false;
    protected Material mobMat;
    [SerializeField, ReadOnly] List<Slot> path;
    public List<Slot> Path
    {
        set
        {
            if (value != null)
            {
                Activate();
                path = value;
            }
        }
    }

    protected virtual void Start()
    {
        SetUp();
    }

    public void SetUpAfterSpawn(ColorType mobType)
    {
        colorType = mobType;
    }

    protected virtual void SetUp()
    {
        mobMat = Instantiate(LevelController.Instance.ColorConfig.ColorDict[colorType]);
        foreach (MeshRenderer part in parts)
            part.material = mobMat;
        anim.enabled = false;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, -0.36f, pos.z);
        col.enabled = false;
        mobMat.SetFloat("_Glossiness", 1);
        eyes.SetActive(false);
    }

    public void Activate()
    {
        if (activate)
            return;
        activate = true;
        transform.DOMoveY(0.06f, 0.25f).SetEase(Ease.OutFlash).OnComplete(() =>
        {
            anim.enabled = true;
            col.enabled = true;
            mobMat.SetFloat("_Glossiness", 0.3f);
            eyes.SetActive(true);
        });
    }
    public void MoveToEdge(float edgeZ)
    {
        SoundManager.Instance.PlaySound(SoundType.SFX_FOOTSTEP, 0.5f);
        anim.SetBool("move", true);
        StartCoroutine(Cor_MoveToEdge(edgeZ));
        //float durationPerSlot = 0.5f / (path.Count + 1);
        //Sequence seq = DOTween.Sequence();
        //bool down = true;
        //foreach (Slot slot in path)
        //{
        //    Vector3 destination = slot.transform.position;
        //    float angle = Vector3.Angle(destination - transform.position, down? new Vector3(0, 0, -1) : new Vector3(1,0,0));
        //    print(angle);
        //    if (angle > 80)// i mean for square angle
        //    {
        //        down = !down;
        //        if (destination.x > transform.position.x)
        //            angle *= -1;
        //        seq.Append(transform.DORotate(new Vector3(0, angle, 0), durationPerSlot * 0.2f).SetEase(Ease.OutFlash));
        //        seq.Append(transform.DOMove(destination, durationPerSlot * 0.8f).SetEase(Ease.Linear));
        //    }
        //    else
        //        seq.Append(transform.DOMove(destination, durationPerSlot).SetEase(Ease.Linear));

        //}
        //seq.Append(transform.DOMoveZ(edgeZ, durationPerSlot)).SetEase(Ease.Linear);
        //seq.Play();
    }

    IEnumerator Cor_MoveToEdge(float edgeZ)
    {
        float durationPerSlot = 0.5f / (path.Count + 1);
        bool down = true;
        foreach (Slot slot in path)
        {
            Vector3 destination = slot.transform.position;
            if (Mathf.Abs(destination.x - transform.position.x) > 0.5f && down)
            {
                down = false;
                if (destination.x > transform.position.x)
                    transform.DORotate(new Vector3(0, -90, 0), durationPerSlot * 0.2f).SetEase(Ease.OutFlash);
                else
                    transform.DORotate(new Vector3(0, 90, 0), durationPerSlot * 0.2f).SetEase(Ease.OutFlash);
                yield return new WaitForSeconds(durationPerSlot * 0.2f);
                transform.DOMove(destination, durationPerSlot * 0.8f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(durationPerSlot * 0.8f);
                continue;
            }
            if (Mathf.Abs(destination.z - transform.position.z) > 0.5f && !down)
            {
                down = true;
                transform.DORotate(new Vector3(0, 0, 0), durationPerSlot * 0.2f).SetEase(Ease.OutFlash);
                yield return new WaitForSeconds(durationPerSlot * 0.2f);
                transform.DOMove(destination, durationPerSlot * 0.8f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(durationPerSlot * 0.8f);
                continue;
            }
            transform.DOMove(destination, durationPerSlot).SetEase(Ease.Linear);
            yield return new WaitForSeconds(durationPerSlot);
        }
        transform.DOMoveZ(edgeZ, durationPerSlot).SetEase(Ease.Linear);
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
        transform.DOScale(0.25f, 0.25f).OnComplete(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.SFX_DISAPPEAR, 0.75f);
            transform.DOScale(0, 0.25f).SetEase(Ease.OutFlash).OnComplete(() =>
            {
                Destroy(mobMat);
                Destroy(gameObject);
            });
        });
    }

    private void OnDestroy()
    {
        if (LevelController.Instance != null && LevelController.Instance.MobContainer.childCount == 1)
            LevelController.Instance.Victory();
    }
}
