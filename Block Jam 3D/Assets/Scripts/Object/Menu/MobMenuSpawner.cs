using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MobMenuSpawner : MonoBehaviour
{
    public static MobMenuSpawner Instance { get; private set; }
    [SerializeField] List<MobMenu> mobMenus = new List<MobMenu>();
    public List<MobMenu> MobMenus => mobMenus;
    [SerializeField] int mobMenuNum;
    [SerializeField] Transform mobMenuContainter;
    [SerializeField] ColorConfig colorConfig;
    [SerializeField] Transform template;
    [SerializeField] Vector2 horizontal, vertical;
    [SerializeField] float timeFly, timeBetweenMob;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    private void Start()
    {
        //InvokeRepeating("ActivateMob", 0, timeBetweenMob);
        
    }

    [Button]
    void SpawnMob()
    {
        if (mobMenuNum == 0)
            return;
        mobMenus.Clear();

        while (mobMenuContainter.childCount > 0)
        {
            DestroyImmediate(mobMenuContainter.GetChild(0).gameObject);
        }

        for (int i = 0; i < mobMenuNum; i++)
        {
            Transform clone = Instantiate(template, new Vector3(horizontal.x, Random.Range(vertical.x, vertical.y)),Quaternion.identity, mobMenuContainter);
            mobMenus.Add(clone.GetComponent<MobMenu>());
        }
    }

    public IEnumerator Cor_ActivateMob()
    {
        ActivateMob();
        yield return new WaitForSeconds(timeBetweenMob);
        StartCoroutine(Cor_ActivateMob());
    }

    void ActivateMob()
    {
        if (mobMenus.Count == 0)
            return;
        MobMenu currentMob = mobMenus[0];
        mobMenus.Remove(currentMob);
        ColorType type = (ColorType)Random.Range(1, 5);
        currentMob.SetUp(colorConfig.ColorDict[type], horizontal, vertical, timeFly);
    }

    public void ClearMat()
    {
        for (int i = 0;i < mobMenuContainter.childCount; i++)
        {
            mobMenuContainter.GetChild(i).GetComponent<MobMenu>().ClearMat();
        }
    }
}
