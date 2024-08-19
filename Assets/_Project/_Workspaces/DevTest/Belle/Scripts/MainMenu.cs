using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioManager _audioMan;
    // Start is called before the first frame update
    public void Start()
    {
        _audioMan.PlaySound();
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
