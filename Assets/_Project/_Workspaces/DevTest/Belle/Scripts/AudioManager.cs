using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] Clips;
    [SerializeField] private int _index;
    [SerializeField] private float _volume = 0.5f;
    [SerializeField] private bool _loop;
    // Start is called before the first frame update
    
    public void PlaySound()
    {
        if(_audioSource != null)
        {
            _audioSource.loop = _loop;
            _audioSource.volume = _volume;
            _audioSource.clip = Clips[_index];
            _audioSource.Play();
        }
    }
}
