
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;


    [Header("UI SFX")]
    public AudioClip ButtonClick;

    [Header("Game SFX")]
    public AudioClip SelfMove;
    public AudioClip OpponentMove;
    public AudioClip Castling;
    public AudioClip Capture;
    public AudioClip Check;
    public AudioClip Illegal;
    public AudioClip GameStart;
    public AudioClip GameEnd;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Assigned to slider in the Inspector
    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("Music", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }

    // Assigned to slider in the Inspector
    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }

    public void PlayButtonClick() => PlaySFX(ButtonClick);
}