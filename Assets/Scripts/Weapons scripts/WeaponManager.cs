using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponStorage weaponStorage;
    public WeaponStorage firstViewWeaponStorage;
    public List<SOWeapon> SOWeaponList;

    public FPSWeapon CurrentWeapon { get; private set; }
    public FPSHandsWeapon CurrentFirstViewWeapon { get; private set; }

    public event Action OnSelectWeaponStart;
    public event Action<int> OnSelectWeaponEnd;

    private PhotonView view;


    public void Awake()
    {
        for (int i = 0; i < weaponStorage.weapons.Length; i++)
        {
            SOWeaponList.Add(weaponStorage.weapons[i].GetComponent<FPSWeapon>().SOWeapon);
        }

        SetCurrentWeapon(index: 0);
        view = GetComponent<PhotonView>();
    }

    public void ChangeWeapon(KeyCode keyCode)
    {
        for (int i = 0; i < weaponStorage.weapons.Length; i++)
        {
            if (keyCode == SOWeaponList[i].KeyCode)
            {
                for (int j = 0; j < weaponStorage.weapons.Length; j++)
                {
                    weaponStorage.weapons[j].SetActive(false);
                    firstViewWeaponStorage.weapons[j].SetActive(false);
                }
                CurrentWeapon = null;
                CurrentFirstViewWeapon = null;

                SetCurrentWeapon(i);
                weaponStorage.weapons[i].SetActive(true);
                firstViewWeaponStorage.weapons[i].SetActive(true); 
                
                CurrentWeapon.UpdateNextTimeToWeaponsReady();
            }
        }
    }

    public void SelectWeapon()
    {
        for (int i = 0; i < weaponStorage.weapons.Length; i++)
        {
            if (InputManager.Instance.input.WeaponChangePressed(SOWeaponList[i]) 
                && !weaponStorage.weapons[i].activeInHierarchy)
            {
                view.RPC(nameof(SelectWeaponPun), RpcTarget.All, i);
            }
        }
    }

    public void SelectTargetWeapon(int index) => view.RPC(nameof(SelectWeaponPun), RpcTarget.All, index);

    private void SetCurrentWeapon(int index)
    {
        CurrentWeapon = weaponStorage.weapons[index].GetComponent<FPSWeapon>();
        CurrentFirstViewWeapon = firstViewWeaponStorage.weapons[index].GetComponent<FPSHandsWeapon>();
    }

    [PunRPC]
    private void SelectWeaponPun(int index)
    {
        OnSelectWeaponStart?.Invoke();
        CurrentFirstViewWeapon.StopReloadVisual();
        ChangeWeapon(SOWeaponList[index].KeyCode);
        CurrentWeapon.SetNextTimeToFire(Time.time + CurrentWeapon.DelayAfterTakeWeapon);
        OnSelectWeaponEnd?.Invoke(index);
    }
}





