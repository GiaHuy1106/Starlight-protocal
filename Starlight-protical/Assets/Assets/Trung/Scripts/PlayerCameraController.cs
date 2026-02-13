using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public CinemachineFreeLook freeLookCam;
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minRadius = 2f;
    public float maxRadisu= 6f;
    [Header("Deadzone")]
    public float deadzone = 150f;
    public float rotationSpeed = 120f;
    public float smoothTime = 0.4f; // càng lớn càng chậm
    public bool IsEdgeScrolling { get;private set; }
    float currentYaw;
    float yawVelocity;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentYaw = freeLookCam.m_XAxis.Value;
    }

    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        HandleDeadzoneRotation();
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll)  < 0.01f) return;
        float currentRadius = freeLookCam.m_Orbits[1].m_Radius;
        currentRadius -= scroll * zoomSpeed;
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadisu);
        //set cho cả 3 rig để không bị zoom khi lia dọc
        for (int i = 0; i < 3; i++)
        {
            freeLookCam.m_Orbits[i].m_Radius = currentRadius;
        }
    }
    void HandleDeadzoneRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        IsEdgeScrolling = false;

        if (mousePos.x <= 0f)
        {
            currentYaw -= rotationSpeed * Time.deltaTime;
            freeLookCam.m_XAxis.Value = currentYaw;
            IsEdgeScrolling = true;
        }
        else if (mousePos.x >= Screen.width - 1f)
        {
            currentYaw += rotationSpeed * Time.deltaTime;
            freeLookCam.m_XAxis.Value = currentYaw;
            IsEdgeScrolling = true;
        }
    }
    public void ForceRotateTo(Vector3 direction)
    {
        float targetYaw = Quaternion.LookRotation(direction).eulerAngles.y;

        currentYaw = Mathf.SmoothDampAngle(
        currentYaw,
        targetYaw,
        ref yawVelocity,
        smoothTime
    );

    freeLookCam.m_XAxis.Value = currentYaw;
    }
}
