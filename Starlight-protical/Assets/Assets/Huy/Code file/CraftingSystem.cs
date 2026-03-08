using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }

    [Header("Recipes")]
    public List<CraftingRec> craftingRecipes = new List<CraftingRec>();

    private void Awake()
    {
        Instance = this;
    }

    // CRAFT ITEM
    public void CraftItem(CraftingRec recipe)
    {
        if (recipe == null || !CanCraft(recipe))
            return;

        ConsumeIngredients(recipe);

        InventorySystem.Instance.Additem(recipe.resultItem, recipe.resultAmount);

        // thông báo cho UI item đã được craft
        int index = craftingUI.Instance.GetCurrentIndex();
        craftingUI.Instance.SetCrafted(index);
    }

    // TRỪ NGUYÊN LIỆU
    void ConsumeIngredients(CraftingRec recipe)
    {
        foreach (Ingredient ingredient in recipe.requiredItems)
        {
            int remaining = ingredient.amount;

            foreach (Slots slot in InventorySystem.Instance.inventorySlots)
            {
                if (!slot.HasItem()) continue;
                if (slot.GetItem() != ingredient.item) continue;

                int take = Mathf.Min(slot.GetAmount(), remaining);

                slot.SetItem(slot.GetItem(), slot.GetAmount() - take);

                if (slot.GetAmount() <= 0)
                    slot.ClearSlots();

                remaining -= take;

                if (remaining <= 0)
                    break;
            }
        }
    }

    // KIỂM TRA CÓ ĐỦ NGUYÊN LIỆU KHÔNG
    public bool CanCraft(CraftingRec recipe)
    {
        foreach (Ingredient ingredient in recipe.requiredItems)
        {
            int totalFound = 0;

            foreach (Slots slot in InventorySystem.Instance.inventorySlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                {
                    totalFound += slot.GetAmount();
                }
            }

            if (totalFound < ingredient.amount)
                return false;
        }

        return true;
    }

    // UPDATE DANH SÁCH RECIPE
    public void UpdateCraftingRecipes(List<CraftingRec> newRecipes)
    {
        craftingRecipes = newRecipes;
    }
}