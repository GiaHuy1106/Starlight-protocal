using UnityEngine;

public class IceWarningZone : MonoBehaviour
{
    public float warningTime = 1.5f;
    public float spawnHeight = 30f;
    public GameObject iceFallingPrefab;
    public LayerMask groundLayer;
    void Start()
    {
        AdjustToGround();
        Invoke(nameof(SpawnIce), warningTime);
    }
    void AdjustToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 50f, groundLayer))
        {
            //Debug.Log("Raycast trúng: " + hit.collider.name);
            transform.position = hit.point;
        }
    }    
    void SpawnIce()
    {
        if (iceFallingPrefab == null)
        {
            Debug.LogError("iceFallingPrefab chưa được gán!");
            return;
        }
        Vector3 spawnPos = transform.position + Vector3.up * spawnHeight;
        
        Instantiate(iceFallingPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }  
}
