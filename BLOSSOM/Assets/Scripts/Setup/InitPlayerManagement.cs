using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class InitPlayerManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.singleton.canDungeonPause = true;
        GameManager.singleton.SearchForPlayer();
    }
}
