using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonsai : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bonsaiRender;
    [SerializeField] private Sprite[] _bonsaiSprites;
    [SerializeField] private float[] _yCords;
    private LevelTracker _levelTracker;
    // Start is called before the first frame update
    void Start()
    {
        _levelTracker = FindObjectOfType<LevelTracker>();
        if (_levelTracker != null)
        {
            _bonsaiRender.sprite = _bonsaiSprites[_levelTracker.LevelIndex - 1];
            transform.position = new Vector3(transform.position.x, _yCords[_levelTracker.LevelIndex - 1], transform.position.z);
        }
        else
        {
            _bonsaiRender.sprite = _bonsaiSprites[0];
            transform.position = new Vector3(transform.position.x, _yCords[0], transform.position.z);

        }
    }
}
