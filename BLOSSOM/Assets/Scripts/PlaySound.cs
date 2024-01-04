using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class PlaySound : MonoBehaviour
{
    public void PlayOneShot(int soundIndex)
    {
        // 1 - select ding
        // 4 - confirm ding
        GameManager.singleton.PlaySound(soundIndex);
    }
}
