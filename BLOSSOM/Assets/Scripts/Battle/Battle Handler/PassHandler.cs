/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class PassHandler : MonoBehaviour
{
    [SerializeField] public BattleHandler battleHandler;
    [SerializeField] public Text textBox;

    public void Pass()
    {
        battleHandler.playerPresses.currentTurns -= 1;
        battleHandler.ActionTaken();
        textBox.text = "Passing to " + GameManager.singleton.playerParty[battleHandler.currentActor].name + ".";
    }
}
*/