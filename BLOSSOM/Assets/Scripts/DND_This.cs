using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DND_This : MonoBehaviour
{ // generic DontDestroyOnLoad script, use on anything I want to carry between scenes
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
