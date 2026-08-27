
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX")]
    // Board
    public AudioClip sfxSelfMove;
    public AudioClip sfxOpponentMove;
    public AudioClip sfxCastling;
    public AudioClip sfxCapture;
    public AudioClip sfxCheck;
    public AudioClip sfxIllegal;
    public AudioClip sfxGameStart;
    public AudioClip sfxGameEnd;

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
        musicSource.volume = value;
    }

    // Assigned to slider in the Inspector
    public void SetSfxVolume(float value)
    {
        sfxSource.volume = value;
    }
}