using UnityEngine;

public class Constant
{
    public static readonly int MoveXHash = Animator.StringToHash("MoveX");
    public static readonly int MoveZHash = Animator.StringToHash("MoveZ");
    public static readonly int AttackHash = Animator.StringToHash("Basic Attack");
    public static readonly int FireballHash = Animator.StringToHash("Fireball Attack");
    public static readonly int SkillHash = Animator.StringToHash("Special Skill");
    public static readonly int JumpHash = Animator.StringToHash("IsJump");
    //Né
    public static readonly int DodgeLeftHash = Animator.StringToHash("DodgeLeft");
    public static readonly int DodgeRightHash = Animator.StringToHash("DodgeRight");
    public static readonly int HurtHash = Animator.StringToHash("Hurt");
    public static readonly int DieHash = Animator.StringToHash("Die");
}
