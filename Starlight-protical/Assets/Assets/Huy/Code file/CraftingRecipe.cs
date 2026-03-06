using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Ingredient
{
    public ItemObject item;
    public int amount;
}

[CreateAssetMenu(fileName = "CraftingRec", menuName = "GameData/CraftingRec"/*, order = 1*/)]
public class CraftingRec : ScriptableObject
{
    public List<Ingredient> ingredients; //Danh sách nguyên liệu cần thiết để chế tạo, có thể chứa nhiều nguyên liệu khác nhau
    public ItemObject resultItem;
    public int amount = 1;
}
