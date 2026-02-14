using UnityEngine;

public enum ItemType
{
    Currency,
    Consumable,
    Gem,
    Skull,
    Health 
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "ItemObject", menuName = "GameData/Item")]
public class ItemObject : ScriptableObject
{
    [Header("Identity")]
    //public string itemId;
    public string itemName;
    [TextArea] public string description;

    [Header("Visual")]
    public Sprite icon;
    public GameObject worldPrefab;

    [Header("Category")] //loại item
    public ItemType itemType;
    public ItemRarity rarity;

    [Header("Stack & Economy")]
    public bool stackable = true;
    public int maxStack = 99;
    public int sellPrice;

    [Header("Combat / Effect")] //các chỉ số cộng thêm khi sử dụng item
    public int attackBonus;
    public int healAmount;

    [Header("Drop Settings")] //tỉ lệ rơi
    [Range(0f, 1f)]
    public float dropChance = 0.25f; // 0.25 = 25%
}
