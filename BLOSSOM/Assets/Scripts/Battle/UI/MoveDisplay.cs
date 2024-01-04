using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class MoveDisplay : MonoBehaviour
{
    [SerializeField] BattleHandler battleHandler;

    public Text[] buttonSet = new Text[10];

    public void DisplayMoves()
    {
        int currentCount = 0;

        for (int i = 0; i < 2; i++)
        {
            if (GameManager.singleton.playerParty[battleHandler.currentActor].basicAttacks[i] != null)
            {
                buttonSet[currentCount].text = GameManager.singleton.playerParty[battleHandler.currentActor].basicAttacks[i].name;
            }
            else
            {
                buttonSet[currentCount].text = "-----";
            }

            currentCount++;
        }

        for (int i = 0; i < 8; i++)
        {
            if (GameManager.singleton.playerParty[battleHandler.currentActor].moveList[i] != null)
            {
                buttonSet[currentCount].text = GameManager.singleton.playerParty[battleHandler.currentActor].moveList[i].name;
            }
            else
            {
                buttonSet[currentCount].text = "-----";
            }

            currentCount++;
        }
    }
}
