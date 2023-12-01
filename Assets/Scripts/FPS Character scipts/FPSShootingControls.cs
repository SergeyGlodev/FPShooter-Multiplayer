using Photon.Pun;
using UnityEngine;

public class FPSShootingControls : MonoBehaviour
{
    [SerializeField] private GameObject concreteImpact;
    [SerializeField] private GameObject bodyImpact;
    [SerializeField] private LayerMask PlayerMask;

    private RaycastHit hit;
    private PhotonView view;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        view = GetComponent<PhotonView>();
    }
    
    public void ShootVisual()
    {
        if (!Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, 1000, PlayerMask))
        {
            return; 
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            view.RPC(nameof(OnHitBodyShootEffect), RpcTarget.All, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else
        {
            view.RPC(nameof(OnHitShootEffect), RpcTarget.All, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void ShootDamage(float damage)
    {
        if (!view.IsMine)
        {
            return;
        }

        RaycastHit onFireHit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out onFireHit, 1000, PlayerMask))
        {
            FPSController target = onFireHit.transform.GetComponent<FPSController>();

            if (target != null)
            {
                target.ReduceHealth(damage);
            }
        }
    }

    [PunRPC]
    private void OnHitShootEffect(Vector3 point, Quaternion rotation)
    {
        PoolManager.Instance.ReuseObject(concreteImpact, point, rotation);
        AudioManager.OnPointPlay3dSound?.Invoke(point, SfxClipIndex.ConcreteImpact);
    }

    [PunRPC]
    private void OnHitBodyShootEffect(Vector3 point, Quaternion rotation)
    {
        PoolManager.Instance.ReuseObject(bodyImpact, point, rotation);
        AudioManager.OnPointPlay3dSound?.Invoke(point, SfxClipIndex.BodyImpact);
    }
}
