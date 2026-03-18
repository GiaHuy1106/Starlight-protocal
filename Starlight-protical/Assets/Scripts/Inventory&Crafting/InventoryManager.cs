using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //inventory menu UI
    public GameObject inventoryMenu;
    private bool isInventoryOpen = false;

    //crafting menu UI
    public GameObject craftingMenu;
    private bool isCraftingMenuOpen = false;

    //player id panel
    public GameObject playerID;
    private bool isPanelOpen = false;


    void Start()
    {
        inventoryMenu.SetActive(false);
        craftingMenu.SetActive(false);
        playerID.SetActive(false);
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
            CloseInventory();
        }

        if (Input.GetKeyDown(KeyCode.T) && !isPanelOpen)
        {
            CloseInventory();
            CloseInventory();
            playerID.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.T) && isPanelOpen)
        {
            playerID.SetActive(false);
        }
    }

    public void OpenInventory()
    {
        inventoryMenu.SetActive(true);
        isInventoryOpen = true;
        CloseCraftingMenuVisual();
        //gamePaused.SetPauseState(true);
    }

    public void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        isInventoryOpen = false;
        CloseCraftingMenuVisual();
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
        CloseCraftingMenuVisual();
        isCraftingMenuOpen = false;
        inventoryMenu.SetActive(false);
        //gamePaused.SetPauseState(false);
    }

    private void CloseCraftingMenuVisual()
    {
        if (craftingMenu == null || !craftingMenu.activeSelf)
        {
            return;
        }

        craftingUI craftingUiComponent = craftingMenu.GetComponent<craftingUI>();
        if (craftingUiComponent != null)
        {
            craftingUiComponent.CloseWithAnimation();
            return;
        }

        craftingMenu.SetActive(false);
    }
}
