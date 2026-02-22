using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public bool IsInputLocked { get; private set;}
    
    void Start()
    {
        IsInputLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInputLocked)
        {
            horizontalInput = 0;
            verticalInput = 0;
            return;
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput   = Input.GetAxisRaw("Vertical");  
    }
    //Độ mạnh input (0 -> 1)
    public float GetInputMagnitude()
    {
        Vector2 input = new Vector2(horizontalInput, verticalInput);
        input = Vector2.ClampMagnitude(input, 1f);
        return input.magnitude;
    }
    // Nút chạy
    public bool IsRunning() 
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
    // Nút nhảy
    public bool IsJumping() 
    {
        return Input.GetKeyDown(KeyCode.Z);
    }
    // Nút né 
    public bool IsDodging() 
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public Vector2 GetRawInputDir() 
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    // nút giữ ALT để mở con trỏ chuột (UI mode)
    public bool IsAltHolding() 
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }
    // khóa tất cả input khi hiện trỏ chuột
    public void SetInputLock(bool value)
    {
        IsInputLocked = value;
    }
    // Nhấn chuột trái xài basic attack
    public bool IsAttacking(bool block = false) 
    {
        if (IsInputLocked) return false;
        if (block) return false;
        return Input.GetMouseButtonDown(0);
    }
    // Nhấn chuột phái xài special attack

    public bool IsAimSkill() // Dùng để nhắm kỹ năng special
    {
        if (IsInputLocked) return false;
        return Input.GetKeyDown(KeyCode.R);
    }
    public bool IsConfirmSkill() // Dùng để phóng kỹ năng special
    {
        
        return Input.GetMouseButtonDown(0);
    }
    public bool IsCancelSkill() // Dùng để huy kỹ năng special
    {

        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
    }
}
