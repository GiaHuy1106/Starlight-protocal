using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarRotateUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
     public Transform target;
    public float rotateSpeed = 0.5f;

    bool dragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || target == null) return;

        float rot = eventData.delta.x * rotateSpeed;
        target.Rotate(Vector3.up * -rot, Space.Self);
    }
}
