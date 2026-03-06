using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public SkillTooltipUI tooltipUI;
    public PlayerSkill playerSkill;
    public PlayerStats playerStats;

    [Header("Info")]
    public string skillTitle;
    [TextArea]
    public string description;
    public bool isSpecialSkill = false; // phân biệt kỹ năng đặc biệt và cơ bản

    public void OnPointerEnter(PointerEventData eventData)
    {
        int damage ;
        int mana ;
        float cooldown ;
        if (isSpecialSkill)
        {
            damage = playerStats.GetSpecialDamage();
            mana = playerSkill.specialManaCost;
            cooldown = playerSkill.specialCooldown;
        }
        else
        {
            damage = playerStats.GetBasicDamage();
            mana = 0;
            cooldown = playerSkill.fireballCooldown;
        }
        tooltipUI.Show(skillTitle, description, damage, mana, cooldown);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.Hide();
    }
}
