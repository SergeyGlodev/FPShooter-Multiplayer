using UnityEngine;
using DG.Tweening;

public class FPSHandsWeapon : MonoBehaviour
{
    readonly int AnimatorShootTrigger = Animator.StringToHash("Shoot");
    readonly int AnimatorReloadTrigger = Animator.StringToHash("Reload");

    [SerializeField] private SOWeapon SOWeapon;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Animator animator;

    public float delayMuzzleFlashBeforeFade;
    public float delayToPlayReloadSound;

    private SfxClipIndex shootClip;
    private SfxClipIndex reloadClip;
    private Tween reloadSoundCall;


    private void Awake()
    {
        shootClip = SOWeapon.ShootClip;
        reloadClip = SOWeapon.ReloadClip;
    }

    public void OnShootMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        DOVirtual.DelayedCall(delayMuzzleFlashBeforeFade, () => muzzleFlash.SetActive(false));
    }

    public void OnShootAnim() => animator.SetTrigger(AnimatorShootTrigger);

    public void OnShootAudio() => AudioManager.OnPointPlay2dSound?.Invoke(shootClip);

    public void PlayReloadVisual()
    {
        animator.SetTrigger(AnimatorReloadTrigger);
        reloadSoundCall = DOVirtual.DelayedCall(delayToPlayReloadSound, PlayReloadSound);
    }

    public void StopReloadVisual() => CancelDelayCall(reloadSoundCall);

    private void PlayReloadSound() => AudioManager.OnPointPlay2dSound?.Invoke(reloadClip);

    private void CancelDelayCall(Tween DelayedCallTween)
    {
        if (DelayedCallTween != null && DelayedCallTween.IsActive())
        {
            DelayedCallTween.Kill();
        }
    }
}