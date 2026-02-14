using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public Camera cam;
    public LayerMask enemyLayer;
    public GameObject crosshairPrefab;
    GameObject currentCrosshair;
    Transform currentTarget;

    
    void Update()
    {
        DetectTarget();
    }
    void DetectTarget()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
        {
            Transform target = hit.transform;

            if (currentTarget != target)
            {
                ClearTarget();
                currentTarget = target;
                currentCrosshair = Instantiate(crosshairPrefab);
            }

            // Lấy collider center để luôn nằm chính giữa enemy
            Collider col = target.GetComponent<Collider>();
            Vector3 center = col != null ? col.bounds.center : target.position;

            // Lấy hướng về camera
            Vector3 dirToCam = (cam.transform.position - center).normalized;

            // Đẩy ra phía trước mặt enemy
            Vector3 finalPos = center + dirToCam * 0.8f;

            currentCrosshair.transform.position = finalPos;

            // Billboard về camera
            currentCrosshair.transform.rotation =
                Quaternion.LookRotation(currentCrosshair.transform.position - cam.transform.position);
        }
        else
        {
            ClearTarget();
        }
    }

    void ClearTarget()
    {
        if (currentCrosshair != null)
        {
            Destroy(currentCrosshair);
        }
        currentTarget = null;
    }
    public Transform GetTarget()
    {
        return currentTarget;
    }
}
