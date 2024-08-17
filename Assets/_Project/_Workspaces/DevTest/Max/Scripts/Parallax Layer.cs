using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    private float _startPosition;
    public GameObject _camera; 

    /*
        Set the strength of the parallax effect.
        Strength < 0 for foreground layer
        Strength = 0 for middle layer
        Strength > 0 for background layer
        strength = 1 for non-moving background layer (ie the sun)
    */
    [SerializeField] private float _effectStrength;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float newPosition = _startPosition + _camera.transform.position.x * _effectStrength;
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
    }
}
