using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI _timer;
    private float _currTime;
    public bool BeatLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatLevel)
        {
            _currTime += Time.deltaTime;
            _timer.text = _currTime.ToString("0.0");
        }
    }

    public void ToHubWorld()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
