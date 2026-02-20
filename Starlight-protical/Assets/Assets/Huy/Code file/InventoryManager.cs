using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private bool isInventoryOpen = false;
    void Start()
    {
        inventoryMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isInventoryOpen)
        {
            OpenInventory();
            //Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked; //Mở khóa con trỏ chuột khi mở inventory, khóa lại khi đóng inventory
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && isInventoryOpen)
        {
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        inventoryMenu.SetActive(true);
        isInventoryOpen = true;
        //gamePaused.SetPauseState(true);
    }

    public void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        isInventoryOpen = false;
        //gamePaused.SetPauseState(false);
    }
}
