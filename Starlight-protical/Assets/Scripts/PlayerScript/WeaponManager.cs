using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set;}

    public PlayerStats playerStats;
    [Header("Weapon Hold Point")]
    public Transform handPoint; // vị trí gắn vũ khí trên tay

    [Header("Preview")]
    public Transform previewPoint;// vị trí preview trong UI
    public Transform previewHandPoint; 

    [Header("Weapons")]
    public WeaponData[] weapons;
    public PlayerSkill playerSkill;
    public bool[] unlockedWeapons; // trạng thái mở khóa từng weapon

    GameObject currentPreviewHandWeapon;
    GameObject currentWeapon;
    GameObject currentPreview;
     int currentIndex = 0;
    void Start()
{
    unlockedWeapons = new bool[weapons.Length];

    // mặc định chỉ có weapon đầu tiên
    unlockedWeapons[0] = true;

    EquipWeapon(0);
}

    public void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        WeaponSelection.Instance.Selection();
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        //chặn weapon chưa unlock
        if (!unlockedWeapons[index])
        {
            Debug.Log("Weapon chưa unlock: " + weapons[index].weaponName);
            return;
        }

        currentIndex = index;

        WeaponData weapon = weapons[index];

        // clear weapon cũ
        if (currentWeapon != null)
            Destroy(currentWeapon);

        if (currentPreview != null)
            Destroy(currentPreview);

        if (currentPreviewHandWeapon != null)
            Destroy(currentPreviewHandWeapon);

        // weapon trên tay player
        currentWeapon = Instantiate(weapon.weaponPrefab, handPoint);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // preview UI
        currentPreview = Instantiate(weapon.previewPrefab, previewPoint);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;

        // preview trên avatar
        currentPreviewHandWeapon = Instantiate(weapon.weaponPrefab, previewHandPoint);
        currentPreviewHandWeapon.transform.localPosition = Vector3.zero;
        currentPreviewHandWeapon.transform.localRotation = Quaternion.identity;

        // lấy glow
        WandGlow glow = currentWeapon.GetComponentInChildren<WandGlow>();
        if (playerSkill != null)
            playerSkill.wandGlow = glow;

        // update stats
        playerStats.SetStats(
            weapon.maxHP,
            weapon.maxMana,
            weapon.attack,
            weapon.defense
        );

        ApplyWeaponSkills(weapon);

        Debug.Log("Equipped weapon: " + weapon.weaponName);
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

    public int GetWeaponIndexByItem(ItemObject item)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].item == item)
            {
                return i;
            }

        }
        return -1;
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        if (unlockedWeapons[index])
        {
            Debug.Log("Weapon đã unlock rồi: " + weapons[index].weaponName);
            return;
        }

        unlockedWeapons[index] = true;

        Debug.Log("Đã unlock weapon: " + weapons[index].weaponName);
    }

    public bool IsWeaponUnlocked(int index)
    {
        if (index < 0 || index >= weapons.Length) return false;
        return unlockedWeapons[index];
    }
}
