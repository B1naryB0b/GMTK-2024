using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletManager : MonoBehaviour
{

    [SerializeField] private GameObject dropletPrefab;
    [SerializeField] private float ejectionForce;
    
    [SerializeField] private float mass = 1f;

    private void Start()
    {
        ScalePlayer();
    }

    public void AddMass(float addMass)
    {
        mass += addMass;
        mass = Mathf.Min(mass, 2f);
        float visualScalingFactor = Mathf.Sqrt(mass / 2f) + (mass / 2f) + 0.2f;
        Vector3 playerScale = Vector3.one * visualScalingFactor;
        Debug.Log(visualScalingFactor);
        transform.localScale = playerScale;
        ScalePlayer(); 
    }
    
    public void SubtractMass(Vector2 direction)
    {
        GameObject dropletObj = Instantiate(dropletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = dropletObj.GetComponent<Rigidbody2D>();
        rb.AddForce(direction.normalized * ejectionForce, ForceMode2D.Impulse);
        mass -= dropletObj.GetComponent<Droplet>().mass;
        mass = Mathf.Max(mass, 0f);
        ScalePlayer();
    }

    private void ScalePlayer()
    {
        float visualScalingFactor = Mathf.Sqrt(mass / 2f) + (mass / 3f) + 0.4f;
        Vector3 playerScale = Vector3.one * visualScalingFactor;
        Debug.Log(visualScalingFactor);
        transform.localScale = playerScale;
    }

    public float GetMass()
    {
        return mass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
