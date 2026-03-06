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
        hpText.text = playerStats.maxHP.ToString();
        manaText.text = playerStats.maxMana.ToString();
        atkText.text = playerStats.attack.ToString();
        defText.text = playerStats.defense.ToString();
    }
}
