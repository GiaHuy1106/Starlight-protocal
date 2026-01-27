using System.Collections;

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f; // Tốc độ nhân vật di chuyển
    public float runMultiplier = 1.8f; // Hệ số tăng tốc khi nhân vật chạy
    public float acceleration = 6f; // Tốc độ animation speed tăng  IDLE -> RUN, Walk -> Run
    public float deceleration = 8f; // Tốc độ animation speed giảm  IDLE -> Walk, Run -> Walk

    [Header("Gravity")]
    public float gravity = -9.81f; 
    [Header("References")]
    public CharacterController characterController; 
    public PlayerInput playerInput; 
    //Internal
    public Vector3 _movementVelocity; 
    private float _verticalVelocity; 
    [Header("Jump")]
    public float jumpForce = 6f;
    [Header("Dodge")]
    public float dodgeForce = 6f;
    private bool isDodging;
    //Animator
    public Animator playerAnimator; 
    public int speedHash; 
    private float currentAnimSpeed;
    private float targetAnimSpeed;

    public int attackHash; 
    public int dieHash; 
    public GameObject playerModel; // model để xoay nhân vật 

    void Start() 
    { 
        
        //Khởi tạo Hash cho các trạng thái animation 
        speedHash = Constant.SpeedHash;
        playerAnimator.SetFloat(speedHash, 0f);
    } 
    // xử lý di chuyển trong FixedUpdate 
    void Update() 
    { 
            if (playerInput.IsInputLocked)
        {
            _movementVelocity.x = 0;
            _movementVelocity.z = 0;

            ApplyGravity(); // vẫn cho rơi tự nhiên
            characterController.Move(_movementVelocity * Time.deltaTime);
            return;
        }

        HandleDogde();

        if (!isDodging)
        {
            CalculateMovement();
            HandleJump();
        }

        ApplyGravity();
        characterController.Move(_movementVelocity * Time.deltaTime);
        HandleLanding();
        UpdateAnimanator();

        // Vector3 horizontalVelocity = new Vector3(_movementVelocity.x, 0, _movementVelocity.z);
        // float normalizedSpeed = horizontalVelocity.magnitude / moveSpeed;
        // playerAnimator.SetFloat(speedHash, normalizedSpeed);
    }
    // hàm tính toán vector di chuyển 
    void CalculateMovement() 
    { 
        if (playerInput.IsInputLocked) return; //khóa toàn bộ gameplay
        Vector3 input = new Vector3(playerInput.horizontalInput, 0,playerInput.verticalInput);

        input = Vector3.ClampMagnitude(input, 1f);
        if (input.sqrMagnitude < 0.01f)
        {
            _movementVelocity.x = 0f;
            _movementVelocity.z = 0f;
            return;
        }

        //lấy hướng camera 
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // QUYẾT ĐỊNH TỐC ĐỘ WALK / RUN
        Vector3 moveDir = camForward * input.z + camRight * input.x;
        float speed = moveSpeed;
        if (playerInput.IsRunning())
        {
            Debug.Log("RUNNING");
            speed *= runMultiplier; // <-- RUN SPEED
        }

        _movementVelocity.x = moveDir.x * speed;
        _movementVelocity.z = moveDir.z * speed;

        //Xoay model theo camera
        if (moveDir.sqrMagnitude > 0.01f)
        {
            // Smooth rotation (đẹp hơn)
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetRot, 12f * Time.deltaTime);
        }
    }
    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            // CHỈ reset khi KHÔNG nhảy
            if (!playerAnimator.GetBool(Constant.JumpHash))
            {
                _verticalVelocity = -2f;
            }
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        _movementVelocity.y = _verticalVelocity;
    }
    void UpdateAnimanator()
    {
        float inputMagnitude = playerInput.GetInputMagnitude();
        if (inputMagnitude > 0.01f)
        {
            targetAnimSpeed = playerInput.IsRunning() ? 1f : 0.5f;
        }
        else
        {
            targetAnimSpeed = 0f;
        }
        float smooth = targetAnimSpeed > currentAnimSpeed ? acceleration : deceleration;
        currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, smooth * Time.deltaTime);

        playerAnimator.SetFloat(speedHash, currentAnimSpeed);
    }
    // hàm để nhân vật khi nhảy lên
    void HandleJump()
    {
       if (characterController.isGrounded && !playerAnimator.GetBool(Constant.JumpHash))
        {
            if (playerInput.IsJumping())
            {
                _verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpForce);
                playerAnimator.SetBool(Constant.JumpHash, true);
                Debug.Log("JUMP!");
            }
        }
    }
    // hàm nhân vật chạm đất
    void HandleLanding()
    {
        if (characterController.isGrounded && playerAnimator.GetBool(Constant.JumpHash))
        {
            playerAnimator.SetBool(Constant.JumpHash, false);
        }
    }
    // hàm để nhân vật né
    void HandleDogde()
    {
            if (isDodging) return;
            if (!playerInput.IsDodging()) return;

           Vector2 raw = playerInput.GetRawInputDir();
           Vector3 dir = new Vector3(raw.x, 0, raw.y);

            if (dir.sqrMagnitude < 0.1f)
            dir = playerModel.transform.forward;

            dir.Normalize();

            StartCoroutine(DodgeRoutine(dir, raw.x));
    }
    IEnumerator DodgeRoutine(Vector3 dir, float horizontal)
    {
        isDodging = true;

        float dodgeTime = 0.25f; // thời gian dash
        float timer = 0f;

        // chọn animation theo trái phải
        if (horizontal > 0f)
            playerAnimator.SetTrigger(Constant.DodgeRightHash);
        else
            playerAnimator.SetTrigger(Constant.DodgeLeftHash);

        while (timer < dodgeTime)
        {
            _movementVelocity.x = dir.x * dodgeForce;
            _movementVelocity.z = dir.z * dodgeForce;

            timer += Time.deltaTime;
            yield return null;
        }
        _movementVelocity.x = 0;
        _movementVelocity.z = 0;

        isDodging = false;
    }

}