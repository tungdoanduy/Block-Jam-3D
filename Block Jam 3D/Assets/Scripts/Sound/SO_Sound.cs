using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/SO Sound", fileName = "SO Sound")]
public class SO_Sound : ScriptableObject, IComparable
{
    public SoundType soundType;
    public AudioClip clip;

    int IComparable.CompareTo(object obj)
    {
        if (obj == null) return 1;

        SO_Sound other = (SO_Sound)obj;

        if (this.soundType < other.soundType)
            return -1;
        else if (this.soundType > other.soundType)
            return 1;
        else
            return 0;
    }
}
