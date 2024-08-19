using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DONotDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
