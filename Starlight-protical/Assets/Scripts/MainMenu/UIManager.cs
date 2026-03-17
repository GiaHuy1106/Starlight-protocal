using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TMP_Text titleText;
    private float normalAlpha = 1f;   // trong suốt bình thường
    private float hoverAlpha = 0.5f;  // trong suốt khi hover


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (titleText == null)
            titleText = GetComponent<TMP_Text>();
        SetAlpha(hoverAlpha);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(normalAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(hoverAlpha);
    }

    void SetAlpha(float a)
    {
        Color c = titleText.color;
        c.a = a;
        titleText.color = c;
    }
}
