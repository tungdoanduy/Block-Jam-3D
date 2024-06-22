using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using NaughtyAttributes;

[System.Serializable]
public class LevelDict : SerializableDictionaryBase<SceneType,string> { }

[CreateAssetMenu(menuName = "Scriptable Object/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] LevelDict levelDict;
    public LevelDict LevelDict => levelDict;

    [SerializeField,ReadOnly] int levelUnlocked = 1;
    public int LevelUnlocked
    {
        get => levelUnlocked;
        set 
        {
            if (value > levelUnlocked)
                levelUnlocked = value;
        }
    }

    public void ForceSetLevelUnlocked(int levelUnlocked)
    {
        this.levelUnlocked = levelUnlocked;
    }
}
