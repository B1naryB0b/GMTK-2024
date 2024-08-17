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
    private DialougeManager _diaManager;

    // Start is called before the first frame update
    void Start()
    {
        _diaManager = FindObjectOfType<DialougeManager>();
    }

    private void OnEnable()
    {
        //get current items from manager
        _charName = _diaManager.GetCurrName();
        _sentances = _diaManager.GetCurrDialouge();
        CharImage = _diaManager.GetCurrSprite();
        DialougeName.text = _charName;
        //play animation
        DialougeAnimator.SetTrigger("Enter");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        _index++;
    }
}
