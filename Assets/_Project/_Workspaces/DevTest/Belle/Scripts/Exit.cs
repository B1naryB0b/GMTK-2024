using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private LevelManager _levelMan;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_levelMan.HasCollectedObjective)
        {
            _levelMan.EndLevel();
        }
    }
}
