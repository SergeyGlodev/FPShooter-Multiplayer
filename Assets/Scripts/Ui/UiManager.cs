 using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private GameObject uiLobby;
    [SerializeField] private UiMenu uiLoading;
    [SerializeField] private UiGameplay uiGameplay;
    [SerializeField] private UiMainMenu uiMainMenu;
    [SerializeField] private UiCreateRoom uiCreateRoom;
    [SerializeField] private UiJoinRoom uiJoinRoom;
    [SerializeField] private UiSettings uiSettings;
    [SerializeField] private TextMeshProUGUI startGameNumbers;
    [SerializeField] private TMP_InputField createInputField;
    [SerializeField] private TMP_InputField joinInputField;
    [SerializeField] private TextMeshProUGUI statusField;
    [SerializeField] private Button createRoom;
    [SerializeField] private Button joinRoom;
    [SerializeField] private Button backRoom;
    [SerializeField] private Button leaveRoom;
    [SerializeField] private Button startGame;
    [SerializeField] private Button leaveGame;

    public UiGameplay UiGameplay => uiGameplay;
    public bool IsSettingsOpen => isSettingsOpen;

    private UiMenu currentUiMenu;
    private PhotonView view;
    private bool isSettingsOpen;


    private void Start()
    {
        view = GetComponent<PhotonView>();
        uiSettings.SetResolutionIndex();
        uiSettings.SetScreenResolution(uiSettings.ActiveScreenResIndex);

        GlobalEvents.OnLobbyLoaded += OnLobbyLoaded;
        GlobalEvents.OnInGameplaySettingsOpen += OpenInGameplaySettings;
        GlobalEvents.OnInGameplaySettingsClose += CloseInGameplaySettings;
        GlobalEvents.OnJoinedRoom += () => OpenMenu(uiJoinRoom);
        GlobalEvents.OnStartGamePressed += () => OpenMenu(uiGameplay);
        GlobalEvents.OnStartGamePressed += StartGameUiAnimation;
        GlobalEvents.OnGameRestarted += StartGameUiAnimation;
        GlobalEvents.IsBackButtonInteratable += IsBackButtonInteratable;
        GlobalEvents.OnJoinedRoom += OnJoinedRoom;

        createRoom.onClick.AddListener(OnCreateRoom);
        joinRoom.onClick.AddListener(OnJoinButton);
        leaveRoom.onClick.AddListener(OnLeaveRoom);
        leaveGame.onClick.AddListener(OnLeaveGame);
        uiMainMenu.createRoom.onClick.AddListener(() => OpenMenu(uiCreateRoom));
        uiMainMenu.joinRoom.onClick.AddListener(() => OpenMenu(uiJoinRoom));
        uiMainMenu.settings.onClick.AddListener(() => OpenMenu(uiSettings));
        uiCreateRoom.back.onClick.AddListener(() => OpenMenu(uiMainMenu));
        uiJoinRoom.back.onClick.AddListener(() => OpenMenu(uiMainMenu));
        uiJoinRoom.startGame.onClick.AddListener(OnStartPressed);
        uiSettings.back.onClick.AddListener(() => OpenMenu(uiMainMenu));
        uiGameplay.uiSettings.back.onClick.AddListener(OnGameplaySettingsBack);
        uiMainMenu.quit.onClick.AddListener(StartGameUiAnimation);

        if (InputManager.Instance.input is InputKeyboard)
        {
            uiMainMenu.quit.gameObject.SetActive(true);
            uiMainMenu.quit.onClick.AddListener(() => Application.Quit());
        }
        uiSettings.GetSensitivity();

        OpenMenu(uiLoading);
        SetStartStatusField();
        IsBackButtonInteratable(true);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient 
            && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            startGame.interactable = true;
        }
        else
        {
            startGame.interactable = false;
        }
    }

    private void SetStartStatusField()
    {
        statusField.text = "Lobby";
        statusField.color = Color.yellow;
    }

    private void OnCreateRoom()
    {
        GlobalEvents.OnCreateRoom?.Invoke(createInputField.text);
        createInputField.text = null;
    }

    private void OnJoinButton()
    {
        GlobalEvents.OnJoinButtonPressed?.Invoke(joinInputField.text);
    }

    private void OnJoinedRoom()
    {
        statusField.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
        statusField.color = Color.green;
    }

    private void OnLeaveRoom()
    {
        statusField.text = "Lobby";
        statusField.color = Color.yellow;
        GlobalEvents.OnLeaveRoom?.Invoke();
    }

    private void OnLeaveGame()
    {
        GlobalEvents.OnLeaveGame?.Invoke();
        isSettingsOpen = false;
    }

    private void IsBackButtonInteratable(bool isBackButtonInteracteble)
    {
        leaveRoom.interactable = !isBackButtonInteracteble;
        backRoom.interactable = isBackButtonInteracteble;
    }

    private void OnGameplaySettingsBack()
    {
        GlobalEvents.OnInGameplaySettingsClose?.Invoke();
    }

    private void StartGameUiAnimation()
    {
        startGameNumbers.gameObject.SetActive(true);
        int startNumber = 3;
        SetStartNumber();
        startGameNumbers.rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Sequence colorSequence = DOTween.Sequence();
        colorSequence.Append(DOVirtual.Color(Color.white, Color.red, 2f, 
            (value) => startGameNumbers.color = value)
            .SetLoops(3, LoopType.Restart));
        colorSequence.Append(DOVirtual.Color(Color.white, Color.green, 2f, 
            (value) => startGameNumbers.color = value));

        DOVirtual.DelayedCall(2f, () => SetStartNumber())
            .SetLoops(3)
            .OnComplete(() => GameStarted());

        startGameNumbers.transform.DOScale(Vector3.one, 2f)
            .SetLoops(4, LoopType.Restart)
            .OnComplete(() => startGameNumbers.gameObject.SetActive(false));

        void SetStartNumber()
        {
            startGameNumbers.text = startNumber.ToString();
            startNumber -= 1;
        }
    }

    private void GameStarted()
    {
        startGameNumbers.text = "GO!";
        GlobalEvents.OnGameStarted?.Invoke();
    }

    private void OpenInGameplaySettings()
    {
        uiGameplay.inGameplaySettings.SetActive(true);
        isSettingsOpen = true;
    }

    private void CloseInGameplaySettings()
    {
        uiGameplay.inGameplaySettings.SetActive(false);
        isSettingsOpen = false;
    }

    private void OpenMenu(UiMenu targetMenu)
    {
        uiLoading.gameObject.SetActive(false);
        uiGameplay.gameObject.SetActive(false);
        uiMainMenu.gameObject.SetActive(false);
        uiCreateRoom.gameObject.SetActive(false);
        uiJoinRoom.gameObject.SetActive(false);
        uiSettings.gameObject.SetActive(false);
        uiGameplay.inGameplaySettings.SetActive(false);
        if (targetMenu == uiGameplay)
        {
            uiCamera.gameObject.SetActive(false);
        }
        else
        {
            uiCamera.gameObject.SetActive(true);
        }

        currentUiMenu = targetMenu;
        targetMenu.gameObject.SetActive(true);
    }

    private void OnLobbyLoaded()
    {
        if (currentUiMenu != uiJoinRoom)
        {
            OpenMenu(uiMainMenu);
        }
    }

    private void OnStartPressed() => view.RPC(nameof(OnStartPressedRpc), RpcTarget.All);
    
    [PunRPC]
    private void OnStartPressedRpc() => GlobalEvents.OnStartGamePressed?.Invoke();
}