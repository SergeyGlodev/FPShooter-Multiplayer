using System;

public static class GlobalEvents
{
    public static Action OnStartGamePressed;
    public static Action OnGameStarted;
    public static Action OnDead;
    public static Action OnGameRestarted;
    public static Action OnLobbyLoaded;
    public static Action <string> OnCreateRoom;
    public static Action OnJoinedRoom;
    public static Action <string> OnJoinButtonPressed;
    public static Action OnLeaveRoom;
    public static Action<bool> IsBackButtonInteratable;
    public static Action OnLeaveGame;
    public static Action OnInGameplaySettingsOpen;
    public static Action OnInGameplaySettingsClose;
    public static Action OnMovementTouchOverButton;
    public static Action OnLookTouchOverButton;
    public static Action OnHitShootEffect;
    public static Action <FPSController> OnPlayerSet;
}