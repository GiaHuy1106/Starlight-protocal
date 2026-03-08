using UnityEngine;

public class FirePointFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        transform.forward = player.forward;
    }
}
