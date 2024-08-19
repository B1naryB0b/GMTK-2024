using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private bool _hasCollectedObjective = false;
    public bool HasCollectedObjective { get { return _hasCollectedObjective; } }
    [SerializeField] private HudManager _hudMan;
    [SerializeField] private PlayerController _playerCon;
    private LevelTracker _levelTrack;

    public void CollectedObjective()
    {
        _hasCollectedObjective = true;
    }
    public void EndLevel()
    {
        //disable player
        _playerCon.enabled = false;
        _levelTrack = FindObjectOfType<LevelTracker>();
        if (_levelTrack != null)
        {
            _levelTrack.IncIndex();
        }
        _hudMan.BeatLevel();
        //animateHud

    }
}
