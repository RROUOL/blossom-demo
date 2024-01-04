/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class GuardHandler : MonoBehaviour
{
    [SerializeField] public BattleHandler battleHandler;
    [SerializeField] public Text textBox;

    public void Guard()
    {
        battleHandler.playerPresses.currentTurns -= 2;
        GameManager.singleton.playerParty[battleHandler.currentActor].isGuarding = true;
        textBox.text = GameManager.singleton.playerParty[battleHandler.currentActor].name + " begins guarding!";
        battleHandler.ActionTaken();
    }
}
*/