using Unity.VisualScripting;
using UnityEngine;

public class ProfilePanelToggle : MonoBehaviour
{
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
        profilePanel.SetActive(true);
    }
    public void Close()
    {
        isOpen = false;
        profilePanel.SetActive(false);
    }
}
