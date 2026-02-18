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
        if (Input.GetKeyDown(KeyCode.I) && !isInventoryOpen)
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.I ) && isInventoryOpen)
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
