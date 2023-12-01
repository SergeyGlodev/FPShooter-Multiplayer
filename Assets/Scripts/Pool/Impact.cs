using DG.Tweening;
using UnityEngine;

public class Impact : PoolObject
{
    [SerializeField] private float timeBeforeVanish = 10f;


    public override void OnObjectReuse ()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        AudioSource audioSource = GetComponent<AudioSource>();
        particleSystem.Clear();
        particleSystem.Play();

        gameObject.SetActive(false);
        DOVirtual.DelayedCall(timeBeforeVanish, Destroy);
    }
}
