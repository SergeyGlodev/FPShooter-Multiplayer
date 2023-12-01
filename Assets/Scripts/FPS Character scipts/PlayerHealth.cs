using Photon.Pun;
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public bool IsDead => isDead;
    public float Health => health;

    public event Action <bool> OnDeadBool;
    public event Action <bool> OnResurrectBool;

    private float health;
    private bool isDead = false;
    private PhotonView view;


    private void Start()
    {
        view = GetComponent<PhotonView>();
        health = maxHealth;
        GlobalEvents.OnGameRestarted += Resurrect;
    }

    public void TakeDamage(float damage) => view.RPC(nameof(TakeDamageRpc), RpcTarget.All, damage);

    public void Resurrect() => view.RPC(nameof(ResurrectRpc), RpcTarget.All);

    private void Die()
    {
        isDead = true;
        GlobalEvents.OnDead?.Invoke();
        OnDeadBool?.Invoke(isDead);
    }

    [PunRPC]
    private void TakeDamageRpc(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    [PunRPC]
    private void ResurrectRpc()
    {
        health = maxHealth;
        isDead = false;
        OnResurrectBool?.Invoke(isDead);
    }
}
