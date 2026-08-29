
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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
        musicSource.volume = value;
    }

    // Assigned to slider in the Inspector
    public void SetSfxVolume(float value)
    {
        sfxSource.volume = value;
    }
}