using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput playerInput;
    public Animator playerAnimator;

    private int attackHash;
    private int skillHash;

    private bool isAttacking;

    void Start()
    {
        attackHash = Constant.AttackHash;
        skillHash  = Constant.SkillHash;
    }

    void Update()
    {
        if (isAttacking) return; // ⭐ chặn spam

        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        if (playerInput.IsAltHolding()) return;

        if (playerInput.IsAttacking())
        {
            StartAttack(attackHash);
        }
        else if (playerInput.IsSpecialAttacking())
        {
            StartAttack(skillHash);
        }
    }

    void StartAttack(int hash)
    {
        isAttacking = true;

        playerInput.SetInputLock(true); // ⭐ khóa di chuyển

        playerAnimator.SetTrigger(hash);
    }

    // ⭐ Animation Event gọi cuối clip
    public void EndAttack()
    {
        isAttacking = false;
        playerInput.SetInputLock(false);
    }
}
