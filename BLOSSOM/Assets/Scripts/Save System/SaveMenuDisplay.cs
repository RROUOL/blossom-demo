using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuDisplay : MonoBehaviour
{
    public Button[] nameButton = new Button[3];
    public Text[] nameField = new Text[3];
    public Text[] dateField = new Text[3];
    public Text[] timeField = new Text[3];

    void Awake()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < DataPersistenceManager.numberOfSaves; i += 1)
        {
            GameData gD = DataPersistenceManager.singleton.GetSaveFile(i);

            if (gD != null)
            {
                Debug.Log("save file " + i + " found.");
                nameButton[i].interactable = true;
                nameField[i].text = gD.playerName;

                dateField[i].text = 
                gD.day + "/" + 
                gD.month + "/" + 
                gD.year;

                int num = gD.playTime;

                nameField[i].text = 
                TimeSpan.FromSeconds(num).Hours + ":" +
                TimeSpan.FromSeconds(num).Minutes + ":" + 
                TimeSpan.FromSeconds(num).Seconds;
            }
            else
            {
                Debug.Log("save file " + i + " NOT found.");
                nameButton[i].interactable = true;
            }
        }
    }

    public void SaveChosen(int i)
    {
        Debug.Log("SaveChosen(int i) called");

        GameData gD = DataPersistenceManager.singleton.GetSaveFile(i);

        if (gD != null)
        {
            Debug.Log("save file " + i + " found. Overwriting...");
        }
        else
        {
            Debug.Log("save file " + i + " NOT found.");
        }
        
        DataPersistenceManager.singleton.SaveGame(i);
    }
}
