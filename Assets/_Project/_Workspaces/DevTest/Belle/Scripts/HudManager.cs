using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI _timer;
    private float _currTime;
    private bool _beatLevel;
    [SerializeField] private GameObject _endBoard;
    // Start is called before the first frame update
    void Start()
    {
        _endBoard.SetActive(false);  
    }

    // Update is called once per frame
    void Update()
    {
        if (!_beatLevel)
        {
            _currTime += Time.deltaTime;
            _timer.text = _currTime.ToString("0.0");
        }
    }

    public void EnableHud()
    {
        if (_endBoard.activeInHierarchy)
        {

        }
        else
        {
            _endBoard.SetActive(true);
        }
    }
    public void BeatLevel()
    {
        _beatLevel = true;
        EnableHud();
    }
    public void ToHubWorld()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
