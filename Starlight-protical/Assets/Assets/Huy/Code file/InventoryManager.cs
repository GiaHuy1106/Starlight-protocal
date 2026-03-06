using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //inventory menu UI
    public GameObject inventoryMenu;
    private bool isInventoryOpen = false;

    //crafting menu UI
    public GameObject craftingMenu;
    private bool isCraftingMenuOpen = false;


    void Start()
    {
        inventoryMenu.SetActive(false);
        craftingMenu.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.I) && !isCraftingMenuOpen)
        {
            OpenCraftingMenu();
            //Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked; //Mở khóa con trỏ chuột khi mở crafting menu, khóa lại khi đóng crafting menu
        }
        else if (Input.GetKeyDown(KeyCode.I) && isCraftingMenuOpen)
        {
            CloseCraftingMenu();
        }
    }

    public void OpenInventory()
    {
        inventoryMenu.SetActive(true);
        isInventoryOpen = true;
        craftingMenu.SetActive(false);
        //gamePaused.SetPauseState(true);
    }

    public void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        isInventoryOpen = false;
        craftingMenu.SetActive(false);
        //gamePaused.SetPauseState(false);
    }

    public void OpenCraftingMenu()
    {
        craftingMenu.SetActive(true);
        isCraftingMenuOpen = true;
        inventoryMenu.SetActive(false);
        //gamePaused.SetPauseState(true);
    }

    public void CloseCraftingMenu()
    {
        craftingMenu.SetActive(false);
        isCraftingMenuOpen = false;
        inventoryMenu.SetActive(false);
        //gamePaused.SetPauseState(false);
    }
}
