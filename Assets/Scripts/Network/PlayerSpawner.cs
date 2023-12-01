using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private Vector3 tBase;
    private Vector3 ctBase;
    private Quaternion tRotate;
    private Quaternion ctRotate;
    private MapManager mapManager;


    private void Awake()
    {
        mapManager = GetComponent<MapManager>();
        GlobalEvents.OnStartGamePressed += SpawnPlayer;
    }

    public void SetBases(Vector3 tBase, Vector3 ctBase)
    {
        this.tBase = tBase;
        this.ctBase = ctBase;
    }

    private void SpawnPlayer()
    {
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        tBase = mapManager.GetCurrentTBase();
        tRotate = mapManager.GetCurrentTBaseRotate();
        ctBase = mapManager.GetCurrentCtBase();
        ctRotate = mapManager.GetCurrentCtBaseRotate();

        if (playerNumber % 2 == 0)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, tBase, tRotate);
        }
        else
        {
            PhotonNetwork.Instantiate(playerPrefab.name, ctBase, ctRotate);
        }
    }
}
