using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public PlayerStats playerStats;
    [Header("Weapon Hold Point")]
    public Transform handPoint; // vị trí gắn vũ khí trên tay

    [Header("Preview")]
    public Transform previewPoint;// vị trí preview trong UI
    public Transform previewHandPoint; 

    [Header("Weapons")]
    public WeaponData[] weapons;
    
    GameObject currentPreviewHandWeapon;
    GameObject currentWeapon;
    GameObject currentPreview;
     int currentIndex = 0;
    void Start()
    {
        EquipWeapon(0); // mặc định level 0
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.V)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.B)) EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.N)) EquipWeapon(3);
        if (Input.GetKeyDown(KeyCode.M)) EquipWeapon(4);
    }
    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        currentIndex = index;

        WeaponData weapon = weapons[index];

        // xoá vũ khí cũ
        if (currentWeapon != null)
            Destroy(currentWeapon);
        // xóa vũ khí cũ trong profile panel
        if (currentPreview != null)
            Destroy(currentPreview);
        // xóa vũ khí cũ của player trong profile panel
        if (currentPreviewHandWeapon != null)
        Destroy(currentPreviewHandWeapon);

        // spawn vũ khí trên tay
        currentWeapon = Instantiate(weapon.weaponPrefab, handPoint);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // spawn preview
        currentPreview = Instantiate(weapon.previewPrefab, previewPoint);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;

        // ⭐ spawn weapon on avatar preview hand
        currentPreviewHandWeapon = Instantiate(weapon.weaponPrefab, previewHandPoint);
        currentPreviewHandWeapon.transform.localPosition = Vector3.zero;
        currentPreviewHandWeapon.transform.localRotation = Quaternion.identity;


        // update stats
        playerStats.attack = weapon.attack;
        playerStats.defense = weapon.defense;

        playerStats.RefreshStats();
    }
}
