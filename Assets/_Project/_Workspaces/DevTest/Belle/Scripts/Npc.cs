using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Npc : MonoBehaviour
{
    public string Name;
    private CapsuleCollider2D triggerCol;
    // Start is called before the first frame update
    void Start()
    {
        triggerCol = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
