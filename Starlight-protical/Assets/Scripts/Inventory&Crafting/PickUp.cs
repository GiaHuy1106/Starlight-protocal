using UnityEngine;


public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;


        Debug.Log("Picked up object: " + other.name);


        Item item = other.GetComponent<Item>();


        if (item == null)
        {
            Debug.LogError("Item component NOT FOUND on object");
            return;
        }


        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem Instance is NULL");
            return;
        }


        Debug.Log("Trying to add item: " + item.item.itemName + " x" + item.itemAmount);


        // Add vào inventory
        InventorySystem.Instance.Additem(item.item, item.itemAmount);


        // Kiểm tra sau khi add
        int total = InventorySystem.Instance.GetInventoryItemAmount(item.item);


        InventorySystem.Instance.UpdateUI();


        Debug.Log("Inventory now has: " + total + " " + item.item.itemName);


        Destroy(other.gameObject);
    }


}


       

