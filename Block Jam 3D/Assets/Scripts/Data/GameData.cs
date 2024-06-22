[System.Serializable]
public class GameData
{
    public int levelUnlocked;

    public GameData()
    {
        levelUnlocked = 1;
    }

    public GameData(int levelUnlocked)
    {
        this.levelUnlocked = levelUnlocked;
    }

}
