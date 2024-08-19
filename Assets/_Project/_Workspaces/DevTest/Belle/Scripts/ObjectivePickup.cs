using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePickup : MonoBehaviour
{
    [SerializeField] private LevelManager _levelMan;
    [SerializeField] private float _rotationSpeed = 50.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            _levelMan.CollectedObjective();
            //maybe play animation
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }
}
