using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droplet : MonoBehaviour
{
    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player")) {
            // TODO : add mass to player
            Destroy(gameObject);
        }
    }

}
