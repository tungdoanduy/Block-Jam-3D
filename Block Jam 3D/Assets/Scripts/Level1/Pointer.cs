using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] List<BoxCollider> mobs = new List<BoxCollider>();
    Collider currentMob;

    private void Start()
    {
        foreach (BoxCollider mob in mobs)
            mob.enabled = false;
        transform.DOScale(new Vector3(0.2f, 0.24f, 0.2f), 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        currentMob = mobs[0];
        currentMob.enabled = true;
        transform.position = currentMob.transform.position + new Vector3(0, 0.95f, -0.45f);
        mobs.RemoveAt(0);
    }

    private void Update()
    {
        if (currentMob == null)
        {
            if (mobs.Count > 0)
            {
                currentMob = mobs[0];
                currentMob.enabled = true;
                transform.position = currentMob.transform.position + new Vector3(0, 0.95f, -0.45f);
                mobs.RemoveAt(0);
            }
            else
                gameObject.SetActive(false);
        }
        foreach (BoxCollider mob in mobs)
            mob.enabled = false;
    }
}
