using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SkillTooltipUI : MonoBehaviour
{
    public GameObject root;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI cooldownText;
    void Start()
    {
        root.SetActive(false);
    }
    void Update()
    {   
        // luôn di chuyển theo chuột
        transform.position = Input.mousePosition + new Vector3(20f, -20f, 0f);
    }
   public void Show(string title, string desc, int damage, int mana, float cooldown)
    {
        root.SetActive(true);

        titleText.text = title;
        descriptionText.text = desc;
        damageText.text = $"Damage: {damage}";
        manaText.text = $"Mana Cost: {mana}";
        cooldownText.text = $"Cooldown: {cooldown} s";
    }
    public void Hide()
    {
        root.SetActive(false);
    }

}
