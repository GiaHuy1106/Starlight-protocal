using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using System;
//* Quản lý chỉ số nhân vật như HP, Mana, Atk, v.v.
//*
//*Cung cấp các phương thức để tính sát thương kỹ năng dựa trên chỉ số nhân vật

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public int maxMana = 200;
    public int attack = 20;
    public int defense = 20;
    [Header("Skill Scaling")]
    public float basicMultiplier = 1.0f; // Hệ số nhân sát thương kỹ năng cơ bản
    public float specialMultiplier = 2.5f; // Hệ số nhân sát thương kỹ năng đặc biệt
    public int CurrentHP { get; private set; }
    public int CurrentMana { get; private set; }

    public event Action OnStatChanged; // Sự kiện để thông báo khi chỉ số thay đổi
    public event Action<int> OnDamaged; // Sự kiện để thông báo khi nhận sát thương
    public event Action OnDead;
    void Awake()
    {
        CurrentHP = maxHP;
        CurrentMana = maxMana;
        OnStatChanged?.Invoke();
    }
    //damage tính theo Atk
    public int GetBasicDamage()
    {
        // Tính sát thương kỹ năng cơ bản dựa trên chỉ số tấn công và hệ số nhân
        return Mathf.RoundToInt(attack * basicMultiplier);
    }
    public int GetSpecialDamage()
    {
        // Tính sát thương kỹ năng đặc biệt dựa trên chỉ số tấn công và hệ số nhân
        return Mathf.RoundToInt(attack * specialMultiplier);
    }
    // Trả về phần trăm HP hiện tại
    public float GetHealthPercent()
    {
        return (float)CurrentHP / maxHP;
    }
    // Trả về phần trăm Mana hiện tại
    public float GetManaPercent()
    {
        return (float)CurrentMana / maxMana;
    }
    public void UseMana(int mana)
    {
        CurrentMana -= mana;
        CurrentMana = Mathf.Max(0, CurrentMana);
        OnStatChanged?.Invoke();
    }
    public void Heal(int value)
    {
        CurrentHP += value;
        CurrentHP = Mathf.Min(maxHP, CurrentHP);
        OnStatChanged?.Invoke();
    }
    // hàm
    public void TakeDamage(int dmg, bool triggerHurt = true)
    {
        CurrentHP -= dmg;
        CurrentHP = Mathf.Max(0, CurrentHP);
        if (triggerHurt)
            OnDamaged?.Invoke(dmg);

        OnStatChanged?.Invoke();

        if (CurrentHP <= 0)
        {
            OnDead?.Invoke();
        }
    }
    public void RefreshStats()
    {
        OnStatChanged?.Invoke();
    }
    public void SetStats(int hp, int mana, int atk, int def)
    {
        maxHP = hp;
        maxMana = mana;
        attack = atk;
        defense = def;

        CurrentHP = maxHP;
        CurrentMana = maxMana;

        RefreshStats();
    }
}