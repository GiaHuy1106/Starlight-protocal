using UnityEngine;

public class WeaponSelection : MonoBehaviour
{
    public static WeaponSelection Instance { get; private set;}

    public void Awake()
    {
        Instance = this;
    }

    public void Selection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponManager.Instance.EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponManager.Instance.EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponManager.Instance.EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) WeaponManager.Instance.EquipWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) WeaponManager.Instance.EquipWeapon(4);
    }
}
