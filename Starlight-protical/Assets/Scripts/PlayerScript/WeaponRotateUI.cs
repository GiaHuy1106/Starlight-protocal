using UnityEngine;
using System.Collections;
public class WeaponRotateUI : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 40f;
    float currentY;
    void Start()
    {
        if (target != null) currentY = target.localEulerAngles.y;
    }
    void Update()
    {
        if (target == null) return;
        currentY += rotateSpeed * Time.unscaledDeltaTime;
        Vector3 angles = target.localEulerAngles;
        target.eulerAngles = new Vector3(angles.x, currentY, angles.z);
    }
}
