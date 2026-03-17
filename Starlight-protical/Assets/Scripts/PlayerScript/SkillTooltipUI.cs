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
    RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        root.SetActive(false);
    }
    void Update()
    {   
        if (!root.activeSelf) return;

        Vector2 pos = Input.mousePosition + new Vector3(20f, -20f);

        Vector2 size = rect.sizeDelta;

        float minX = 0;
        float maxX = Screen.width - size.x;

        float minY = size.y;
        float maxY = Screen.height;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
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
