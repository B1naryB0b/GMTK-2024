using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialougeInstance : MonoBehaviour
{
    public TextMeshProUGUI DialougeName;
    public TextMeshProUGUI DialougeText;
    public Image CharImage;
    private string _charName;
    private string[] _sentances;
    private int _index = 0;
    [SerializeField] private float _dialougeSpeed;
    public Animator DialougeAnimator;
    [SerializeField] private DialougeManager _diaManager;
    private bool firstEnable = true;
    private InputHandler _inputHandler;
    private bool _finishedWrighting;

    // Start is called before the first frame update
    void Start()
    {
        _diaManager = FindObjectOfType<DialougeManager>();
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    private void OnEnable()
    {
        if (firstEnable)
        {
            firstEnable = false;
        }
        else
        {
            //get current items from manager
            _charName = _diaManager.GetCurrName();
            _sentances = _diaManager.GetCurrDialouge();
            CharImage = _diaManager.GetCurrSprite();
            DialougeName.text = _charName;
            //play animation
            DialougeAnimator.SetTrigger("Enter");
            NextSentance();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputHandler._inputActions.Player.Interact.WasPressedThisFrame() && _finishedWrighting == true)
        {
            _finishedWrighting = false;
            NextSentance();
        }
    }
    private void NextSentance()
    {
        if(_index <= _sentances.Length -1)
        {
            DialougeText.text = "";
            StartCoroutine(WriteDialouge());
        }
        else
        {
            DialougeText.text = "";
            DialougeAnimator.SetTrigger("Exit");
            _index = 0;
            //get dialouge manager to destroy prefab this is attatched to. event thats called.
            _diaManager.DestroyCurrBox();
        }
    }

    IEnumerator WriteDialouge()
    {
        foreach(char character in _sentances[_index].ToCharArray())
        {
            DialougeText.text += character;
            yield return new WaitForSeconds(_dialougeSpeed);
        }
        _finishedWrighting = true;
        _index++;
    }
}
