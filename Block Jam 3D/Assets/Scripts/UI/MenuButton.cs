using UnityEngine;
using UnityEngine.UI;

public class MenuButton : CustomButton
{
    [SerializeField] bool locked = true;
    [SerializeField] GameObject lockImg;

    private void Start()
    {
        if (locked)
        {
            foreach (Image part in parts)
            {
                part.color = new Color(part.color.r - 0.3f, part.color.g - 0.3f, part.color.b - 0.3f, 1);
            }
            text.color = new Color(text.color.r - 0.3f, text.color.g - 0.3f, text.color.b - 0.3f, 1);
            GetComponent<Image>().raycastTarget = false;
            lockImg.SetActive(true);
        }
    }

    public void Unlocked()
    {
        if (!locked)
            return;
        locked = false;
        foreach (Image part in parts)
        {
            part.color = new Color(part.color.r + 0.3f, part.color.g + 0.3f, part.color.b + 0.3f, 1);
        }
        text.color = new Color(text.color.r + 0.3f, text.color.g + 0.3f, text.color.b + 0.3f, 1);
        GetComponent<Image>().raycastTarget = true;
        lockImg.SetActive(false);
    }

    public void LoadLevel(string sceneName)
    {
        UIMenu.Instance.LoadLevel(sceneName);
    }

    public void Quit()
    {
        print("Quit game");
        Application.Quit();
    }
}
