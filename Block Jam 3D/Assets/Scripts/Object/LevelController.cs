using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[System.Serializable]
public class Column
{
    public List<Slot> slots = new List<Slot>();
}

[System.Serializable]
public class Stage
{
    public Transform slotContainer;
    public Transform groundSlotContainer;
    public List<Slot> slots = new List<Slot>();
    public List<Column> groundSlots = new List<Column>();
    public Transform mobContainer,tunnelContainer;
    public float cameraPosX;
    public List<Vector2Int> outSlots;
    
    public List<Mob> mobs = new List<Mob>();
}

public class LevelController : MonoBehaviour
{
    public static LevelController Instance {  get; private set; }
    [SerializeField] List<Stage> stages = new List<Stage>();
    [SerializeField] int setUpStage = 0;
    [SerializeField] Transform edge;
    float edgeZ;
    int currentSlot = 0;//max = 7
    int currentStage = 0;
    [SerializeField] ColorConfig colorConfig;
    public ColorConfig ColorConfig => colorConfig;
    [SerializeField] Grid grid;

    [SerializeField, Foldout("Spawn")] int col, row;
    [SerializeField, Foldout("Spawn")] GameObject mobPrefab,tunnelPrefab;
    public Transform MobContainer => stages[currentStage].mobContainer;
    bool interactable = false;
    public bool Interactable
    {
        set => interactable = value;
    }
    [Button]
    void SpawnMob()
    {
        GameObject clone = Instantiate(mobPrefab, stages[setUpStage].groundSlots[col - 1].slots[row-1].transform.position,Quaternion.identity, stages[setUpStage].mobContainer);
        Mob scr = clone.GetComponent<Mob>();
        scr.Pos = new Vector2Int(row-1,col-1);
        stages[setUpStage].mobs.Add(scr);
    }

    [Button]
    void SpawnTunnel()
    {
        GameObject clone = Instantiate(tunnelPrefab, stages[setUpStage].groundSlots[col - 1].slots[row - 1].transform.position, Quaternion.identity, stages[setUpStage].tunnelContainer);
        clone.GetComponent<Tunnel>().Pos = new Vector2Int(row - 1, col - 1);
    }

    [Button]
    void SetUp()
    {
        stages[setUpStage].slots.Clear();
        for (int i = 0; i < stages[setUpStage].slotContainer.childCount; i++)
        {
            stages[setUpStage].slots.Add(stages[setUpStage].slotContainer.GetChild(i).GetComponent<Slot>());
        }

        stages[setUpStage].groundSlots.Clear();
        for (int i = 0; i < stages[setUpStage].groundSlotContainer.childCount; i++)
        {
            Column col = new Column();
            Transform column = stages[setUpStage].groundSlotContainer.GetChild(i);
            for (int j = 0; j < column.childCount; j++)
            {
                col.slots.Add(column.GetChild(j).GetComponent<Slot>());
            }
            stages[setUpStage].groundSlots.Add(col);
        }
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
        SetUpObject();
        CreateGrid();
    }

    private void Update()
    {
        if (!interactable)
            return;
        Click();
    }

    void SetUpObject()
    {
        foreach(Stage stage in stages)
        {
            for (int i = 0;i<stage.mobContainer.childCount;i++)
            {
                Mob temp = stage.mobContainer.GetChild(i).GetComponent<Mob>();
                stage.groundSlots[temp.Pos.y].slots[temp.Pos.x].Mob = temp;
            }
            if (stage.tunnelContainer!=null)
            {
                for (int i = 0; i < stage.tunnelContainer.childCount; i++)
                {
                    Tunnel temp = stage.tunnelContainer.GetChild(i).GetComponent<Tunnel>();
                    stage.groundSlots[temp.Pos.y].slots[temp.Pos.x].Tunnel = temp;
                }
            }
        }
    }

    void HandleInput(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Mob"))
            {
                Mob script = hit.collider.GetComponent<Mob>();
                MoveMob(script.Pos, script);
                ActivateTunnel(script.Pos);
                Destroy(hit.collider);
            }
        }
    }

    void Click()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 position = Input.mousePosition;
            HandleInput(position);
        }

#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector3 position = Input.GetTouch(0).position;
            HandleInput(position);
        }
