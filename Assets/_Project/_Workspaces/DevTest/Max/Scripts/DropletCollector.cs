using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletCollector : MonoBehaviour
{

    private Transform _transform;
    private List<Rigidbody2D> _rigidbody2Ds = new List<Rigidbody2D>();
    public ParticleSystem particleSystem; 

    [SerializeField] private float pullStrength;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rb in _rigidbody2Ds)
        {
            Vector2 dir = (transform.position - rb.gameObject.transform.position).normalized;
            rb.AddForce(dir * pullStrength * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Droplet droplet))
        {
            Rigidbody2D rb = droplet.gameObject.GetComponent<Rigidbody2D>();
            _rigidbody2Ds.Add(rb);
        }
    }
    
    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.CompareTag("Droplet")) 
        {
            Vector3 contactPoint = other.GetContact(0).point;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(contactPoint.x - _transform.position.x, contactPoint.y - _transform.position.y);
            Debug.Log(angle);
            particleSystem.transform.rotation = Quaternion.Euler(0, 0, angle);
            particleSystem.Play();

            _rigidbody2Ds.Remove(other.gameObject.GetComponent<Rigidbody2D>());
            Destroy(other.gameObject);
        }
    }
}
