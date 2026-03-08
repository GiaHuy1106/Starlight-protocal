using UnityEngine;

public class NotRotationText : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // Lấy Transform của Camera chính khi bắt đầu
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // LateUpdate khi cam xoay xong rồi mới xoay canvas
    void LateUpdate()
    {
        if (camTransform == null) return;

        // Canvas có cùng góc xoay với Camera
        transform.rotation = camTransform.rotation;

    }
}
