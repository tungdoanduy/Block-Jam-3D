using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Column
{
    public List<Slot> slots = new List<Slot>();
}

public class LevelController : MonoBehaviour
{
    public static LevelController Instance {  get; private set; }
    [SerializeField] Transform slotContainer;
    [SerializeField] Transform groundSlotContainer;
    [SerializeField,ReadOnly] List<Slot> slots = new List<Slot>();
    [SerializeField,ReadOnly] List<Column> groundSlots = new List<Column>();
    [SerializeField] Transform edge;
    float edgeZ;
    int currentSlot = 0;//max = 7
    [SerializeField] ColorConfig colorConfig;
    public ColorConfig ColorConfig => colorConfig;

    [SerializeField, Foldout("Spawn")] int col, row;
    [SerializeField, Foldout("Spawn")] GameObject mobPrefab;
    [SerializeField, Foldout("Spawn")] Transform mobContainer;
    public Transform MobContainer => mobContainer;
    [Button]
    void SpawnMob()
    {
        GameObject clone = Instantiate(mobPrefab, groundSlots[col - 1].slots[row-1].transform.position + new Vector3(0,0.01f,0),Quaternion.identity,mobContainer);
        groundSlots[col - 1].slots[row - 1].Mob = clone.GetComponent<Mob>();
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        edgeZ = edge.position.z;
    }

    [Button]
    void SetUp()
    {
        slots.Clear();
        for (int i =0;i<slotContainer.childCount;i++)
        {
            slots.Add(slotContainer.GetChild(i).GetComponent<Slot>());
        }

        groundSlots.Clear();
        for (int i = 0; i < groundSlotContainer.childCount; i++)
        {
            Column col = new Column();
            Transform column = groundSlotContainer.GetChild(i);
            for (int j = 0; j < column.childCount; j++)
            {
                col.slots.Add(column.GetChild(j).GetComponent<Slot>());
            }
            groundSlots.Add(col);
        }
    }

   

    public void MoveMob(Vector2Int mobPos, Mob mob)
    {
        mob.Anim.SetBool("move",true);
        mob.transform.DOMoveZ(edgeZ, 0.5f).OnComplete(() =>
        {
            slots[currentSlot].Mob = mob;
            SortMob(currentSlot);
            /*float angle = Vector3.Angle(destination, new Vector3(0, 0, -1));
            if (destination.x > mob.transform.position.x)
                angle *= -1;
            mob.transform.DORotate(new Vector3(0, angle, 0), 0.1f).SetEase(Ease.OutFlash).OnComplete(() =>
            {
                mob.transform.DOMove(destination, 0.5f).OnComplete(() =>
                {
                    mob.transform.DOMove(destination, 0.5f);
                    mob.transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutFlash);
                    mob.Anim.Play("idle");
                });
            });*/
            //StartCoroutine(Cor_Connect(currentSlot));
            currentSlot++;
        });
        groundSlots[mobPos.x].slots[mobPos.y].Mob = null;

        for (int i = mobPos.y; i >= 0; i--)
        {
            if (groundSlots[mobPos.x].slots[i].Mob != null)
            {
                groundSlots[mobPos.x].slots[i].Mob.Activate();
                return;
            }
        }
        
    }

    void SortMob(int currentSlot)
    {
        for (int i = currentSlot - 1; i >= 0; i--)
        {
            if (slots[i].Mob.ColorType == slots[currentSlot].Mob.ColorType)
            {
                if (i == currentSlot - 1)
                    break;
                Mob tempMob = slots[currentSlot].Mob;
                for (int j = currentSlot - 1; j > i; j--)
                {
                    slots[j].Mob.Move(new Vector3(slots[j + 1].transform.position.x, 0.06f, slots[j + 1].transform.position.z));
                    slots[j + 1].Mob = slots[j].Mob;
                }
                tempMob.Move(new Vector3(slots[i+1].transform.position.x, 0.06f, slots[i+1].transform.position.z));
                slots[i+1].Mob = tempMob;
                StartCoroutine(Cor_Connect(i + 1));
                return;
            }
        }
        slots[currentSlot].Mob.Move(new Vector3(slots[currentSlot].transform.position.x, 0.06f, slots[currentSlot].transform.position.z));
        StartCoroutine(Cor_Connect(currentSlot));
    }

    IEnumerator Cor_Connect(int currentSlot)
    {
        if (currentSlot>=2 && slots[currentSlot-1].Mob.ColorType == slots[currentSlot].Mob.ColorType && slots[currentSlot - 2].Mob.ColorType == slots[currentSlot].Mob.ColorType)
        {
            Mob mob1 = slots[currentSlot - 2].Mob, mob2 = slots[currentSlot - 1].Mob, mob3 = slots[currentSlot].Mob;
            for (int i = currentSlot + 1; i <= this.currentSlot; i++)
            {
                if (slots[i].Mob == null)
                    break;
                slots[i].Mob.Move(new Vector3(slots[i - 3].transform.position.x, 0.06f, slots[i - 3].transform.position.z));
                slots[i - 3].Mob = slots[i].Mob;
                slots[i].Mob = null;
            }
            this.currentSlot -= 3;
            mob1.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob2.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob3.Disappear();
        }
    }

    public void Victory()
    {
        print("Victory");
    }

    void Lose()
    {
        print("Lose");
    }
}
