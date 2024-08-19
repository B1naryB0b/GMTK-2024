using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] private AudioSource musicSource, sfxSource;

    private GameObject _sfxGameObject;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Start()
    {
        _sfxGameObject = sfxSource.gameObject;
    }

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
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null) { sfxSource = _sfxGameObject.GetComponent<AudioSource>(); }

        if (clip == null)
        {
            Debug.LogError("AudioClip is null!");
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        sfxSource.volume = value;
    }

    public void FadeInAndOut(AudioClip nextTrack)
    {
        StartCoroutine(FadeOutIn(nextTrack));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public AudioController GetAudioControllerInstance()
    {
        return Instance;
    }

    private IEnumerator FadeOutIn(AudioClip nextTrack)
    {
        yield return FadeOutCoroutine();
        musicSource.clip = nextTrack;
        musicSource.Play();
        yield return FadeInCoroutine();
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();
    }

    private IEnumerator FadeInCoroutine()
    {
        float startVolume = 0.0f;
        musicSource.volume = startVolume;
        musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 1.0f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1.0f;
    }
}
