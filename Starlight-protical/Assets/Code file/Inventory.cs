using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventorySlotsPanel; //Panel chứa các slot trong inventory

    private List<Slots> inventorySlots = new List<Slots>(); //Danh sách các slot trong inventory, sẽ được khởi tạo từ các slot con của inventorySlotsPanel
    private List<Slots> allSlots = new List<Slots>(); //Danh sách tất cả các slot trong game, bao gồm cả các slot trong inventory và các slot khác (như slot của nhân vật, slot của cửa hàng, v.v.)

    void Awake()
    {
        inventorySlots.AddRange(inventorySlotsPanel.GetComponentsInChildren<Slots>()); //Lấy tất cả các slot con của inventorySlotsPanel và thêm vào danh sách inventorySlots
        allSlots.AddRange(inventorySlots); //Thêm tất cả các slot trong inventory vào danh sách allSlots
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Additem (ItemObject ItemToAdd, int amount)
    {
        int remainingAmount = amount; //Số lượng còn lại cần thêm vào inventory, ban đầu bằng với số lượng muốn thêm

        foreach (Slots slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == ItemToAdd)
            {
                int currentAmount = slot.GetAmount(); //Số lượng hiện tại của item trong slot
                int maxStack = ItemToAdd.maxStack; //Số lượng tối đa có thể xếp chồng của item

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToadd = Mathf.Min(spaceLeft, remainingAmount); //Số lượng có thể thêm vào slot này, không vượt quá spaceLeft và remainingAmount

                    slot.SetItem(ItemToAdd, currentAmount + amountToadd); //Cập nhật slot với số lượng mới
                    remainingAmount -= amountToadd; //Cập nhật số lượng còn lại cần thêm vào inventory

                    if(remainingAmount <= 0) return;
                }
            }
        }

        foreach (Slots slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(ItemToAdd.maxStack, remainingAmount); //Số lượng có thể đặt vào slot này, không vượt quá maxStack và remainingAmount
                slot.SetItem(ItemToAdd, amountToPlace); //Đặt item vào slot với số lượng đang có

                if (remainingAmount <= 0) return;
            }
        }

        if (remainingAmount > 0)
        {
            Debug.LogWarning("Not enough space in inventory to add all items. Remaining amount: " + remainingAmount + " of " + ItemToAdd.name);
        }
    }
}
