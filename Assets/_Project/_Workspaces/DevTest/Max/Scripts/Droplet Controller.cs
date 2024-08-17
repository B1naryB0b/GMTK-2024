using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    [SerializeField] private float _attractionRadius;
    [SerializeField] private float _attractionSpeed;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform transform in transform) 
        {
            if (IsClose(transform)) 
            {
                float xManDist = player.transform.position.x - transform.position.x;
                float xDist = xManDist * xManDist;
                float yManDist = player.transform.position.y - transform.position.y;
                float yDist = yManDist * yManDist;
                float dist = Mathf.Sqrt(xDist + yDist);
                if (dist > _attractionRadius) 
                {
                    continue;
                }

    	        GameObject droplet = transform.gameObject;
                Rigidbody2D rb = droplet.GetComponent<Rigidbody2D>();
                Transform tf = droplet.GetComponent<Transform>();
                droplet.GetComponent<Transform>();
                rb.velocity = new Vector3(
                    xManDist * (_attractionSpeed / dist) * (_attractionSpeed / dist), 
                    yManDist * (_attractionSpeed / dist) * (_attractionSpeed / dist), 
                    0);

                tf.Rotate(rb.velocity);
            }
        }
    }

    private bool IsClose(Transform transform) {
        return Mathf.Abs(player.transform.position.x - transform.position.x) < _attractionRadius
            && Mathf.Abs(player.transform.position.y - transform.position.y) < _attractionRadius;
    }
}