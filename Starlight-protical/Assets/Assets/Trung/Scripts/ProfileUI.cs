using UnityEngine;
using TMPro;
public class ProfileUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    
    void Start()
    {
        playerStats.OnStatChanged += Refresh;
        Refresh();
    }

    // Update is called once per frame
    void Refresh()
    {
        hpText.text   = $"HP: {playerStats.maxHP}";
        manaText.text = $"Mana: {playerStats.maxMana}";
        atkText.text  = $"ATK: {playerStats.attack}";
        defText.text  = $"DEF: {playerStats.defense}";
    }
}
