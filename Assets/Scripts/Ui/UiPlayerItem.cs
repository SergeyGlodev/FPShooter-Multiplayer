using TMPro;
using UnityEngine;

public class UiPlayerItem : MonoBehaviour
{
    [HideInInspector] public LobbyNetworkManager lobbyNetworkParent;

    [SerializeField] private TextMeshProUGUI playerName;


    public void SetName(string playerName) => this.playerName.text = playerName;

    public void OnJoinPressed() => lobbyNetworkParent.JoinRoom(playerName.text);
}