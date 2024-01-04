/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class SkillHandler : MonoBehaviour
{
    [SerializeField] public BattleHandler battleHandler;
    [SerializeField] public Text textBox;

    [SerializeField] GameObject reservesOrEnemiesBox;
    [SerializeField] GameObject skillBox;

    // skill
    public void UseMove(int numOfMove)
    { // player using move function

        if (numOfMove < 2)
        { // use basic skill
            if (GameManager.singleton.playerParty[battleHandler.currentActor].basicAttacks[numOfMove] != null)
            {
                battleHandler.currentlySelectedMove = GameManager.singleton.playerParty[battleHandler.currentActor].basicAttacks[numOfMove];
                
                Debug.Log("USING MOVE: " + battleHandler.currentlySelectedMove.name);
                battleHandler.PlaySound(1);
                textBox.text = battleHandler.currentlySelectedMove.name + ": " + battleHandler.currentlySelectedMove.moveDescription;


                skillBox.SetActive(false);
            }
            else
            {
                Debug.Log("ERROR: NO MOVE SELECTED");
                battleHandler.PlaySound(2);
                return;
            }
        }
        else
        { // use advanced skill
            if (GameManager.singleton.playerParty[battleHandler.currentActor].moveList[numOfMove-2] != null)
            {
                battleHandler.currentlySelectedMove = GameManager.singleton.playerParty[battleHandler.currentActor].moveList[numOfMove-2];

                Debug.Log("USING MOVE: " + battleHandler.currentlySelectedMove.name);
                battleHandler.PlaySound(1);
                textBox.text = battleHandler.currentlySelectedMove.name + ": " + battleHandler.currentlySelectedMove.moveDescription;


                skillBox.SetActive(false);
            }
            else
            {
                Debug.Log("ERROR: NO MOVE SELECTED");
                battleHandler.PlaySound(2);
                return;
            }
        }

        // if healing..
        if (!battleHandler.currentlySelectedMove.isDamaging)
        {
            reservesOrEnemiesBox.SetActive(true);
            battleHandler.eventSystem.SetSelectedGameObject(battleHandler.sk_ROE_Default);
        }
        else
        {
            battleHandler.singleDisplay.displayReserves = false;
            battleHandler.groupDisplay.displayReserves = false;

            battleHandler.ShowTargets(battleHandler.currentlySelectedMove.hitsAll);
        }

    }

    public void Skill(int chosenIndex)
    {
        List<Entity> skillTargets = new List<Entity>();

        if (chosenIndex < 4)
        {
            if (battleHandler.currentlySelectedMove.hitsAll)
            {
                for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
                {
                    skillTargets.Add(GameManager.singleton.playerParty[i]);
                }
            }
            else
            {
                if (GameManager.singleton.playerParty[chosenIndex] != null)
                {   
                    skillTargets.Add(GameManager.singleton.playerParty[chosenIndex]);
                }
                else
                {
                    battleHandler.PlaySound(2);
                    return;
                }
            }
        }
        else
        {
            if (battleHandler.groupDisplay.displayReserves)
            { // if reserves are showing, add ally to skillTargets
                if (battleHandler.currentlySelectedMove.hitsAll)
                {
                    for (int i = 0; i < GameManager.singleton.playerReserves.Length; i += 1)
                    {
                        skillTargets.Add(GameManager.singleton.playerReserves[i]);
                    }
                }
                else
                {
                    if (GameManager.singleton.playerReserves[chosenIndex-4] != null)
                    {   
                        skillTargets.Add(GameManager.singleton.playerReserves[chosenIndex-4]);
                    }
                    else
                    {
                        battleHandler.PlaySound(2);
                        return;
                    }
                }
            }
            else
            { // if not, add enemy to skillTargets
                if (battleHandler.currentlySelectedMove.hitsAll)
                {
                    for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
                    {
                        skillTargets.Add(GameManager.singleton.currentEncounter.encounter[i]);
                    }
                }
                else
                {
                    if (GameManager.singleton.currentEncounter.encounter[chosenIndex-4] != null)
                    {   
                        skillTargets.Add(GameManager.singleton.currentEncounter.encounter[chosenIndex-4]);
                    }
                    else
                    {
                        battleHandler.PlaySound(2);
                        return;
                    }
                }
            }

        }

        battleHandler.PlaySound(1);
        textBox.text = GameManager.singleton.playerParty[battleHandler.currentActor].name + " uses " + battleHandler.currentlySelectedMove.name + "!";
        Entity[] targetsArray = skillTargets.ToArray();
        battleHandler.playerPresses.currentTurns -= battleHandler.currentlySelectedMove.UseMove(GameManager.singleton.playerParty[battleHandler.currentActor], targetsArray);

        battleHandler.ActionTaken();
    }
}
*/