using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudInstance : MonoBehaviour
{
    public Animator HudAnimator;
    private bool _firstEnabled = true;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (_firstEnabled)
        {
            _firstEnabled = false;
        }
        else
        {
            HudAnimator.SetTrigger("Entry");
        }
    }
}
