using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


public class ItemIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
{
    public bool hovering;
    public bool isPress;
    private ItemObject item;
    public Sprite imageIcon;
    private Image iconImage;
    private Vector3 defaultScale;
    [SerializeField] private float pressedScaleMultiplier = 1.1f;
    [SerializeField] private float hoverScaleMultiplier = 1.05f;
    [SerializeField] private float animationSpeed = 12f;


    private void Awake()
    {
        iconImage = GetComponent<Image>();
        defaultScale = transform.localScale;
    }


    public void Update()
    {
        Animation();
    }


    public ItemObject Get3D()
    {
        return item;
    }


    public void Animation()
    {
        if (iconImage == null)
        {
            return;
        }


        if (isPress || hovering)
        {
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.color = Color.gray;
        }


        float scaleMultiplier = 1f;
        if (isPress)
        {
            scaleMultiplier = pressedScaleMultiplier;
        }
        else if (hovering)
        {
            scaleMultiplier = hoverScaleMultiplier;
        }


        Vector3 targetScale = defaultScale * scaleMultiplier;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }


    public void SetPressed(bool pressed)
    {
        isPress = pressed;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        hovering = true;
        if (craftingUI.Instance != null)
        {
            int index = craftingUI.Instance.itemIcons.IndexOf(this);
            if (index >= 0)
            {
                craftingUI.Instance.SelectItem(index);
            }
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        hovering = true;
    }


    public void OnPointerMove(PointerEventData eventData)
    {
        hovering = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}



