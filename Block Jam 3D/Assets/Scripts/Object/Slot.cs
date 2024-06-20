using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] Mob mob;
    public Mob Mob
    {
        get => mob;
        set => mob = value;
    }


}
