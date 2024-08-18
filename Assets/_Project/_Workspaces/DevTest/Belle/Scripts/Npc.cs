using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Npc : MonoBehaviour
{
    public string Name;
    private Collider2D triggerCol;
    private DialougeManager _diaManager;
    private InputHandler _inputHandler;
    private bool _canTalk;
    // Start is called before the first frame update
    void Start()
    {
        triggerCol = GetComponent<Collider2D>();
        _diaManager = FindObjectOfType<DialougeManager>();
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputHandler._inputActions.Player.Interact.WasPressedThisFrame() && _canTalk)
        {
            _diaManager.SpawnNewBox(Name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("i entered" + Name);
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
