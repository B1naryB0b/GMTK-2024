using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DialougeManager : MonoBehaviour
{
    private int LevelIndex;
    [SerializeField] private GameObject _dialougeBox;
    //[SerializeField] private Transform _dialougeStartPoint;
    private string[][] _rakeyDialouge;
    private string[][] _clipyDialouge;
    private string[][] _sprinkleyDialouge;
    public TextAsset RakeyText;
    public TextAsset ClipyText;
    public TextAsset SprinkleyText;
    [SerializeField] private Sprite _rakeyDiaSprite;
    [SerializeField] private Sprite _clipyDiaSprite;
    [SerializeField] private Sprite _sprinkleyDiaSprite;
    private string[] _currDialouge;
    private string _currName = "";
    [SerializeField] private Image _currSprite;
    private LevelTracker _levelTrack;
    
    // Start is called before the first frame update
    void Start()
    {
        _levelTrack = FindObjectOfType<LevelTracker>();
        if (_levelTrack != null)
        {
            LevelIndex = _levelTrack.LevelIndex;
            LevelIndex -= 1;
        }
        DestroyCurrBox();
        ReadTextFiles();
    }
    public void SetCurrDialouges(string name)
    {
        switch (name)
        {
            case "Rakey":
                _currName = name;
                _currSprite.sprite = _rakeyDiaSprite;
                _currDialouge = _rakeyDialouge[LevelIndex];
                break;
            case "Clipy":
                _currName = name;
                _currSprite.sprite = _clipyDiaSprite;
                _currDialouge = _clipyDialouge[LevelIndex];
                break;
            case "Sprinkley":
                _currName = name;
                _currSprite.sprite = _sprinkleyDiaSprite;
                _currDialouge = _sprinkleyDialouge[LevelIndex];
                break;
        }
    }

    public string[] GetCurrDialouge()
    {
        return _currDialouge;
    }
    public string GetCurrName()
    {
        return _currName;
    }

    public Image GetCurrSprite()
    {
        return _currSprite;
    }

    public void SpawnNewBox(string name)
    {
        if (_dialougeBox.activeInHierarchy)
        {
        }
        else
        {
            SetCurrDialouges(name);
            //_currBox.transform.position = _dialougeStartPoint.position;
            _dialougeBox.SetActive(true);
        }
    }
    public void DestroyCurrBox()
    {
        _dialougeBox.SetActive(false);
    }

    private void ReadTextFiles()
    {
        //rakey
        string[] lines = RakeyText.text.Split('\n');
        _rakeyDialouge = new string[lines.Length][];
        for(int i = 0; i<lines.Length; i++)
        {
            _rakeyDialouge[i] = lines[i].Split(',');
        }
        //clipy
        lines = ClipyText.text.Split('\n');
        _clipyDialouge = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            _clipyDialouge[i] = lines[i].Split(',');
        }
        //sprinkley
        lines = SprinkleyText.text.Split('\n');
        _sprinkleyDialouge = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            _sprinkleyDialouge[i] = lines[i].Split(',');
        }
    }
}
