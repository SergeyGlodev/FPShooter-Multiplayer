using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuTheme;
    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private float delayBetweenThemes = 1f;

    private bool isMenu = true;
    private bool isGameplay = false;
    private string sceneName;


    private void Start()
    {
        GlobalEvents.OnStartGamePressed += OnGame;
        GlobalEvents.OnLeaveGame += OnMenu;
        PlayMusic();
    }

    private void OnMenu()
    {
        isMenu = true;
        isGameplay = false;
        PlayMusic();
    }
    
    private void OnGame()
    {
        isMenu = false;
        isGameplay = true;
        PlayMusic();
    }

    private void PlayMusic()
    {
        AudioClip clipToPlay = null;
        if (isMenu)
        {
            clipToPlay = menuTheme;
        }
        else if (isGameplay)
        {
            clipToPlay = mainTheme;
        }

        if (clipToPlay != null)
        {
            AudioManager.Instance.PlayMusic(clipToPlay, delayBetweenThemes);
            Invoke(nameof(PlayMusic), clipToPlay.length);
        }
    }
}
