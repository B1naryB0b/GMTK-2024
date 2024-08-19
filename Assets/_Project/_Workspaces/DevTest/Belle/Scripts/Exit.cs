using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private LevelManager _levelMan;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerCon))
        {
            if (_levelMan.HasCollectedObjective)
            {
                _levelMan.EndLevel();
            }
        }
    }
}
