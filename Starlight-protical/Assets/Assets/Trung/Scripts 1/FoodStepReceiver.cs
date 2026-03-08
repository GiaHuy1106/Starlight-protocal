using UnityEngine;

public class FoodStepReceiver : MonoBehaviour
{
     public void PlayFootstep()
    {
        GetComponentInParent<PlayerMovement>()?.PlayFootstep();
    }
}
