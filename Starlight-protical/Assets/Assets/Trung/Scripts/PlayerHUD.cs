using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerHUD : MonoBehaviour
{
    public PlayerStats playerStats;

    public Image hpFill;
    public TextMeshProUGUI hpText;

    public Image manaFill;
    public TextMeshProUGUI manaText;

    public float smoothSpeed = 8f;

    float targetHP;
    float targetMana;
    void Start()
    {
        playerStats.OnStatChanged += RefreshUI;

        targetHP = playerStats.GetHealthPercent();
        targetMana = playerStats.GetManaPercent();

        RefreshUI();
    }
    void Update()
    {
        hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, targetHP, Time.deltaTime * smoothSpeed);
        manaFill.fillAmount = Mathf.Lerp(manaFill.fillAmount, targetMana, Time.deltaTime * smoothSpeed);
    }
    void OnDestroy()
    {
        playerStats.OnStatChanged -= RefreshUI;
    } 
    void RefreshUI()
    {
        targetHP = playerStats.GetHealthPercent();
        targetMana = playerStats.GetManaPercent();
        
        hpText.text = $"{playerStats.CurrentHP} / {playerStats.maxHP}";
        manaText.text = $"{playerStats.CurrentMana} / {playerStats.maxMana}";
    }
}
