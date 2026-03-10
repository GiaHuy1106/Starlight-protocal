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
    public PlayerSkill playerSkill;
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

        if (currentWeapon != null)
            Destroy(currentWeapon);

        if (currentPreview != null)
            Destroy(currentPreview);

        if (currentPreviewHandWeapon != null)
            Destroy(currentPreviewHandWeapon);

        // weapon on player hand
        currentWeapon = Instantiate(weapon.weaponPrefab, handPoint);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // weapon preview panel
        currentPreview = Instantiate(weapon.previewPrefab, previewPoint);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;

        // weapon on avatar preview hand
        currentPreviewHandWeapon = Instantiate(weapon.weaponPrefab, previewHandPoint);
        currentPreviewHandWeapon.transform.localPosition = Vector3.zero;
        currentPreviewHandWeapon.transform.localRotation = Quaternion.identity;

        // cập nhật wand glow

        WandGlow glow = currentWeapon.GetComponentInChildren<WandGlow>();
        if (playerSkill != null)
        playerSkill.wandGlow = glow;

        // update player stats
        playerStats.SetStats(
            weapon.maxHP,
            weapon.maxMana,
            weapon.attack,
            weapon.defense
        );
        ApplyWeaponSkills(weapon);
    }
    void ApplyWeaponSkills(WeaponData weapon)
    {
        if (playerSkill == null) return;

        playerSkill.fireballPrefab = weapon.fireballPrefab;
        playerSkill.specialPrefab = weapon.meteorPrefab;

        playerSkill.fireballManaCost = weapon.fireballManaCost;
        playerSkill.specialManaCost = weapon.meteorManaCost;

        playerSkill.fireballSkillDamage = weapon.fireballDamage;
        playerSkill.specialSkillDamage = weapon.meteorDamage;

        playerSkill.fireballCooldown = weapon.fireballCooldown;
        playerSkill.specialCooldown = weapon.meteorCooldown;
    }
}
