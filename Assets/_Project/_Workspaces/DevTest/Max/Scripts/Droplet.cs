using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droplet : MonoBehaviour
{
    public float mass = 1;
    public bool isBeingEjected = true;
    [SerializeField] private float ejectionDuration = 2.0f;

    private Collider2D _col;
    
    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.enabled = false;
        StartCoroutine(EjectionTimer());
    }

    private IEnumerator EjectionTimer()
    {
        yield return new WaitForSeconds(ejectionDuration);

        isBeingEjected = false;
        _col.enabled = true;
    }

    /*private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player")) {
            // TODO : add mass to player
            Destroy(gameObject);
        }
    }*/

}
