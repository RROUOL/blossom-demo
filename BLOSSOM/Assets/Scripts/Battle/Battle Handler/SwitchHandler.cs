/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class SwitchHandler : MonoBehaviour
{
    [SerializeField] public BattleHandler battleHandler;
    [SerializeField] public Text textBox;

    [SerializeField] GameObject switch1Box;
    [SerializeField] GameObject switch2Box;

    [SerializeField] TargetDisplay switch1Display;
    [SerializeField] TargetDisplay switch2Display;

    bool partyCharSelected;
    int storedPos1 = -1;
    int storedPos2 = -1;

    // switch
    public void ClearStoredChar()
    {
        partyCharSelected = false;
        storedPos1 = -1;
        storedPos2 = -1;
    }

    public void ShowReserves()
    { // display party and reserves when switch is pressed
        switch1Display.SwitchTargets();
        switch2Display.SwitchTargets();
    }

    public void PickWhoToSwap1(int chosenIndex)
    {
        // check if position is null, if so give off reject sound, else move on to second phase of switch

        if (chosenIndex < 4)
        {
            if (GameManager.singleton.playerParty[chosenIndex] != null)
            {
                partyCharSelected = true;
                textBox.text = "Swap " + GameManager.singleton.playerParty[chosenIndex].name + " with who?";
            }
            else
            {
                battleHandler.PlaySound(2);
                return;
            }
        }
        else
        {
            if (GameManager.singleton.playerReserves[chosenIndex-4] != null)
            {
                partyCharSelected = false;
                textBox.text = "Swap " + GameManager.singleton.playerReserves[chosenIndex-4].name + " with who?";
            }
            else
            {
                battleHandler.PlaySound(2);
                return;
            }
        }

        battleHandler.PlaySound(1);
        switch1Box.SetActive(false);
        switch2Box.SetActive(true);
        storedPos1 = chosenIndex;
    }

    public void PickWhoToSwap2(int chosenIndex)
    {
        if (storedPos1 == chosenIndex)
        {
            battleHandler.PlaySound(2);
            return;
        }

        if (chosenIndex < 4)
        {
            if (GameManager.singleton.playerParty[chosenIndex] != null)
            {
                //textBox.text = "Swap with " + GameManager.singleton.playerParty[chosenIndex].name;
            }
        }
        else
        {
            if (GameManager.singleton.playerReserves[chosenIndex-4] == null)
            {
                if (battleHandler.HowManyActive() <= 1 && partyCharSelected)
                {
                    textBox.text = "Cannot have an empty party.";
                    battleHandler.PlaySound(2);
                    return;
                }
            }
        }

        battleHandler.PlaySound(1);
        storedPos2 = chosenIndex;
        switch2Box.SetActive(false);
        Switch();
    }

    void Switch()
    {
        battleHandler.playerPresses.currentTurns -= 1;
        Hero storedChar1;
        Hero storedChar2;

        textBox.text = "Swapping...";

        // store characters
        if (storedPos1 < 4)
        {
            storedChar1 = GameManager.singleton.playerParty[storedPos1];
        }
        else
        {
            storedChar1 = GameManager.singleton.playerReserves[storedPos1-4];
        }
        if (storedPos2 < 4)
        {
            storedChar2 = GameManager.singleton.playerParty[storedPos2];
        }
        else
        {
            storedChar2 = GameManager.singleton.playerReserves[storedPos2-4];
        }

        // null the originals
        if (storedPos1 < 4)
        {
            GameManager.singleton.playerParty[storedPos1] = null;
        }
        else
        {
            GameManager.singleton.playerReserves[storedPos1-4] = null;
        }
        if (storedPos2 < 4)
        {
            GameManager.singleton.playerParty[storedPos2] = null;
        }
        else
        {
            GameManager.singleton.playerReserves[storedPos2-4] = null;
        }

        // swap
        if (storedPos1 < 4)
        {
            GameManager.singleton.playerParty[storedPos1] = storedChar2;
        }
        else
        {
            GameManager.singleton.playerReserves[storedPos1-4] = storedChar2;
        }
        if (storedPos2 < 4)
        {
            GameManager.singleton.playerParty[storedPos2] = storedChar1;
        }
        else
        {
            GameManager.singleton.playerReserves[storedPos2-4] = storedChar1;
        }

        // re-initialise actors
        battleHandler.InitialiseActors();
        // end turn
        battleHandler.IsTurnOver();
    }
}
*/