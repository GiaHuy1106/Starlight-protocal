using UnityEngine;

public class Boss01HealthTrigger : MonoBehaviour
{
    public Boss01Health bossHealth;

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
