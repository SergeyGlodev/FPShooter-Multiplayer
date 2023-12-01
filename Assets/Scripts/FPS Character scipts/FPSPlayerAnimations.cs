using UnityEngine;

public class FPSPlayerAnimations : MonoBehaviour
{
    readonly int AnimatorMoveFloat = Animator.StringToHash("Move");
    readonly int AnimatorVelocityYFloat = Animator.StringToHash("VelocityY");
    readonly int AnimatorCrouchWalkFloat = Animator.StringToHash("CrouchWalk");
    readonly int AnimatorCrouchBool = Animator.StringToHash("Crouch");
    readonly int AnimatorStandShootTrigger = Animator.StringToHash("StandShoot");
    readonly int AnimatorCrouchShootTrigger = Animator.StringToHash("CrouchShoot");
    readonly int AnimatorReloadBool = Animator.StringToHash("IsReloading");
    readonly int AnimatorWithPistolBool = Animator.StringToHash("WithPistol");
    readonly int AnimatorDeathBool = Animator.StringToHash("Death");
    readonly int AnimatorStayCrouchFloat = Animator.StringToHash("StayCrouchFloat");

    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        ChangeController(isPistol: true);
    }

    public void Movement(float magnitude) => animator.SetFloat(AnimatorMoveFloat, magnitude);

    public void PlayerJump(float velocity) => animator.SetFloat(AnimatorVelocityYFloat, velocity);

    public void PlayerCrouchWalk(float magnitude) => animator.SetFloat(AnimatorCrouchWalkFloat, magnitude);

    public void PlayerCrouch(bool isCrouching)
    {
        animator.SetBool(AnimatorCrouchBool, isCrouching);

        if (isCrouching)
        {
            animator.SetFloat(AnimatorStayCrouchFloat, 0f);
        }
        else
        {
            animator.SetFloat(AnimatorStayCrouchFloat, 1f);
        }

    }

    public void PlayerDeath(bool isDeath) => animator.SetBool(AnimatorDeathBool, isDeath);

    public void IsReloading(bool isReloading) => animator.SetBool(AnimatorReloadBool, isReloading);

    public void Shoot(bool isCrouching)
    {
        if (isCrouching)
        {
            animator.SetTrigger(AnimatorCrouchShootTrigger);
        }
        else
        {
            animator.SetTrigger(AnimatorStandShootTrigger);
        }
    }

    public void ChangeController(bool isPistol)
    {
        animator.SetBool(AnimatorWithPistolBool, isPistol);
    }
}
