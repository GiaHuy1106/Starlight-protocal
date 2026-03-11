using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }

    public WeaponManager weaponManager;

    [Header("Recipes")]
    public List<CraftingRec> craftingRecipes = new List<CraftingRec>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // =========================
    // CRAFT ITEM
    // =========================
    // public void CraftItem(CraftingRec recipe)
    // {
    //     if (recipe == null)
    //     {
    //         Debug.LogWarning("Recipe is null");
    //         return;
    //     }

    //     if (!CanCraft(recipe))
    //     {
    //         Debug.Log("Không đủ nguyên liệu");
    //         return;
    //     }

    //     // trừ nguyên liệu
    //     ConsumeIngredients(recipe);

    //     // thêm item kết quả
    //     InventorySystem.Instance.Additem(recipe.resultItem, recipe.resultAmount);

    //     // cập nhật inventory UI
    //     InventorySystem.Instance.UpdateUI();

    //     // cập nhật crafting UI
    //     if (craftingUI.Instance != null)
    //     {
    //         int index = craftingUI.Instance.GetCurrentIndex();
    //         craftingUI.Instance.SetCrafted(index);
    //     }
    // }

    public void CraftItem(CraftingRec recipe)
    {
        Debug.Log("CraftItem called");

        if (recipe == null)
        {
            Debug.LogWarning("Recipe is null");
            return;
        }

        if (!CanCraft(recipe))
        {
            Debug.Log("Không đủ nguyên liệu");
            return;
        }

        Debug.Log("Crafting: " + recipe.resultItem.name);

        ConsumeIngredients(recipe);

        InventorySystem.Instance.Additem(recipe.resultItem, recipe.resultAmount);

        InventorySystem.Instance.UpdateUI();

        if (weaponManager != null)
        {
            Debug.Log("Checking if crafted item is weapon");

            int weaponIndex = weaponManager.GetWeaponIndexByItem(recipe.resultItem);

            if (weaponIndex != -1)
            {
                Debug.Log("Auto equipping crafted weapon");
                weaponManager.EquipWeapon(weaponIndex);
            }
            else
            {
                Debug.Log("Crafted item is not weapon");
            }
        }
        else
        {
            Debug.LogWarning("WeaponManager not found");
        }

        if (craftingUI.Instance != null)
        {
            int index = craftingUI.Instance.GetCurrentIndex();
            craftingUI.Instance.SetCrafted(index);
        }
    }

    // =========================
    // TRỪ NGUYÊN LIỆU
    // =========================
    void ConsumeIngredients(CraftingRec recipe)
    {
        foreach (Ingredient ingredient in recipe.requiredItems)
        {
            int remaining = ingredient.amount;

            foreach (Slots slot in InventorySystem.Instance.inventorySlots)
            {
                if (!slot.HasItem())
                    continue;

                if (slot.GetItem() != ingredient.item)
                    continue;

                int take = Mathf.Min(slot.GetAmount(), remaining);

                slot.SetItem(slot.GetItem(), slot.GetAmount() - take);

                // if (slot.GetAmount() <= 0)
                //     slot.ClearSlots();

                remaining -= take;

                if (remaining <= 0)
                    break;
            }
        }
    }

    // =========================
    // KIỂM TRA NGUYÊN LIỆU
    // =========================
    public bool CanCraft(CraftingRec recipe)
    {
        if (recipe == null)
            return false;

        foreach (Ingredient ingredient in recipe.requiredItems)
        {
            int totalFound = 0;

            foreach (Slots slot in InventorySystem.Instance.inventorySlots)
            {
                if (!slot.HasItem())
                    continue;

                if (slot.GetItem() == ingredient.item)
                    totalFound += slot.GetAmount();
            }

            if (totalFound < ingredient.amount)
                return false;
        }

        return true;
    }

    // =========================
    // UPDATE RECIPE LIST
    // =========================
    public void UpdateCraftingRecipes(List<CraftingRec> newRecipes)
    {
        craftingRecipes = newRecipes;

        if (craftingUI.Instance != null)
            craftingUI.Instance.RefreshUI();
    }
}