using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Ingredient
{
    public ItemObject item;
    public int amount;
}

public enum type
{
    WandLVL1,
    WandLVL2,
    WandLVL3,
    WandLVL4,
    BigBLuePotion,
    BigGreenPOtion,
    BigRedPOtion,
}

[CreateAssetMenu(fileName = "CraftingRec", menuName = "GameData/CraftingRec")]
public class CraftingRec : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;
    [TextArea] public string description;
    public type upgradeType;

    [Header("Upgrade Data")]
    public ItemObject currentItem;
    public List<Ingredient> requiredItems = new List<Ingredient>();
    public ItemObject resultItem;
    public int resultAmount = 1;

    [Header("Extra Cost")]
    public int goldCost;

    public bool IsValid()
    {
        return currentItem != null
            && resultItem != null
            && resultAmount > 0
            && requiredItems != null
            && requiredItems.Count > 0;
    }

    public int GetRequiredAmount(ItemObject item)
    {
        if (item == null || requiredItems == null) return 0;

        foreach (Ingredient ingredient in requiredItems)
        {
            if (ingredient.item == item)
            {
                return ingredient.amount;
            }
        }

        return 0;
    }
}
