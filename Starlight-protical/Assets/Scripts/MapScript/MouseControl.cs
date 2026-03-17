using System;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    [Header("SetUp")]
    [SerializeField] float intensity = 10f;
    [SerializeField] Transform player;
    bool isLocked = true;
    public Action<bool> IsLockedChange;
    float yaw;
    float pitch;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;    
    Vector2 mouseDelta;
    public bool IsLocked
    {
        get => isLocked;
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            isLocked = value;
            IsLockedChange?.Invoke(value);
        }
    }

    private void Awake()
    { 
        IsLockedChange += LockeChange;
        IsLocked = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            IsLocked = !isLocked;
        }
        if (!isLocked) return;
        mouseDelta = Input.mousePositionDelta;
        float angleX = mouseDelta.x * intensity * Time.deltaTime;
        float angleY = mouseDelta.y * intensity * Time.deltaTime;
        yaw += angleX;
        pitch -= angleY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }




    void LockeChange(bool value)
    {
        player.GetComponent<Player>().LockMove(value);
    }

}
