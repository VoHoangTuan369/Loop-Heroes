using UnityEngine;

public class SoundMN : MonoBehaviour
{
    public static SoundMN Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip walkSound;
    public AudioClip shootSound;
    public AudioClip enemyHitSound;
    public AudioClip baseHitSound;
    public AudioClip enemySound;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayWalk() => Play(walkSound);
    public void PlayShoot() => Play(shootSound);
    public void PlayEnemyHit() => Play(enemyHitSound);
    public void PlayBaseHit() => Play(baseHitSound);
    public void PlayWinSound() => Play(winSound);
    public void PlayLoseSound() => Play(loseSound);
    public void PlayEnemySound() => Play(enemySound);

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }

    public void ToggleSound(bool isOn)
    {
        float musicVol = isOn ? 0.4f : 0f;
        float sfxVol = isOn ? 1f : 0f;

        if (musicSource != null)
            musicSource.volume = musicVol;

        if (sfxSource != null)
            sfxSource.volume = sfxVol;
    }
}
