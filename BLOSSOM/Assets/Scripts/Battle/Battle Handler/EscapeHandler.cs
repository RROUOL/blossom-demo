/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class EscapeHandler : MonoBehaviour
{
    [SerializeField] public BattleHandler battleHandler;
    [SerializeField] public Text textBox;

    // escape
    public void Escape()
    {
        if (GameManager.singleton.currentEncounter.isBoss)
        {
            textBox.text = "You can't escape!";
            battleHandler.TurnCooldown();
        }
        else
        {
            textBox.text = "You ran away!";
            battleHandler.PlaySound(3);
            battleHandler.timeToEndBattle = 1f;
        }
    }
}
*/