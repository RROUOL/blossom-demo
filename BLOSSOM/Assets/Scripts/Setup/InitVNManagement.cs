using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class InitVNManagement : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.singleton.canDungeonPause = false;
        GameManager.singleton.ForgetPlayer();
    }
}