#endif
    }

    void CreateGrid()
    {
        if (grid.CreateGrid(stages[currentStage].groundSlots))
        {
            FindPath();
        }
    }

    void FindPath()
    {
        List<Vector2Int> endPoints = new List<Vector2Int>();
        foreach (Vector2Int outSlot in stages[currentStage].outSlots)
        {
            Slot currentSlot = stages[currentStage].groundSlots[outSlot.y].slots[outSlot.x];
            if (currentSlot != null && currentSlot.Mob == null && currentSlot.Tunnel == null)
            {
                endPoints.Add(outSlot);
            }
        }

        foreach (Mob mob in stages[currentStage].mobs)
        {
            List<Slot> shortestPath = null;
            foreach (Vector2Int endPoint in endPoints)
            {
                List<Slot> path = Pathfinding.FindPath(mob.Pos, endPoint, grid);
                if (path == null)
                    continue;
                if (shortestPath == null || shortestPath.Count > path.Count)
                    shortestPath = path;
            }
            mob.Path = shortestPath;
        }
    }

    void ActivateTunnel(Vector2Int mobPos)
    {
        List<Column> columns = stages[currentStage].groundSlots;
        if (mobPos.x > 0 && columns[mobPos.y].slots[mobPos.x - 1] != null)
        {
            ColorType color = ColorType.NONE;
            if (columns[mobPos.y].slots[mobPos.x - 1].Tunnel != null && columns[mobPos.y].slots[mobPos.x - 1].Tunnel.GetMob(out color))
            {
                GameObject clone = Instantiate(mobPrefab, columns[mobPos.y].slots[mobPos.x - 1].transform.position, Quaternion.identity, stages[currentStage].mobContainer);
                clone.GetComponent<Mob>().SetUpAfterSpawn(color);
                clone.GetComponent<Mob>().Pos = mobPos;
                stages[currentStage].mobs.Add(clone.GetComponent<Mob>());   
                clone.transform.localScale = Vector3.zero;
                clone.GetComponent<BoxCollider>().enabled = false;
                clone.transform.DOMove(columns[mobPos.y].slots[mobPos.x].transform.position, 0.15f).SetEase(Ease.OutFlash);
                clone.transform.DOScale(0.2f, 0.25f).SetEase(Ease.OutFlash).OnComplete(() =>
                {
                    columns[mobPos.y].slots[mobPos.x].Mob = clone.GetComponent<Mob>();
                    clone.GetComponent<BoxCollider>().enabled = true;
                });
            }
            else
                grid.grid[mobPos.x, mobPos.y].walkable = true;
        }
        else
            grid.grid[mobPos.x, mobPos.y].walkable = true;
        FindPath();
    }

    public void MoveMob(Vector2Int mobPos, Mob mob)
    {
        mob.Anim.SetBool("move",true);
        stages[currentStage].groundSlots[mobPos.y].slots[mobPos.x].Mob = null;
        stages[currentStage].mobs.Remove(mob);
        mob.MoveToEdge(edgeZ);
        StartCoroutine(Cor_MoveMob(mob));
    }

    IEnumerator Cor_MoveMob(Mob mob)
    {
        yield return new WaitForSeconds(0.5f);
        stages[currentStage].slots[currentSlot].Mob = mob;
        SortMob(currentSlot); ;
        currentSlot++;
    }

    void SortMob(int currentSlot)
    {
        for (int i = currentSlot - 1; i >= 0; i--)
        {
            if (stages[currentStage].slots[i].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType)
            {
                if (i == currentSlot - 1)
                    break;
                Mob tempMob = stages[currentStage].slots[currentSlot].Mob;
                for (int j = currentSlot - 1; j > i; j--)
                {
                    stages[currentStage].slots[j].Mob.Move(new Vector3(stages[currentStage].slots[j + 1].transform.position.x, 0.06f, stages[currentStage].slots[j + 1].transform.position.z));
                    stages[currentStage].slots[j + 1].Mob = stages[currentStage].slots[j].Mob;
                }
                SoundManager.Instance.PlaySound(SoundType.SFX_FOOTSTEP, 0.5f);
                tempMob.Move(new Vector3(stages[currentStage].slots[i+1].transform.position.x, 0.06f, stages[currentStage].slots[i+1].transform.position.z));
                stages[currentStage].slots[i+1].Mob = tempMob;
                StartCoroutine(Cor_Connect(i + 1));
                return;
            }
        }
        SoundManager.Instance.PlaySound(SoundType.SFX_FOOTSTEP, 0.5f);
        stages[currentStage].slots[currentSlot].Mob.Move(new Vector3(stages[currentStage].slots[currentSlot].transform.position.x, 0.06f, stages[currentStage].slots[currentSlot].transform.position.z));
        StartCoroutine(Cor_Connect(currentSlot));
    }

    IEnumerator Cor_Connect(int currentSlot)
    {
        if (currentSlot>=2 && stages[currentStage].slots[currentSlot-1].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType && stages[currentStage].slots[currentSlot - 2].Mob.ColorType == stages[currentStage].slots[currentSlot].Mob.ColorType)
        {
            Mob mob1 = stages[currentStage].slots[currentSlot - 2].Mob, mob2 = stages[currentStage].slots[currentSlot - 1].Mob, mob3 = stages[currentStage].slots[currentSlot].Mob;
            SoundManager.Instance.PlaySound(SoundType.SFX_FOOTSTEP, 0.5f);
            for (int i = currentSlot + 1; i <= this.currentSlot; i++)
            {
                if (stages[currentStage].slots[i].Mob == null)
                    break;
                stages[currentStage].slots[i].Mob.Move(new Vector3(stages[currentStage].slots[i - 3].transform.position.x, 0.06f, stages[currentStage].slots[i - 3].transform.position.z));
                stages[currentStage].slots[i - 3].Mob = stages[currentStage].slots[i].Mob;
                stages[currentStage].slots[i].Mob = null;
            }
            this.currentSlot -= 3;
            mob1.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob2.Disappear();
            yield return new WaitForSeconds(0.1f);
            mob3.Disappear();
        }
        if (this.currentSlot == 7)
        {
            Lose();
        }
    }

    public void Victory()
    {
        currentStage++;
        if (currentStage == stages.Count)
        {
            UIController.Instance.Victory();
            return;
        }
        Camera.main.transform.DOMoveX(stages[currentStage].cameraPosX, 0.5f).SetEase(Ease.OutBack);
        CreateGrid();
    }

    void Lose()
    {
        UIController.Instance.Lose();
    }
}
