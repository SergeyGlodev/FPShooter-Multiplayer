using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject roomListWindow;
    [SerializeField] private GameObject playerListWindow;
    [SerializeField] private UiRoomItem uiRoomItemPrefab;
    [SerializeField] private UiPlayerItem uiPlayerItemPrefab;

    private List<UiPlayerItem> playerList = new List<UiPlayerItem>();
    private List<UiRoomItem> roomItems = new List<UiRoomItem>();
    private List<RoomInfo> cashedRoomList = new List<RoomInfo>();


    private void Awake()
    {
        GlobalEvents.OnStartGamePressed += GameStarted;
        GlobalEvents.OnCreateRoom += CreateRoom;
        GlobalEvents.OnJoinButtonPressed += JoinRoom;
        GlobalEvents.OnLeaveRoom += LeaveRoom;
        GlobalEvents.OnLeaveGame += LeaveGame;
    }

    private void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(0, 5000);
        PhotonNetwork.SerializationRate = 40;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region WorkWithRoom

    private void CreateRoom(string fieldTextToName)
    {
        if (!string.IsNullOrEmpty(fieldTextToName) 
            && !fieldTextToName.Contains(" "))
        {
            PhotonNetwork.CreateRoom(fieldTextToName);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    private void GameStarted()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        cashedRoomList.Remove(PhotonNetwork.CurrentRoom);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region OnOverridesMethods

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        GlobalEvents.OnLobbyLoaded?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomListData(roomList);
        UpdateRoomListVisual(roomList);
    }

    public override void OnJoinedRoom()
    {
        IsBackButtonInteratable(false);
        ShowWindow(isRoomList: false);

        UpdatePlayerList();
        GlobalEvents.OnJoinedRoom?.Invoke();
    }

    public override void OnLeftRoom()
    {
        IsBackButtonInteratable(true);
        ShowWindow(isRoomList: true);

        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    #endregion

    #region UpdateMethods

    private void UpdateRoomListData(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            var currentRoom = cashedRoomList.Find(a => a.Name == roomList[i].Name);

            if (currentRoom == null)
            {
                if (roomList[i].PlayerCount != 0)
                {
                    cashedRoomList.Add(roomList[i]);
                }
            }
            else
            {
                if (roomList[i].PlayerCount == 0)
                {
                    cashedRoomList.Remove(currentRoom);
                }
                else
                {
                    int currentRoomIndex = cashedRoomList.IndexOf(currentRoom);

                    if (roomList[i] != null)
                    {
                        cashedRoomList[currentRoomIndex] = roomList[i];
                    }

                }
            }
        }

        for (int i = 0; i < cashedRoomList.Count; i++)
        {
            if (cashedRoomList[i].PlayerCount == 0)
            {
                cashedRoomList.Remove(cashedRoomList[i]);
            }
        }
    }

    private void UpdateRoomListVisual(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomItems.Count; i++)
        {
            Destroy(roomItems[i].gameObject);
        }
        roomItems.Clear();


        for (int i = 0; i < cashedRoomList.Count; i++)
        {
            if (!cashedRoomList[i].IsVisible)
            {
                continue;
            }

            UiRoomItem newRoomItem = Instantiate(uiRoomItemPrefab);
            newRoomItem.lobbyNetworkParent = this;
            newRoomItem.SetName(cashedRoomList[i].Name);
            newRoomItem.transform.SetParent(roomListWindow.transform, false);

            roomItems.Add(newRoomItem);
        }
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            Destroy(playerList[i].gameObject);
        }
        playerList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            UiPlayerItem newPlayerItem = Instantiate(uiPlayerItemPrefab);
            newPlayerItem.SetName(player.Value.NickName);
            newPlayerItem.transform.SetParent(playerListWindow.transform, false);

            playerList.Add(newPlayerItem);
        }
    }

    #endregion

    private void IsBackButtonInteratable(bool isBackButtonInteracteble)
    {
        GlobalEvents.IsBackButtonInteratable?.Invoke(isBackButtonInteracteble);
    }

    private void ShowWindow(bool isRoomList)
    {
        roomListWindow.SetActive(isRoomList);
        playerListWindow.SetActive(!isRoomList);
    }
}