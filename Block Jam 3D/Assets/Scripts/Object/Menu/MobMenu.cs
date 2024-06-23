using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobMenu : Mob
{

    public void SetUp(Material mobMat, Vector2 horizontal, Vector2 vertical, float timeFly)
    {
        gameObject.SetActive(true);
        this.mobMat = Instantiate(mobMat);
        foreach (MeshRenderer part in parts)
            part.material = mobMat;
        transform.DOMove(new Vector3(horizontal.y, Random.Range(vertical.x, vertical.y)), timeFly).SetEase(Ease.Linear);
        transform.DORotate(new Vector3(Random.Range(90, 360f), Random.Range(90, 360f), Random.Range(90, 360f)), timeFly).SetEase(Ease.Linear).OnComplete(() =>
        {
            MobMenuSpawner.Instance.MobMenus.Add(this);
            transform.position = new Vector3(horizontal.x, Random.Range(vertical.x, vertical.y));
            transform.rotation = Quaternion.identity;
            ClearMat();
            gameObject.SetActive(false);
        });
    }

    public void ClearMat()
    {
        DestroyImmediate(mobMat);
    }

    protected override void Start()
    {
        
    }
}
