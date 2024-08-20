using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletCollector : MonoBehaviour
{
    private List<Rigidbody2D> _rigidbody2Ds = new List<Rigidbody2D>(); 
    private ParticleSystem _particleSystem; 

    [SerializeField] private float pullStrength;
    [SerializeField] private AudioClip collectDropletSfx;

    private FluidManager _fluidManager;
    private DropletManager _dropletManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _fluidManager = FindObjectOfType<FluidManager>();
        _dropletManager = GetComponent<DropletManager>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rb in _rigidbody2Ds)
        {
            if (rb.gameObject.TryGetComponent(out Droplet droplet) && !droplet.isBeingEjected)
            {
                Vector2 dir = (transform.position - rb.gameObject.transform.position).normalized;
                rb.velocity = dir * pullStrength * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Droplet droplet))
        {
            Rigidbody2D rb = droplet.gameObject.GetComponent<Rigidbody2D>();
            _rigidbody2Ds.Add(rb);
            
            _fluidManager.AddDroplet(droplet);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Droplet droplet))
        {
            Rigidbody2D rb = droplet.gameObject.GetComponent<Rigidbody2D>();
            _rigidbody2Ds.Remove(rb);
            
            _fluidManager.RemoveDroplet(droplet);
        }
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Droplet droplet) && !droplet.isBeingEjected)
        {
            Vector3 contactPoint = other.GetContact(0).point;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(contactPoint.x - transform.position.x, contactPoint.y - transform.position.y);
            Debug.Log(angle);
            _particleSystem.transform.rotation = Quaternion.Euler(0, 0, angle);
            _particleSystem.Play();
            
            _dropletManager.AddMass(droplet.mass);
            Rigidbody2D rb = droplet.gameObject.GetComponent<Rigidbody2D>();
            _rigidbody2Ds.Remove(rb);
            Destroy(other.gameObject);
            
            AudioController.Instance.PlaySound(collectDropletSfx, 0.5f);
        }
    }
}
