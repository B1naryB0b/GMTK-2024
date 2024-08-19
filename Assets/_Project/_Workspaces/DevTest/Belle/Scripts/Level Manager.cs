using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private bool _hasCollectedObjective = false;
    public bool HasCollectedObjective { get { return _hasCollectedObjective; } }
    [SerializeField] private HudManager _hudMan;
    [SerializeField] private PlayerController _playerCon;

    public void CollectedObjective()
    {
        _hasCollectedObjective = true;
    }
    public void EndLevel()
    {
        //disable player
        _playerCon.enabled = false;
        _hudMan.BeatLevel();
        //animateHud

    }
}
