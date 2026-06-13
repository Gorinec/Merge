using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Звуки")]
    public AudioClip mergeSound;
    public AudioClip albumAddSound;
    public AudioClip albumOpenSound;

    [Header("Фоновая музыка")]
    public List<AudioClip> backgroundMusic;
    private int currentTrack = 0;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false;
        musicSource.volume = 0.3f;

        PlayNextTrack();
    }

    private void Update()
    {
        if (!musicSource.isPlaying && backgroundMusic.Count > 0)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        if (backgroundMusic.Count == 0) return;

        // Выбираем случайный трек, не повторяя предыдущий
        int newTrack;
        do
        {
            newTrack = Random.Range(0, backgroundMusic.Count);
        } while (newTrack == currentTrack && backgroundMusic.Count > 1);

        currentTrack = newTrack;
        musicSource.clip = backgroundMusic[currentTrack];
        musicSource.Play();
    }



    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    private float sfxVolume = 0.7f;

    // В PlayMerge, PlayAlbumAdd, PlayAlbumOpen заменить:
    public void PlayMerge()
    {
        sfxSource.PlayOneShot(mergeSound, sfxVolume);
    }

    public void PlayAlbumAdd()
    {
        sfxSource.PlayOneShot(albumAddSound, sfxVolume);
    }

    public void PlayAlbumOpen()
    {
        sfxSource.PlayOneShot(albumOpenSound, sfxVolume);
    }
}