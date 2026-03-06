using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem instance { get; private set; }


    public List<CraftingRec> craftingRecipes; //Danh sách các công thức chế tạo, có thể được khởi tạo từ các ScriptableObject CraftingRec
    public Transform craftingGrid;
    public GameObject craftingBTN;
    public GameObject itemNeededUIPrefab;

    public void Awake()
    {
        instance = this;
    }

    public void populateCraftingGrid()
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
        InventorySystem.Instance.Additem(recipe.resultItem, recipe.amount);

        populateCraftingGrid(); //Cập nhật lại crafting grid sau khi chế tạo để cập nhật trạng thái có thể chế tạo của các công thức khác
    }

    private void ConsumIngredients(CraftingRec recipe)
    {
        foreach ( Ingredient ingredient in recipe.ingredients)
        {
            int remaining = ingredient.amount;

            foreach (Slots slot in InventorySystem.Instance.allSlots)
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

            foreach (Slots slot in InventorySystem.Instance.allSlots)
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

    public void updateCraftingRecipes(List<CraftingRec> newRecipes)
    {
        craftingRecipes = newRecipes; //Cập nhật danh sách công thức chế tạo mới
        populateCraftingGrid(); //Cập nhật lại crafting grid để hiển thị các công thức mới
    }
}
