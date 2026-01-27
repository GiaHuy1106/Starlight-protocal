using UnityEngine;
using Cinemachine;
public class PlayerCameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public CinemachineFreeLook freeLookCam;
    private bool cursorUnlocked;
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minRadius = 2f;
    public float maxRadisu= 6f;
    void Start()
    {
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCursor();
        HandleZoom();
    }
    void HandleCursor ()
    {
        bool holdingAlt = playerInput.IsAltHolding();
        if (holdingAlt && !cursorUnlocked) 
        {
            UnlockCursor();
        }
        else if (!holdingAlt && cursorUnlocked) 
        {
            LockCursor();
        }
    }
    void UnlockCursor ()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        freeLookCam.enabled = false; // tắt hẳn camera input
        playerInput.SetInputLock(true); // khóa gameplay
        cursorUnlocked = true;
    }
    void LockCursor ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        freeLookCam.enabled = true; // bật lại camera input
        playerInput.SetInputLock(false); // bật lại gameplay
        cursorUnlocked = false;
    }
    void HandleZoom()
    {
        if (cursorUnlocked) return;

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
}
