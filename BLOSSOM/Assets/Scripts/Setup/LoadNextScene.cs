using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    void LateUpdate()
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("LOADING INTO NEXT SCENE");
    }
}
