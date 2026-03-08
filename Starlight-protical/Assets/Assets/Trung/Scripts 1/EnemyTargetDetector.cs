using UnityEngine;

public class EnemyTargetDetector : MonoBehaviour
{
   public LayerMask enemyLayer;
    public Transform currentTarget;
    public GameObject aimIndicatorPrefab;

    GameObject indicatorInstance;

    [Header("Indicator Settings")]
    public float indicatorHeight = 0.5f; 
    public float indicatorDistance = 1.0f; 

    Transform player;

    void Start()
    {
        player = transform;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
        {
            currentTarget = hit.transform;
            ShowIndicator();
        }
        else
        {
            currentTarget = null;
            HideIndicator();
        }

        UpdateIndicatorPosition();
    }

    void UpdateIndicatorPosition()
    {
         if (indicatorInstance == null || currentTarget == null) return;

        // hướng enemy → player
        Vector3 dir = (player.position - currentTarget.position).normalized;
        dir.y = 0;

        // vị trí indicator
        Vector3 pos = currentTarget.position + dir * indicatorDistance;
        pos.y = currentTarget.position.y + indicatorHeight;

        indicatorInstance.transform.position = pos;

        // ⭐ xoay indicator theo camera
        indicatorInstance.transform.rotation =
            Quaternion.LookRotation(Camera.main.transform.forward);
    }

    void ShowIndicator()
    {
        if (indicatorInstance == null)
        {
            indicatorInstance = Instantiate(aimIndicatorPrefab);
        }
    }

    void HideIndicator()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
        }
    }
}

