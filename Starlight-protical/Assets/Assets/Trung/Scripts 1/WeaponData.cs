using UnityEngine;
[System.Serializable]
public class WeaponData 
{
    public string weaponName;
    public GameObject weaponPrefab; // model gắn tay player
    public GameObject previewPrefab; // model dùng cho preview
    public int attack;
    public int defense;

}
