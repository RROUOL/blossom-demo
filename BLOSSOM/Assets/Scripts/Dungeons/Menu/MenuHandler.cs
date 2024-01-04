using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vaz.ManagerSingletons;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {

    }

    public void PlaySound(AudioClip audioClip)
    {
        GameManager.singleton.PlaySound(audioClip);
    }

    // Update is called once per frame
    public void SetSelectedGameObject(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
