using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletController : MonoBehaviour
{
    [SerializeField] private float _attractionRadius;
    //private ArrayList<GameObject> droplets;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform droplet in transform) 
        {
            if (IsClose(droplet)) 
            {
                float xManDist = player.transform.position.x - droplet.position.x;
                float xDist = xManDist * xManDist;
                float yManDist = player.transform.position.y - droplet.position.y;
                float yDist = yManDist * yManDist;
                float dist = Mathf.Sqrt(xDist + yDist);

                if (dist > _attractionRadius) 
                {
                    continue;
                }

                // Move to player
            }
        }
    }

    private bool IsClose(Transform droplet) {
        return Mathf.Abs(player.transform.position.x - droplet.position.x) < _attractionRadius
            && Mathf.Abs(player.transform.position.y - droplet.position.y) < _attractionRadius;
    }
}
