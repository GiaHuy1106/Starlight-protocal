using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Slots : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
{
    public bool hovering;


    private ItemObject item;
    private int amount;


    [Header("UI")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private TextMeshProUGUI textAmount;


    private void Awake()
    {
        // Nếu chưa gán trong Inspector thì tự lấy
        if (imageIcon == null)
            imageIcon = transform.GetChild(0).GetComponent<Image>();


        if (textAmount == null)
            textAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }


    public ItemObject GetItem()
    {
        return item;
    }


    public int GetAmount()
    {
        return amount;
    }


    // =========================
    // SET ITEM
    // =========================
    public void SetItem(ItemObject newItem, int newAmount = 1)
    {
        item = newItem;
        amount = newAmount;


        //UpdateSlots();


        Debug.Log("Slot received item: " + item.itemName);
    }


    // =========================
    // UPDATE UI
    // =========================
    public void UpdateSlots()
    {
        if (item != null)
        {
            imageIcon.enabled = true;


            if (item.icon != null)
                imageIcon.sprite = item.icon;


            if (amount > 1)
                textAmount.text = amount.ToString();
            else
                textAmount.text = "";
        }
        else
        {
            imageIcon.enabled = false;
            imageIcon.sprite = null;
            textAmount.text = "";
        }
    }


    // =========================
    // ADD AMOUNT
    // =========================
    public int AddAmount(int amountToAdd)
    {
        if (item == null)
            return 0;


        amount += amountToAdd;


        UpdateSlots();
        return amount;
    }


    // =========================
    // REMOVE AMOUNT
    // =========================
    public int RemoveAmount(int amountToRemove)
    {
        amount -= amountToRemove;


        // if (amount <= 0)
        // {
        //     ClearSlots();
        //     return 0;
        // }


        UpdateSlots();
        return amount;
    }


    // =========================
    // CLEAR SLOT
    // =========================
    public void ClearSlots()
    {
        item = null;
        amount = 0;


        imageIcon.enabled = false;
        imageIcon.sprite = null;
        textAmount.text = "";
    }


    // =========================
    // CHECK ITEM
    // =========================
    public bool HasItem()
    {
        return item != null && amount > 0;
    }


    public void UpdateUI()
    {
        UpdateSlots();
    }


    // =========================
    // MOUSE EVENTS
    // =========================
    public void OnPointerDown(PointerEventData eventData)
    {
        hovering = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        hovering = false;
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
