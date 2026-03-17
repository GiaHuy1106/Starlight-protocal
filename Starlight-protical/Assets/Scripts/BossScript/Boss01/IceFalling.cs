using System.Collections;
using UnityEngine;

public class IceFalling : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float fallSpeed = 20f;
    private Rigidbody rb;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }
    //void Update()
   // {
    //    transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Ground"))
        {
            hasHit = true;

            //Vector3 hitPoint = other.contacts[0].point;
            Vector3 hitPoint = transform.position;
            Instantiate(explosionPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
