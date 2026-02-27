using Unity.VisualScripting;
using UnityEngine;

public class ProfilePanelToggle : MonoBehaviour
{public PlayerInput playerInput;

    [Header("Panel cần đóng/mở")]
    public GameObject profilePanel;

    bool isOpen = false;

    void Start()
    {
        profilePanel.SetActive(false);
    }

    public void TogglePanel()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        isOpen = true;

        playerInput.SetUILock(true);   // ✅ sửa

        profilePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Close()
    {
        isOpen = false;

        profilePanel.SetActive(false);

        playerInput.SetUILock(false);  // ✅ sửa

        Time.timeScale = 1f;
    }
}
