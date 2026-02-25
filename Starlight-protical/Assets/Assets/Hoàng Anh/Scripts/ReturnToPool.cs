using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    public float lifeTime = 3f;
    private void OnEnable()
    {
        Invoke("DisableObject", lifeTime);
    }
    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
