using DG.Tweening;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] int sourceObjectAmount;
    [SerializeField] AudioSource AudioSourcePrefab;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] GameObject audioSourceHolder;

    public static AudioManager Instance;
    public static Action<Vector3, SfxClipIndex> OnPointPlay3dSound;
    public static Action<SfxClipIndex> OnPointPlay2dSound;

    public float MasterVolumePercent { get; private set; }
    public float SfxVolumePercent { get; private set; }
    public float MusicVolumePercent { get; private set; }

    private AudioSource[] musicSources;
    private AudioSource[] sfxSourcesObjects;
    private GameObject audioListenerHolder;
    private Transform playerFirstViewTransform;
    private bool musicPlayFirstTime = true;
    private bool inGame;
    private int currentSourceIndex;
    private int activeMusicSourceIndex;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        audioListenerHolder = new GameObject("AudioListenerHolder");
        audioListenerHolder.AddComponent(typeof(AudioListener));
        audioListenerHolder.transform.parent = transform;

        MasterVolumePercent = PlayerPrefs.GetFloat("Master volume", defaultValue: 1);
        SfxVolumePercent = PlayerPrefs.GetFloat("Sfx volume", defaultValue: 1);
        MusicVolumePercent = PlayerPrefs.GetFloat("Music volume", defaultValue: 1);

        musicSources = new AudioSource[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
            musicSources[i].loop = true;
        }
        
        sfxSourcesObjects = new AudioSource[sourceObjectAmount];
        for (int i = 0; i < sfxSourcesObjects.Length; i++)
        {
            sfxSourcesObjects[i] = Instantiate(AudioSourcePrefab);
            sfxSourcesObjects[i].gameObject.transform.parent = audioSourceHolder.transform;
        }

        GlobalEvents.OnPlayerSet += SetPlayer;
        GlobalEvents.OnLeaveGame += SetDefaultPos;
        OnPointPlay3dSound += Play3dSound;
        OnPointPlay2dSound += Play2dSound;
    }

    private void Update()
    {
        if (playerFirstViewTransform != null && inGame)
        {
            audioListenerHolder.transform.position = playerFirstViewTransform.position;
            audioListenerHolder.transform.rotation = playerFirstViewTransform.rotation;
        }
        else if (!inGame)
        {
            audioListenerHolder.transform.position = Vector3.zero;
            audioListenerHolder.transform.rotation = Quaternion.identity;
        }
    }

    public void Play3dSound(Vector3 position, SfxClipIndex index) => 
        PlaySound(position, index);
    
    public void Play2dSound(SfxClipIndex index) => 
        PlaySound(audioListenerHolder.transform.position, index);
    
    public void SetVolume(float volumPercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                MasterVolumePercent = volumPercent;
                break;
            case AudioChannel.Sfx:
                SfxVolumePercent = volumPercent;
                break;
            case AudioChannel.Music:
                MusicVolumePercent = volumPercent;
                break;
        }

        musicSources[activeMusicSourceIndex].volume = MusicVolumePercent * MasterVolumePercent;

        PlayerPrefs.SetFloat("Master volume", MasterVolumePercent);
        PlayerPrefs.SetFloat("Sfx volume", SfxVolumePercent);
        PlayerPrefs.SetFloat("Music volume", MusicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fade)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        if (musicPlayFirstTime)
        {
            musicSources[activeMusicSourceIndex].DOFade(MusicVolumePercent * MasterVolumePercent, 0)
                                                .SetEase(Ease.Linear);

            musicSources[1 - activeMusicSourceIndex].DOFade(0, 0)
                                                    .SetEase(Ease.Linear);
            musicPlayFirstTime = false;
        }
        else
        {
            musicSources[activeMusicSourceIndex].DOFade(MusicVolumePercent * MasterVolumePercent, fade)
                                                .SetEase(Ease.Linear);

            musicSources[1 - activeMusicSourceIndex].DOFade(0, fade)
                                                    .SetEase(Ease.Linear);
        }
    }

    public void SetTargetSfxVolume(AudioSource source)
    {
        source.volume = MusicVolumePercent * MasterVolumePercent;
    }

    private void PlaySound(Vector3 position, SfxClipIndex index)
    {
        sfxSourcesObjects[currentSourceIndex].transform.position = position;
        sfxSourcesObjects[currentSourceIndex].clip = sfxClips[(int)index];
        if (position == audioListenerHolder.transform.position)
        {
            sfxSourcesObjects[currentSourceIndex].spatialBlend = 0f;
        }
        else
        {
            sfxSourcesObjects[currentSourceIndex].spatialBlend = 1f;
        }
        sfxSourcesObjects[currentSourceIndex].PlayOneShot(sfxSourcesObjects[currentSourceIndex].clip,
                                                            SfxVolumePercent * MasterVolumePercent);

        if (currentSourceIndex == sfxSourcesObjects.Length - 1)
        {
            currentSourceIndex = 0;
        }
        else
        {
            currentSourceIndex++;
        }
    }

    private void SetPlayer(FPSController player)
    {
        Instance.playerFirstViewTransform = player.FirstPersonView.transform;
        inGame = true;
    }

    private void SetDefaultPos()
    {
        inGame = false;
    }
}
