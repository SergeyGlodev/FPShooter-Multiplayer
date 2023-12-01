using DG.Tweening;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    public SOWeapon SOWeapon;
    public GameObject weaponModel;

    public float FireRate => fireRate;
    public float ReloadTime => reloadTime;
    public float DelayAfterTakeWeapon => delayAfterTakeWeapon;
    public float Damage => damage;
    public int MaximumAmmo => maximumAmmo;
    public float NextTimeToFire { get; private set; }
    public int CurrentAmmo { get; private set; }

    private int maximumAmmo;
    private float fireRate;
    private float delayMuzzleFlashBeforeFade;
    private float reloadTime;
    private float delayAfterTakeWeapon;
    private float nextTimeToWeaponsReady;
    private float damage;
    private GameObject muzzleFlash;


    private void Awake()
    {
        maximumAmmo = SOWeapon.FullAmmo;
        fireRate = SOWeapon.FireRate;
        delayMuzzleFlashBeforeFade = SOWeapon.DelayMuzzleFlashBeforeFade;
        reloadTime = SOWeapon.ReloadTime;
        delayAfterTakeWeapon = SOWeapon.DelayAfterTakeWeapon;
        damage = SOWeapon.Damage;

        muzzleFlash = transform.Find("MuzzleFlash").gameObject;
        muzzleFlash.SetActive(false);
        CurrentAmmo = maximumAmmo;

        NextTimeToFire = Time.time + DelayAfterTakeWeapon;
        UpdateNextTimeToWeaponsReady();
    }

    public void MuzzleFlashShoot()
    {
        muzzleFlash.SetActive(true);
        DOVirtual.DelayedCall(delayMuzzleFlashBeforeFade, () => muzzleFlash.SetActive(false));
    }

    public void SetAmmoFull() => CurrentAmmo = maximumAmmo;

    public void SetNextTimeToFire(float nextTimeToFire) => NextTimeToFire = nextTimeToFire;

    public void DecreaseAmmoInfo() => CurrentAmmo -= 1;

    public void UpdateNextTimeToWeaponsReady() => nextTimeToWeaponsReady = Time.time + delayAfterTakeWeapon;

    public bool IsAmmoFull() => CurrentAmmo == maximumAmmo;

    public bool WeaponIsReady() => Time.time >= nextTimeToWeaponsReady;
}