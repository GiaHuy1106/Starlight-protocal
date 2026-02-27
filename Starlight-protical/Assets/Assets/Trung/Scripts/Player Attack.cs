using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput playerInput;
    public Animator playerAnimator;
    public PlayerSkill playerSkill;

    private int attackHash;
    private int specialHash;
    private bool isAttacking;

    void Start()
    {
        attackHash = Constant.AttackHash;
        specialHash = Constant.SkillHash;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (playerInput.IsInputLocked) return;
        if (playerSkill.IsAiming) return;
        if (Time.timeScale == 0f) return;
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
    }

    void StartAttack(int hash)
    {
        isAttacking = true;

        playerInput.SetAttackLock(true);   // ✅ sửa

        playerAnimator.SetTrigger(hash);
    }

    public void EndAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  // ✅ sửa
    }

    public void StartSpecialAttack()
    {
        isAttacking = true;

        playerInput.SetAttackLock(true);   // ✅ sửa

        playerAnimator.SetTrigger(specialHash);
    }

    public void EndSpecialAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  // ✅ sửa
    }

    public void ForceStopAttack()
    {
        isAttacking = false;
        playerInput.SetAttackLock(false);  // ✅ sửa
    }
}
