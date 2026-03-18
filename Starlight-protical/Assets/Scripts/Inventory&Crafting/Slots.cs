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
        Debug.Log($"[Slots] Awake on {gameObject.name}");

        if (imageIcon == null)
        {
            if (transform.childCount > 0)
            {
                imageIcon = transform.GetChild(0).GetComponent<Image>();
                Debug.Log("[Slots] Auto assign imageIcon from child 0");
            }
            else
            {
                Debug.LogError("[Slots] Không có child để lấy imageIcon");
            }
        }

        if (textAmount == null)
        {
            if (transform.childCount > 1)
            {
                textAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                Debug.Log("[Slots] Auto assign textAmount from child 1");
            }
            else
            {
                Debug.LogError("[Slots] Không có child để lấy textAmount");
            }
        }

        if (imageIcon == null)
            Debug.LogError("[Slots] imageIcon NULL sau Awake");

        if (textAmount == null)
            Debug.LogError("[Slots] textAmount NULL sau Awake");
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

        if (item != null)
            Debug.Log("[Slots] SetItem: " + item.itemName);
        else
            Debug.LogWarning("[Slots] SetItem: item NULL");

        //UpdateSlots(); // 👈 bật lại (rất quan trọng)
    }


    // =========================
    // UPDATE UI
    // =========================
    public void UpdateSlots()
    {
        Debug.Log($"[Slots] UpdateSlots - Item: {(item != null ? item.itemName : "NULL")} | Amount: {amount}");

        if (imageIcon == null)
        {
            Debug.LogError("[Slots] imageIcon NULL -> không thể update UI");
            return;
        }

        if (textAmount == null)
        {
            Debug.LogError("[Slots] textAmount NULL -> không thể update UI");
            return;
        }

        if (item != null)
        {
            imageIcon.enabled = true;

            if (item.icon != null)
            {
                imageIcon.sprite = item.icon;
            }
            else
            {
                Debug.LogWarning($"[Slots] Item {item.itemName} không có icon");
                imageIcon.sprite = null;
            }

            if (amount > 1)
            {
                textAmount.text = amount.ToString();
            }
            else
            {
                textAmount.text = "";
            }
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
