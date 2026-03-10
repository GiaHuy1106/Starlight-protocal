using UnityEngine;

public class JumpImpactVFX : MonoBehaviour
{
    public float lifeTime = 3f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
