using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] Clips;
    [SerializeField] private int _index;
    [SerializeField] private float _volume = 0.5f; 
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = FindObjectOfType<AudioSource>();
        if(Clips[_index] != null)
        {
            _audioSource.clip = Clips[_index];
        }
        _audioSource.loop = true;
        _audioSource.volume = _volume;
    }
    
    public void PlaySound()
    {
        if(_audioSource != null && _audioSource.clip != null)
        {
            _audioSource.Play();
        }
    }
}
