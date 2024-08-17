using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Npc : MonoBehaviour
{
    public string Name;
    private CapsuleCollider2D triggerCol;
    private DialougeManager _diaManager;
    private bool _canTalk;
    // Start is called before the first frame update
    void Start()
    {
        triggerCol = GetComponent<CapsuleCollider2D>();
        _diaManager = FindObjectOfType<DialougeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && _canTalk)
        {
            _diaManager.SpawnNewBox(Name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canTalk = false;
        }
    }
}
