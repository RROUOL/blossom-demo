using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class StartMenuButtons : MonoBehaviour
{
    [SerializeField] Button NewGameButton;
    [SerializeField] Button LoadGameButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button QuitButton;
    [Space]
    [SerializeField] GameObject UI_Menu;

    GameObject canvasObj;
    CanvasGroup canvasGroup;

    bool isFading = false;
    bool finishedFading = false;

    void Awake()
    {
        canvasGroup = GameManager.singleton.GetFadeBlack();

        // if no save file found.. disable load button
        bool saveFileFound = false;

        for (int i = 0; i < DataPersistenceManager.numberOfSaves; i += 1)
        {
            GameData gD = DataPersistenceManager.singleton.GetSaveFile(i);

            if (gD != null)
            {
                Debug.Log("save file " + i + " found");
                saveFileFound = true;
            }
        }
        
        if (!saveFileFound)
        {
            Debug.Log("Load button shut off because all save files are empty.");
            LoadGameButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFading) 
        {
            //Debug.Log("is not fading");
            canvasGroup.alpha -= Time.deltaTime / 2;
            return;
        }
        else
        {
            UI_Menu.SetActive(false);
            //Debug.Log("It has begun.");
        }

        //Debug.Log("before: " + canvasGroup.alpha);

        canvasGroup.alpha += Time.deltaTime / 2;

        //Debug.Log("after: " + canvasGroup.alpha);

        if (canvasGroup.alpha == 1)
        {
            canvasGroup.alpha = 0;
            finishedFading = true;
            isFading = false;
            LoadScene();
        }
    }

    void LoadScene()
    {
        GameManager.singleton.EnterLastActiveSingle();
    }

    public void NewGame()
    {
        isFading = true;
        GameManager.singleton.SetLastActive("DemoScene");
    }

    void LoadGame()
    {   // pt.2 - load specified save file
        isFading = true;
    }

    public void QuitGame()
    {
        Debug.Log("Application Closed");
        Application.Quit();
    }
}