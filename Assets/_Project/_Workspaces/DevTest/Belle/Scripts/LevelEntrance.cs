using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntrance : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 50.0f;
    private LevelTracker _levelTracker;

    private void Start()
    {
        _levelTracker = FindAnyObjectByType<LevelTracker>();
        if (_levelTracker != null)
        {
            if (_levelTracker.LevelIndex >= 3)
            {
                gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerController playerCon))
        {
            _levelTracker.GoTONextLevel();
        }
    }
}
