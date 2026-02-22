using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //Test item
    // public ItemObject BigPotion;
    // public ItemObject SmallPotion;

    //Inventory
    public GameObject inventorySlotsParent; //Panel chứa các slot trong inventory
    public Image IconDrag;
    
    //Pickup item
    public float pickupRange = 3f; //Khoảng cách tối đa để nhặt item, có thể điều chỉnh trong Inspector
    private Item lookedAtItem;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lookedAtItemRenderer = null;

    //Miêu tả item
    public GameObject itemDesciptionParent;
    //public GameObject worldItemInfoPrefab;
    public Image itemDescriptionIcon;
    public TextMeshProUGUI itemDescriptionName;
    public TextMeshProUGUI itemDescription;

    //Crafting
    public List<CraftingRec> craftingRecipes; //Danh sách các công thức chế tạo, có thể được khởi tạo từ các ScriptableObject CraftingRec
    public Transform craftingGrid;
    public GameObject craftingBTN;
    public GameObject itemNeededUIPrefab;

    //Danh sách slot
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
        //Test item 
        //testItem();
        
        DetecLookAtItem();
        Pickup();

        StartDrag();
        UpdateItemDragPos();
        EndDrag();

        UpdateItemDescrip();
    }

    /*private void testItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Additem(SmallPotion, 1); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Additem(BigPotion, 1); 
        }
    }
    */

    public void Additem (ItemObject ItemToAdd, int amount)
    {
        int remainingAmount = amount; //Số lượng còn lại cần thêm vào inventory, ban đầu bằng với số lượng muốn thêm

        //Đầu tiên, cố gắng xếp chồng item vào các slot đã có cùng loại item nếu có thể
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

                    if(remainingAmount <= 0)
                    {
                        populateCraftingGrid();
                        return; //Nếu đã thêm đủ số lượng cần thiết vào inventory thì dừng việc tìm kiếm và thêm vào các slot khác
                    }
                }
            }
        }

        //Nếu vẫn còn thêm item khi hết số lượng, thêm vào slot trống
        foreach (Slots slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(ItemToAdd.maxStack, remainingAmount); //Số lượng có thể đặt vào slot này, không vượt quá maxStack và remainingAmount
                slot.SetItem(ItemToAdd, amountToPlace); //Đặt item vào slot với số lượng đang có
                remainingAmount -= amountToPlace; //Cập nhật số lượng còn lại cần thêm vào inventory

                if(remainingAmount <= 0)
                {
                    populateCraftingGrid();
                    return; //Nếu đã thêm đủ số lượng cần thiết vào inventory thì dừng việc tìm kiếm và thêm vào các slot khác
                }
            }
        }
        
        //Nếu hết slot trông thì thông báo hết chỗ
        if (remainingAmount > 0)
        {
            Debug.LogWarning("Not enough space in inventory to add all items. Remaining amount: " + remainingAmount + " of " + ItemToAdd.name);
        }
        populateCraftingGrid();
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

    private void Pickup()
    {
        if (lookedAtItemRenderer != null && Input.GetMouseButtonDown(0))
        {
            Item item = lookedAtItemRenderer.GetComponent<Item>();
            if (item != null)
            {
                Additem(item.item, item.itemAmount); //Thêm item vào inventory với số lượng được chỉ định trong component Item của item đó
                Destroy(item.gameObject); //Xóa item khỏi thế giới sau khi nhặt
            }
        }
    }

    private void DetecLookAtItem()
    {
        if (lookedAtItemRenderer != null) //Nếu đang có item được nhìn thấy trước đó, khôi phục vật liệu gốc của item đó
        {
            lookedAtItemRenderer.material = originalMaterial; //Khôi phục vật liệu gốc của item trước đó nếu có
            lookedAtItemRenderer = null;
            originalMaterial = null;
        }

        Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward); //Tạo một tia từ vị trí của camera theo hướng mà camera đang nhìn
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponent<Item>(); //Kiểm tra xem tia có va chạm với một collider có component Item hay không
            if (item != null)
            {
                Renderer renderer = item.GetComponent<Renderer>(); //Lưu renderer của item đang nhìn thấy để có thể thay đổi vật liệu khi highlight
                if (renderer != null)
                {
                    originalMaterial = renderer.material; //Lưu vật liệu gốc của item để có thể khôi phục sau khi không còn nhìn thấy nữa
                    renderer.material = highlightMaterial; //Thay đổi vật liệu của item thành highlight để làm nổi bật khi nhìn thấy
                    lookedAtItemRenderer = renderer; //Lưu renderer của item đang nhìn thấy để có thể khôi phục sau này nếu cần
                }
            }
        }
    }

    private void UpdateItemDescrip()
    {
        itemDesciptionParent.SetActive(false);
        Slots hovered = GetHoveredSlot();
        if (hovered != null)
        {
            ItemObject item = hovered.GetItem();
            if (item != null)
            {
                itemDesciptionParent.SetActive(true);
                itemDescriptionIcon.sprite = item.icon;
                itemDescriptionName.text = item.itemName;
                itemDescription.text = item.description;
                return;
            }
            itemDesciptionParent.SetActive(false);
        }
    }

    private void populateCraftingGrid()
    {
        //Xóa tất cả các slot con hiện có trong craftingGrid trước khi tạo lại dựa trên công thức chế tạo hiện tại
        for (int i = 0; i < craftingGrid.childCount; i++)
        {
            Destroy(craftingGrid.GetChild(i).gameObject);
        }

        //Dựa trên công thức chế tạo hiện tại, tạo các slot mới trong craftingGrid và đặt item tương ứng vào mỗi slot
        foreach (CraftingRec recipe in craftingRecipes)
        {
            GameObject btnObj = Instantiate(craftingBTN, craftingGrid); //Tạo một slot mới từ prefab craftingBTN và đặt nó làm con của craftingGrid

            Image img = btnObj.transform.GetChild(0).GetComponent<Image>(); //Lấy component Image của con đầu tiên (icon) của slot mới
            img.sprite = recipe.resultItem.icon; //Đặt icon của item kết quả vào slot mới

            Button btn = btnObj.GetComponent<Button>(); //Lấy component Button của slot mới để có thể thêm sự kiện khi nhấn vào slot đó

            btn.interactable = CanCraft(recipe);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CraftItem(recipe)); //Thêm sự kiện khi nhấn vào slot đó, sẽ gọi hàm CraftItem

            foreach (Ingredient ingredient in recipe.ingredients)
            {
                GameObject neededItemUI = Instantiate(itemNeededUIPrefab, btnObj.transform.GetChild(1));
                neededItemUI.GetComponent<Image>().sprite = ingredient.item.icon;
                neededItemUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + ingredient.amount.ToString(); //Hiển thị số lượng nguyên liệu cần thiết trên UI của slot đó
            }
        }
    }

    public void CraftItem(CraftingRec recipe)
    {
        if (!CanCraft(recipe)) return;
        
        ConsumIngredients(recipe);
        Additem(recipe.resultItem, recipe.amount);

        populateCraftingGrid(); //Cập nhật lại crafting grid sau khi chế tạo để cập nhật trạng thái có thể chế tạo của các công thức khác
    }

    private void ConsumIngredients(CraftingRec recipe)
    {
        foreach ( Ingredient ingredient in recipe.ingredients)
        {
            int remaining = ingredient.amount;

            foreach (Slots slot in allSlots)
            {
                if (!slot.HasItem()) continue;
                if (slot.GetItem() != ingredient.item) continue;
                
                int take = Mathf.Min(slot.GetAmount(), remaining);
                slot.SetItem(slot.GetItem(), slot.GetAmount() - take);

                if (slot.GetAmount() <= 0) slot.ClearSlots();

                remaining -= take; //Cập nhật số lượng còn lại cần tiêu thụ sau khi đã lấy từ slot này
                if (remaining <= 0) break; //Nếu đã tiêu thụ đủ số lượng cần thiết thì dừng việc tìm kiếm và tiêu thụ từ các slot khác

            }
        }
    }

    public bool CanCraft(CraftingRec recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            int totalFound = 0;

            foreach (Slots slot in allSlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                {
                    totalFound += slot.GetAmount(); //Tính tổng số lượng của item nguyên liệu hiện có trong tất cả các slot
                }
            }

            if (totalFound < ingredient.amount) return false; //Nếu tổng số lượng của item nguyên liệu hiện có nhỏ hơn số lượng cần thiết thì không thể chế tạo được
        }

        return true; //Nếu đã kiểm tra tất cả nguyên liệu và đều đủ số lượng cần thiết thì có thể chế tạo được
    }
}
