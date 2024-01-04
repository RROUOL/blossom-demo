using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class TargetDisplay : MonoBehaviour
{
    [SerializeField] BattleHandler battleHandler;

    public Text[] buttonSet = new Text[11];

    public bool displayReserves; // if true, buttonset elements 0-3 are normal, but 4-10 are reserves instead

    public void DisplayTargets()
    {
        // hitsall display

        if (battleHandler.currentlySelectedMove.hitsAll)
        {
            buttonSet[0].text = "Allies";

            if (!displayReserves)
            {
                buttonSet[4].text = "Enemies";
            }
            else
            {
                buttonSet[4].text = "Reserves";
            }

            return;
        }
        else
        {
            // single hit display

            int currentCount = 0;

            for (int i = 0; i < 4; i++)
            {
                if (GameManager.singleton.playerParty[i] != null)
                {
                    buttonSet[currentCount].text = GameManager.singleton.playerParty[i].seenName;
                }
                else
                {
                    buttonSet[currentCount].text = "-----";
                }

                currentCount++;
            }

            for (int i = 0; i < 7; i++)
            {
                if (!displayReserves)
                {
                    if (GameManager.singleton.currentEncounter.encounter[i] != null)
                    {
                        buttonSet[currentCount].text = GameManager.singleton.currentEncounter.encounter[i].name;
                    }
                    else
                    {
                        buttonSet[currentCount].text = "-----";
                    }
                }
                else
                {
                    if (GameManager.singleton.playerReserves[i] != null)
                    {
                        buttonSet[currentCount].text = GameManager.singleton.playerReserves[i].seenName;
                    }
                    else
                    {
                        buttonSet[currentCount].text = "-----";
                    }
                }

                    currentCount++;
            }
        }
    }

    public void SwitchTargets()
    { // displays reserves singularly for swapping out

        // single hit display
    
        int currentCount = 0;

        for (int i = 0; i < 4; i++)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {
                buttonSet[currentCount].text = GameManager.singleton.playerParty[i].seenName;
            }
            else
            {
                buttonSet[currentCount].text = "-----";
            }

            currentCount++;
        }

        for (int i = 0; i < 7; i++)
        {
            if (!displayReserves)
            {
                if (GameManager.singleton.currentEncounter.encounter[i] != null)
                {
                    buttonSet[currentCount].text = GameManager.singleton.currentEncounter.encounter[i].name;
                }
                else
                {
                    buttonSet[currentCount].text = "-----";
                }
            }
            else
            {
                if (GameManager.singleton.playerReserves[i] != null)
                {
                    buttonSet[currentCount].text = GameManager.singleton.playerReserves[i].seenName;
                }
                else
                {
                    buttonSet[currentCount].text = "-----";
                }
            }

                currentCount++;
        }
    }
}
