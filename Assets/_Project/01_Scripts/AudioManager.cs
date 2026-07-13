using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Clips")]
    [SerializeField] private AudioClip connectClip;
    [SerializeField] private AudioClip submitClip;
    [SerializeField] private AudioClip newOrderClip;
    [SerializeField] private AudioClip endScreenClip;
    [SerializeField] private AudioClip backgroundMusic;

    void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayConnectSound()
    {
        PlaySfx(connectClip);
    }

    public void PlaySubmitSound()
    {
        PlaySfx(submitClip);
    }

    public void PlayNewOrderSound()
    {
        PlaySfx(newOrderClip);
    }

    public void PlayEndScreenSound()
    {
        PlaySfx(endScreenClip);
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    void PlayMusic()
    {
        if (backgroundMusic == null || musicSource == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}