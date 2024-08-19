using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePickup : MonoBehaviour
{
    [SerializeField] private LevelManager _levelMan;
    [SerializeField] private float _rotationSpeed = 50.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerCon))
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
