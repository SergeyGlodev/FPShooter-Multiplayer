using UnityEngine;
using UnityEngine.UI;

public class UiSettings : UiMenu
{
    [SerializeField] private Slider[] volumeSliders;
    [SerializeField] private Slider sensitivityX;
    [SerializeField] private Slider sensitivityY;
    [SerializeField] Toggle[] resolutionToggles;
    [SerializeField] int[] screenWidth;
    [SerializeField] Toggle fullScreen;
    [SerializeField] GameObject settingsToggle;

    public Button back;

    public int ActiveScreenResIndex => activeScreenResIndex;

    private int activeScreenResIndex;

    private void Start()
    {
        volumeSliders[0].value = AudioManager.Instance.MasterVolumePercent;
        volumeSliders[0].onValueChanged.AddListener(SetMasterVolume);
        volumeSliders[1].value = AudioManager.Instance.SfxVolumePercent;
        volumeSliders[1].onValueChanged.AddListener(SetSfxVolume);
        volumeSliders[2].value = AudioManager.Instance.MusicVolumePercent;
        volumeSliders[2].onValueChanged.AddListener(SetMusicVolume);
        sensitivityX.value = InputManager.Instance.input.SensitivityX;
        sensitivityX.onValueChanged.AddListener(SetSensitivityX);
        sensitivityY.value = InputManager.Instance.input.SensitivityY;
        sensitivityY.onValueChanged.AddListener(SetSensitivityY);

    #if !UNITY_ANDROID && !UNITY_IOS
        SetResolutionIndex();
        bool isFullScreen = (PlayerPrefs.GetInt("Fullscreen") == 1) ? true : false;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            int j = i;
            resolutionToggles[j].onValueChanged.AddListener((value) => SetScreenResolution(j));
        }
        fullScreen.onValueChanged.AddListener(SetFullScreen);

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }

        SetFullScreen(isFullScreen);
        fullScreen.isOn = isFullScreen;

        settingsToggle.SetActive(true);
        fullScreen.gameObject.SetActive(true);
    #endif
    }

    public void SetResolutionIndex() => activeScreenResIndex = PlayerPrefs.GetInt("Screen Resolution Index");

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16f / 9f;
            Screen.SetResolution(screenWidth[i], (int)(screenWidth[i] / aspectRatio) + 1, false);

            PlayerPrefs.SetInt("Screen Resolution Index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void GetSensitivity() => InputManager.Instance.input.GetSensitivity();

    private void SetMasterVolume(float value) => AudioManager.Instance.SetVolume(value, AudioChannel.Master);
    
    private void SetMusicVolume(float value) => AudioManager.Instance.SetVolume(value, AudioChannel.Music);
    
    private void SetSfxVolume(float value) => AudioManager.Instance.SetVolume(value, AudioChannel.Sfx);
    
    private void SetSensitivityX(float value) => InputManager.Instance.input.SetSensitivityX(value);
    
    private void SetSensitivityY(float value) => InputManager.Instance.input.SetSensitivityY(value);

    private void SetFullScreen(bool isFullScreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullScreen;
        }

        if (isFullScreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("Fullscreen", ((isFullScreen) ? 1 : 0));
        PlayerPrefs.Save();
    }
}