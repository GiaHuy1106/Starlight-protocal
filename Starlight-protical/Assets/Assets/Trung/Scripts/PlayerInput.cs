using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;

    bool lockByAim;
    bool lockByAttack;
    bool lockByHurt;
    bool lockByUI;

    
    public bool IsInputLocked =>
    lockByAim ||
    lockByAttack ||
    lockByHurt ||
    lockByUI;
    public void SetAimLock(bool value)
    {
        lockByAim = value;
    }
    public void SetUILock(bool value)
    {
        lockByUI = value;
    }

    public void SetAttackLock(bool value)
    {
        lockByAttack = value;
    }
    public void SetHurtLock(bool value)
    {
        lockByHurt = value;
    }
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

    public float GetInputMagnitude()
    {
        Vector2 input = new Vector2(horizontalInput, verticalInput);
        input = Vector2.ClampMagnitude(input, 1f);
        return input.magnitude;
    }

    public bool IsRunning() => Input.GetKey(KeyCode.LeftShift);

    public bool IsJumping() => Input.GetKeyDown(KeyCode.Z);

    public bool IsDodging() => Input.GetKeyDown(KeyCode.Space);

    public Vector2 GetRawInputDir()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"),
                           Input.GetAxisRaw("Vertical"));
    }

    public bool IsAltHolding() => Input.GetKey(KeyCode.LeftAlt);

    public bool IsAttacking(bool block = false)
    {
        if (IsInputLocked) return false;
        if (block) return false;
        return Input.GetMouseButtonDown(0);
    }

    public bool IsAimSkill()
    {
        if (IsInputLocked) return false;
        return Input.GetKeyDown(KeyCode.R);
    }

    public bool IsConfirmSkill()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool IsCancelSkill()
    {
        return Input.GetMouseButtonDown(1) || 
               Input.GetKeyDown(KeyCode.Escape);
    }
}