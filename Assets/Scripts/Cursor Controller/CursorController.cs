using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Start()
    {
        GlobalEvents.OnStartGamePressed += LockCursor;
        GlobalEvents.OnInGameplaySettingsClose += LockCursor;
        GlobalEvents.OnGameRestarted += LockCursor;

        GlobalEvents.OnInGameplaySettingsOpen += UnlockCursor;
        GlobalEvents.OnDead += UnlockCursor;
        GlobalEvents.OnLeaveGame += UnlockCursor;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }  
}
