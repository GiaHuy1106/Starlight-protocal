using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats playerStats;
    public Animator playerAnimator;
    void Start()
    {
        playerStats.OnDamaged += PlayHurt;
        playerStats.OnDead += Die;
    }

    void PlayHurt(int dmg)
    {
        playerAnimator.SetTrigger(Constant.HurtHash);
    }
    void Die()
    {
        playerAnimator.SetTrigger(Constant.DieHash);
        GetComponent<PlayerInput>().SetInputLock(true);
        GetComponent<PlayerMovement>().enabled = false;
    }
}
