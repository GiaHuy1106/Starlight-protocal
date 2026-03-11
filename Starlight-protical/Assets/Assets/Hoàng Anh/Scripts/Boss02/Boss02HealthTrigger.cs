using UnityEngine;

public class Boss02HealthTrigger : MonoBehaviour
{
    public Boss02Health bossHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bossHealth.ShowHealthBar();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bossHealth.HideHealthBar();
        }
    }
}
