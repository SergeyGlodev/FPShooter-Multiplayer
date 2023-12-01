using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiRoomItem : MonoBehaviour
{
    [HideInInspector] public LobbyNetworkManager lobbyNetworkParent;

    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private Button join;


    private void Awake() => join.onClick.AddListener(OnJoinPressed);
    public void SetName(string roomName) => this.roomName.text = roomName;

    public void OnJoinPressed() => lobbyNetworkParent.JoinRoom(roomName.text);
}