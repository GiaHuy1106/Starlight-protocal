using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f; // Tốc độ nhân vật di chuyển
    public float runMultiplier = 1.8f; // Hệ số tăng tốc khi nhân vật chạy
    public float acceleration = 6f; // Tốc độ animation speed tăng  IDLE -> RUN, Walk -> Run
    public float deceleration = 8f; // Tốc độ animation speed giảm  IDLE -> Walk, Run -> Walk
    private PlayerCameraController cameraController;
    [Header("Gravity")]
    public float gravity = -9.81f; 
    [Header("References")]
    public CharacterController characterController ; 
    public PlayerInput playerInput; 
    //Internal
    public Vector3 _movementVelocity; 
    private float _verticalVelocity; 
    [Header("Jump")]
    public float jumpForce = 6f;
    [Header("Dodge")]
    public float dodgeForce = 6f;
    private bool isDodging;
    [Header("Footstep")]
    public AudioSource footstepSource;
    public AudioClip[] grassFootsteps;

    //Animator
    public Animator playerAnimator; 
    public int moveXHash;
    public int moveZHash;
    private float currentAnimSpeed;
    private float targetAnimSpeed;

    public int attackHash; 
    public int dieHash; 
    public GameObject playerModel; // model để xoay nhân vật 

    private Vector3 knockbackVelocity;
    private bool isSlowed;
    private float originalSpeed;
    public float knockbackDamping = 6f;

    void Start() 
    { 
        cameraController = FindObjectOfType<PlayerCameraController>();
        //Khởi tạo Hash cho các trạng thái animation 
        moveXHash = Constant.MoveXHash;
        moveZHash = Constant.MoveZHash;
        playerAnimator.SetFloat(moveXHash, 0f);
        playerAnimator.SetFloat(moveZHash, 0f);
        originalSpeed = moveSpeed;
    } 
    // xử lý di chuyển trong FixedUpdate 
    void Update() 
    { 
            if (playerInput.IsInputLocked)
        {
            _movementVelocity.x = 0;
            _movementVelocity.z = 0;

            ApplyGravity(); // vẫn cho rơi tự nhiên
            Vector3 finalVelocityLocked = _movementVelocity + knockbackVelocity;
            characterController.Move(finalVelocityLocked * Time.deltaTime);

            // Giảm dần lực knockback
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
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

        // CHẶN S + A / S + D
        if (input.z < 0 && Mathf.Abs(input.x) > 0.1f)
        {
            moveDir = camRight * input.x;
        }

        moveDir.Normalize();
        float speed = moveSpeed;
        if (playerInput.IsRunning())
        {
            speed *= runMultiplier;
        }

        _movementVelocity.x = moveDir.x * speed;
        _movementVelocity.z = moveDir.z * speed;

        // Luôn nhìn theo hướng camera
        Vector3 lookDir = camForward;

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            playerModel.transform.rotation =
                Quaternion.Slerp(playerModel.transform.rotation, targetRot, 12f * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
         bool grounded = characterController.isGrounded;

    if (grounded)
    {
        // Nếu đang rơi xuống và đã chạm đất
        if (_verticalVelocity < 0f)
        {
            _verticalVelocity = 0f;   // ✅ reset hoàn toàn
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
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 velocity = new Vector3(_movementVelocity.x, 0, _movementVelocity.z);
        // chuyển velocity sang local camera space
        float currentSpeed = playerInput.IsRunning() ? moveSpeed * runMultiplier : moveSpeed;
        float z = Vector3.Dot(velocity.normalized, camForward);
        float x = Vector3.Dot(velocity.normalized, camRight);

        Vector2 input = new Vector2(x, z);
        if (input.magnitude > 0.01f)
        {
            input.Normalize(); // luôn giữ tốc độ animation = 1
        }
        // ép tốc độ di chuyển chéo WA, WD = 1, không có là lun di chuyển tốc độ thành 0.7 cảm giác như slow-motion
        input.x = Mathf.Round(input.x);
        input.y = Mathf.Round(input.y);
        if (playerInput.IsRunning())
        {
            input *= 2f;
        }
        float smoothX = Mathf.Lerp(playerAnimator.GetFloat(moveXHash), input.x, Time.deltaTime * 10f);
        float smoothZ = Mathf.Lerp(playerAnimator.GetFloat(moveZHash), input.y, Time.deltaTime * acceleration);

        playerAnimator.SetFloat(moveXHash, smoothX);
        playerAnimator.SetFloat(moveZHash, smoothZ);
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
            Transform cam = Camera.main.transform;
            Vector3 camRight = cam.right;
            camRight.y = 0;
            camRight.Normalize();

            float horizontal = playerInput.horizontalInput;

            Vector3 dir;

            if (horizontal > 0.1f)
                dir = camRight;        // roll phải
            else if (horizontal < -0.1f)
                dir = -camRight;       // roll trái
            else
                dir = -camRight;       // đứng yên → roll trái

            StartCoroutine(DodgeRoutine(dir, horizontal));
    }
    IEnumerator DodgeRoutine(Vector3 dir, float horizontal)
    {
        isDodging = true;

        playerInput.SetAttackLock(true);
        playerInput.SetAimLock(true);

        float dodgeTime = 0.25f;
        float timer = 0f;

        if (horizontal > 0f)
            playerAnimator.SetTrigger(Constant.DodgeRightHash);
        else
            playerAnimator.SetTrigger(Constant.DodgeLeftHash);

        while (timer < dodgeTime)
        {
            characterController.Move(dir * dodgeForce * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        playerInput.SetAttackLock(false);
        playerInput.SetAimLock(false);

        isDodging = false;
    }
    public void PlayFootstep()
    {
        if (grassFootsteps.Length == 0) return;
        if (footstepSource == null) return;

        int index = Random.Range(0, grassFootsteps.Length);

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.PlayOneShot(grassFootsteps[index], 0.8f);
    }
    public void ApplySlow(float slowPercent, float duration)
    {
        if (!isSlowed)
        {
            StartCoroutine(SlowCoroutine(slowPercent, duration));
        }
    }

    IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        isSlowed = true;

        moveSpeed = originalSpeed * (1f - slowPercent);

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;

        isSlowed = false;
    }
    public void ApplyKnockBack(Vector3 force)
    {
        knockbackVelocity = force;
    }
}