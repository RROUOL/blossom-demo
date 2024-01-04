using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vaz.ManagerSingletons;

public class StartMenuFadeIn : MonoBehaviour
{
    bool finishRunning = false;
    CanvasGroup fadeGroup;
    [Space]
    public CanvasGroup logoGroup;
    public CanvasGroup buttonGroup;
    public GameObject newGameButton;
    [Space]
    static float waitTime = 3.0f;
    float currentTime = 0.0f;

    void Awake()
    {
        fadeGroup = GameManager.singleton.GetFadeBlack();
        fadeGroup.alpha = 1;
        buttonGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (finishRunning) 
        {
            //Debug.Log("finished running");
            return;
        }

        // fadeGroup is fading to 0 because of StartMenuButton.cs line 55

        if (fadeGroup.alpha == 0)
        {
            currentTime += Time.deltaTime;
            
            if (currentTime > waitTime)
            {
                logoGroup.alpha += Time.deltaTime;
            }
        }

        if (logoGroup.alpha == 1 && currentTime > (waitTime * 2))
        {
            buttonGroup.alpha += Time.deltaTime;
        }


        if (buttonGroup.alpha == 1)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(newGameButton);
            }
            Debug.Log(EventSystem.current.currentSelectedGameObject);
            finishRunning = true;
        }
        
        if (cInput.GetKey("Submit"))
        {
            logoGroup.alpha = 1;
            buttonGroup.alpha = 1;
            EventSystem.current.SetSelectedGameObject(newGameButton);
        }
    }
}
