using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vaz.ManagerSingletons;

public class WarningDisplay : MonoBehaviour
{
    public CanvasGroup[] displayGroups = new CanvasGroup[2];
    CanvasGroup fadeGroup;

    static float displayTime = 10.0f;
    float currentTime = 0.0f;
    int fadeInSpeed = 2;
    int fadeOutSpeed = 4;

    bool isFadingIn = true;

    int currentGroup = 0;

    void Start()
    {
        for (int i = 0; i < displayGroups.Length; i += 1)
        {
            displayGroups[i].alpha = 0;
        }
        
        fadeGroup = GameManager.singleton.GetFadeBlack();
        fadeGroup.alpha = 0;
    }

    void Update()
    {
        if (cInput.GetKeyDown("Submit"))
        {
            currentTime += displayTime;
            displayGroups[currentGroup].alpha -= 0.75f;
            isFadingIn = false;
        }

        if (isFadingIn)
        {
            displayGroups[currentGroup].alpha += Time.deltaTime / fadeInSpeed;

            if (displayGroups[currentGroup].alpha == 1)
            {
                currentTime += Time.deltaTime;

                if (displayTime < currentTime)
                {
                    isFadingIn = false;
                    currentTime = 0.0f;
                }
            }
        }
        else
        {
            displayGroups[currentGroup].alpha -= Time.deltaTime * fadeOutSpeed;

            if (displayGroups[currentGroup].alpha == 0)
            {
                isFadingIn = true;
                
                currentGroup += 1;

                if (currentGroup > displayGroups.Length-1)
                {
                    SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
                }
            }
        }
    }
}