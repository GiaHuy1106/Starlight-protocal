using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryObject", menuName = "Inventory System/InventoryObject")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> container = new List<InventorySlot>();
    public void AddItem(ItemObject itemObject, int amount)
    {
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].itemObject == itemObject)
            {
                container[i].AddAmount(amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            container.Add(new InventorySlot(itemObject, amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject itemObject;
    public int amount;
    public InventorySlot(ItemObject itemObject, int amount)
    {
        this.itemObject = itemObject;
        this.amount = amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
