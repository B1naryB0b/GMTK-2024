using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] private AudioSource musicSource, sfxSource;

    private GameObject _sfxGameObject;

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

    public AudioController GetAudioControllerInstance()
    {
        return Instance;
    }
}
