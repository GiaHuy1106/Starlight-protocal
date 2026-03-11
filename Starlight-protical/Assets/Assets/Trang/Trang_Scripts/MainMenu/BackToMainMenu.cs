using UnityEngine;

public class BackToMainMenu : MonoBehaviour
{
    public void OnClickBackToMenu()
    {
        if (MainMenuManager.instance != null)
        {
            Time.timeScale = 1f;
            MainMenuManager.instance.BackMainMenu();
        }
    }
}
