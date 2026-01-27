using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    public float destroyTime = 2f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
