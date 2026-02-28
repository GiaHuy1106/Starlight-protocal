using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wands : MonoBehaviour
{
    public bool hovering;

    private ItemObject item;
    private int amount;

    private Image imageIcon;
    private TextMeshProUGUI textAmount;

    private void Awake()
    {
        imageIcon = transform.GetChild(0).GetComponent<Image>(); //lấy component Image của con đầu tiên (icon)
        textAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>(); //lấy component TextMeshProUGUI của con thứ hai (số lượng)
    }

    public ItemObject GetItem() //lấy item hiện tại trong slot
    {
        return item;
    }

    public int GetAmount() //lấy số lượng hiện tại của item trong slot
    {
        return amount;
    }

    public void SetItem(ItemObject newItem, int amount = 0) //đặt item mới vào slot, mặc định số lượng là 1 nếu không được chỉ định
    {
        item = newItem;
        this.amount = amount;

        UpdateSlots();
    }

    public void UpdateSlots() //cập nhật hiển thị của slot dựa trên item và số lượng hiện tại
    {
        if (imageIcon == null)
        {
            imageIcon = transform.GetChild(0).GetComponent<Image>(); //lấy component Image của con đầu tiên (icon)
            textAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>(); //lấy component TextMeshProUGUI của con thứ hai (số lượng)
        }

        if (item != null)
        {
            imageIcon.enabled = true; //hiển thị icon nếu có item
            imageIcon.sprite = item.icon; //hiển thị icon của item
            textAmount.text = amount.ToString(); //hiển thị số lượng nếu có item
        }
        else
        {
            imageIcon.enabled = false;
            textAmount.text = "";
        }
    }

    public int Addamount (int amountToAdd) //thêm một số lượng nhất định vào slot, nếu slot đã có item thì cộng thêm số lượng, nếu chưa có item thì đặt item mới và số lượng
    {
        amount += amountToAdd;
        UpdateSlots();
        return amount;
    }

    public int RemoveAmount (int amountToRemove) //xóa một số lượng nhất định khỏi slot, nếu số lượng sau khi xóa nhỏ hơn hoặc bằng 0 thì xóa item khỏi slot
    {
        amount -= amountToRemove;
        if (amount <= 0)
        {
            ClearSlots();
        }
        else
        {
            UpdateSlots();
        }
        return amount;
    }

    public void ClearSlots() //xóa item khỏi slot, đặt item về null và số lượng về 0
    {
        item = null;
        amount = 0;
        UpdateSlots();
    }

    public bool HasItem() //kiểm tra xem slot có item hay không
    {
        return item != null;
    }
}
