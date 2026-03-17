using UnityEngine;
[System.Serializable]
public class WeaponData 
{
    public string weaponName;

    [Header("Link to Inventory Item")]
    public ItemObject item;

    public GameObject weaponPrefab;
    public GameObject previewPrefab;

    [Header("Stats")]
    public int attack;
    public int defense;
    public int maxHP;
    public int maxMana;

    [Header("Fireball Skill")]
    public GameObject fireballPrefab;
    public int fireballManaCost;
    public int fireballDamage;
    public float fireballCooldown;

    [Header("Meteor Skill")]
    public GameObject meteorPrefab;
    public int meteorManaCost;
    public int meteorDamage;
    public float meteorCooldown;


}
