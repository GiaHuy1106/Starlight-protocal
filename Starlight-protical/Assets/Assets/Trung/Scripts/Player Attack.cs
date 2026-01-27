using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput playerInput;
    public Animator playerAnimator;
    public PlayerSkill playerSkill;
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
        if (isAttacking) return;

        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        if (playerInput.IsAltHolding()) return;

        if (playerInput.IsAttacking() && playerSkill.IsBasicReady)
        {
            StartAttack(attackHash);
        }
        else if (playerInput.IsSpecialAttacking() && playerSkill.IsSpecialReady) 
        {
            StartAttack(skillHash);
        }
    }

    void StartAttack(int hash)
    {
        isAttacking = true;

        playerInput.SetInputLock(true);

        playerAnimator.SetTrigger(hash);
    }

    public void EndAttack()
    {
        isAttacking = false;
        playerInput.SetInputLock(false);
    }
}
