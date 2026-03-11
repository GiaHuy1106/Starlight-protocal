using UnityEngine;

public class WarningZoneBoss02 : MonoBehaviour
{   
    public float spawnHeight = 30f;
    public float groundOffset = 0.05f;
    public LayerMask groundLayer;
    void Start()
    {
        AdjustToGround();       
    }
    void AdjustToGround()
    {
        RaycastHit hit;

        Vector3 rayStart = transform.position + Vector3.up * spawnHeight;  

        if (Physics.Raycast(rayStart, Vector3.down, out hit,spawnHeight * 2, groundLayer))
        {
            //Debug.Log("Raycast trúng: " + hit.collider.name);
            transform.position = hit.point + Vector3.up * groundOffset;
        }
    }
}
