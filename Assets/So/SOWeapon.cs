using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "mySO/Weapon")]
public class SOWeapon : ScriptableObject
{
    [SerializeField] private bool isPistol;
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private int fullAmmo;
    [SerializeField] private float fireRate;
    [SerializeField] private float delayMuzzleFlashBeforeFade;
    [SerializeField] private float reloadTime;
    [SerializeField] private float delayAfterTakeWeapon;
    [SerializeField] private bool isAuto;
    [SerializeField] private int weaponIndex;
    [SerializeField] private string weaponName;
    [SerializeField] private float delayToPlayReloadSound;
    [SerializeField] private float damage;
    [SerializeField] private SfxClipIndex shootClip;
    [SerializeField] private SfxClipIndex reloadClip;

    public bool IsPistol => isPistol;
    public KeyCode KeyCode => keyCode;
    public int FullAmmo => fullAmmo;
    public float FireRate => fireRate;
    public float DelayMuzzleFlashBeforeFade => delayMuzzleFlashBeforeFade;
    public float ReloadTime => reloadTime;
    public float DelayAfterTakeWeapon => delayAfterTakeWeapon;
    public bool IsAuto => isAuto;
    public int WeaponIndex => weaponIndex;
    public string WeaponName => weaponName;
    public float DelayToPlayReloadSound => delayToPlayReloadSound;
    public float Damage => damage;
    public SfxClipIndex ShootClip => shootClip;
    public SfxClipIndex ReloadClip => reloadClip;
    public float NextTimeToFire { get; private set; }
    public int CurrentAmmo { get; private set; }
}
