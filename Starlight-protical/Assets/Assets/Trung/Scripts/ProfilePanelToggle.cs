using Unity.VisualScripting;
using UnityEngine;

public class ProfilePanelToggle : MonoBehaviour
{
    public PlayerInput playerInput;
    [Header("Panel cần đóng/mở")]
    public GameObject profilePanel;
    
    bool isOpen = false;
    
    void Start()
    {
        profilePanel.SetActive(false); // ẩn lúc đầu
    }

   public void TogglePanlel()
    {
        isOpen = !isOpen;
        profilePanel.SetActive(isOpen);
    }
    public void Open()
    {
        isOpen = true;
        playerInput.SetInputLock(true);
        profilePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game
        

    }
    public void Close()
    {
        isOpen = false;
        profilePanel.SetActive(false);

        playerInput.SetInputLock(false);
        Time.timeScale = 1f; // chạy game
    }
}
