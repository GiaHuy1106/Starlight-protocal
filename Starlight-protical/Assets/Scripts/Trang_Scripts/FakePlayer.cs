using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class FakePlayer : MonoBehaviour
{
    [Header("Thông số")]
    public float moveSpeed = 6f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    public Transform enemyTransform;
    public Transform enemyTransform2;
    public Transform enemyTransform3;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); 
        float z = Input.GetAxisRaw("Vertical");   

        Vector3 moveDirection = new Vector3(x, 0, z).normalized;

        Vector3 currentVelocity = rb.linearVelocity; 
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, currentVelocity.y, moveDirection.z * moveSpeed);

        //Tấn công giả để test anim enemy
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(enemyTransform!=null)
                enemyTransform.GetComponent<Enemy1>().TakeDamage(10);
            if(enemyTransform2!=null)
                enemyTransform2.GetComponent<Enemy2>().TakeDamage(10);
            if(enemyTransform3!=null)
                enemyTransform3.GetComponent<Enemy3>().TakeDamage(10);
        }    
    }
}
