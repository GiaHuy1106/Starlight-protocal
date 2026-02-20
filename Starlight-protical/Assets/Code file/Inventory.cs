using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ItemObject BigPotion;
    public ItemObject SmallPotion;

    public GameObject inventorySlotsParent; //Panel chứa các slot trong inventory
    public Image IconDrag;

    private List<Slots> inventorySlots = new List<Slots>(); //Danh sách các slot trong inventory, sẽ được khởi tạo từ các slot con của inventorySlotsPanel
    private List<Slots> allSlots = new List<Slots>(); //Danh sách tất cả các slot trong game, bao gồm cả các slot trong inventory và các slot khác (như slot của nhân vật, slot của cửa hàng, v.v.)

    private Slots draggedSlot = null; //Biến tạm để lưu slot đang được kéo thả, sẽ được sử dụng trong các sự kiện kéo thả để xác định slot nguồn và slot đích
    private bool isDragging = false; //Kiểm tra xem có đang kéo item hay không

    void Awake()
    {
        inventorySlots.AddRange(inventorySlotsParent.GetComponentsInChildren<Slots>()); //Lấy tất cả các slot con của inventorySlotsPanel và thêm vào danh sách inventorySlots
        allSlots.AddRange(inventorySlots); //Thêm tất cả các slot trong inventory vào danh sách allSlots
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Additem(BigPotion, 3);          
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Additem(SmallPotion, 2);
        }

        StartDrag();
        UpdateItemDragPos();
        EndDrag();
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
                remainingAmount -= amountToPlace; //Cập nhật số lượng còn lại cần thêm vào inventory

                if (remainingAmount <= 0) return;
            }
        }

        if (remainingAmount > 0)
        {
            Debug.LogWarning("Not enough space in inventory to add all items. Remaining amount: " + remainingAmount + " of " + ItemToAdd.name);
        }
    }

    public void StartDrag() //Bắt đầu kéo item khi nhấn chuột trái vào slot có item, sẽ được gọi trong sự kiện OnMouseDown của slot
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slots hovered = GetHoveredSlot();;

            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;

                // Hiển thị icon của item đang kéo theo con trỏ chuột
                IconDrag.sprite = hovered.GetItem().icon;
                IconDrag.color = new Color (1,1,1,0.5f); //Đặt màu trắng với độ trong suốt 50% để làm nổi bật icon khi kéo
                IconDrag.enabled = true;
            }
        }
    }

    public void EndDrag()
    {
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Slots hovered = GetHoveredSlot();

            if (hovered != null)
            {
                HandleDrag(draggedSlot, hovered); //Xử lý logic khi thả item từ slot nguồn sang slot đích
            }

            draggedSlot = null;
            isDragging = false;
            IconDrag.enabled = false; //Ẩn icon khi kết thúc kéo
        }
    }

    public Slots GetHoveredSlot() //Lấy slot đang được hover bởi con trỏ chuột, sẽ được sử dụng trong các sự kiện kéo thả để xác định slot nguồn và slot đích
    {
        foreach (Slots slot in allSlots)
        {
            if (slot.hovering)
            {
                return slot;
            }
        }
        return null;
    }

    public void HandleDrag(Slots fromOldSlots, Slots toNewSlots) //Xử lý logic khi thả item từ slot nguồn sang slot đích, sẽ được gọi trong sự kiện OnMouseUp của slot
    {
        if (fromOldSlots == toNewSlots) return; //Nếu thả vào cùng một slot thì không làm gì

        //Stacking
        if (toNewSlots.HasItem() && toNewSlots.GetItem() == fromOldSlots.GetItem())
        {
            int maxStacking = toNewSlots.GetItem().maxStack; //Số lượng tối đa có thể xếp chồng của item trong slot đích
            int space = maxStacking - toNewSlots.GetAmount(); //Số lượng còn trống trong slot đích để xếp chồng

            if (space > 0)
            {
                int move = Mathf.Min(space, fromOldSlots.GetAmount());

                toNewSlots.SetItem(toNewSlots.GetItem(), toNewSlots.GetAmount() + move);
                fromOldSlots.SetItem(fromOldSlots.GetItem(), fromOldSlots.GetAmount() - move);

                if (fromOldSlots.GetAmount() <= 0) fromOldSlots.ClearSlots();

                return;
            }
        }

        //different item
        if (toNewSlots.HasItem())
        {
            ItemObject tempItem = toNewSlots.GetItem();
            int tempAmount = toNewSlots.GetAmount();

            toNewSlots.SetItem(fromOldSlots.GetItem(), fromOldSlots.GetAmount()); //Đặt item từ slot gốc sang slot mới
            fromOldSlots.SetItem(tempItem, tempAmount);
            return;
        }

        //empty slot
        toNewSlots.SetItem(fromOldSlots.GetItem(), fromOldSlots.GetAmount()); //Đặt item từ slot gốc sang slot mới
        fromOldSlots.ClearSlots(); //Xóa item khỏi slot gốc
    }

    public void UpdateItemDragPos()
    {
        if (isDragging)
        {
            IconDrag.transform.position = Input.mousePosition; //Cập nhật vị trí của icon theo con trỏ chuột khi đang kéo
        }
    }
}
