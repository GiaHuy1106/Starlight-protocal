using UnityEngine;

public class WeaponSelection : MonoBehaviour
{
    public static WeaponSelection Instance { get; private set;}

    public GameObject[] weaponIcons; // index = weapon index

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateAllIcons();
    }

    public void Selection()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1)) TryEquip(0);
        // if (Input.GetKeyDown(KeyCode.Alpha2)) TryEquip(1);
        // if (Input.GetKeyDown(KeyCode.Alpha3)) TryEquip(2);
        // if (Input.GetKeyDown(KeyCode.Alpha4)) TryEquip(3);
        // if (Input.GetKeyDown(KeyCode.Alpha5)) TryEquip(4);
    }

    public void TryEquip(int index)
    {
        if (!WeaponManager.Instance.IsWeaponUnlocked(index))
        {
            Debug.Log("Chưa unlock nên không chọn được: " + index);
            return;
        }

        WeaponManager.Instance.EquipWeapon(index);
    }

    public void UpdateAllIcons()
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            bool isUnlocked = WeaponManager.Instance.IsWeaponUnlocked(i);

            weaponIcons[i].SetActive(isUnlocked);

            Debug.Log($"Element {i} active: {isUnlocked}");
        }
    }

    public void UpdateSingleIcon(int index)
    {
        if (index < 0 || index >= weaponIcons.Length) return;

        weaponIcons[index].SetActive(true);
    }
}