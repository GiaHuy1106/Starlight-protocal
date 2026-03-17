using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject TutPanel;
    private void Start()
    {
        if (TutPanel != null)
        {
            TutPanel.SetActive(false);
        }
    }

    public void ToggelActive()
    {
        if (TutPanel != null)
        {
            TutPanel.SetActive(!TutPanel.activeSelf);
            if (TutPanel.activeSelf)
            {
                MainMenuManager.instance.PauseGame();
            }
            else
            {
                MainMenuManager.instance.ResumeGame();
            }
        }
    }

}
