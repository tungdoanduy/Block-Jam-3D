using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Tunnel : MonoBehaviour
{
    [SerializeField] List<ColorType> mobs = new List<ColorType>();
    [SerializeField] TMP_Text mobLeftText;
    [SerializeField] Vector2Int pos;
    public Vector2Int Pos
    {
        get => pos;
        set => pos = value;
    }
    private void Start()
    {
        mobLeftText.text = mobs.Count.ToString();
    }
    public bool GetMob(out ColorType mob)
    {
        if (mobs.Count > 0)
        {
            mob = mobs[0];
            mobs.RemoveAt(0);
            mobLeftText.text = mobs.Count.ToString();
            return true;
        }
        mob = ColorType.NONE;
        return false;
    }
}
