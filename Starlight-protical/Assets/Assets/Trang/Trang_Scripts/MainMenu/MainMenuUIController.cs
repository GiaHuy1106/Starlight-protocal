using UnityEngine;


public class MainMenuUIController : MonoBehaviour
{
    public void OnClickPlayGame()
    {
        if (MainMenuManager.instance != null)
        {
            MainMenuManager.instance.PlayNew();
        }
    }

    public void OnClickQuitGame()
    {
        if (MainMenuManager.instance != null)
        {
            MainMenuManager.instance.QuitGame();
        }
    }
}
