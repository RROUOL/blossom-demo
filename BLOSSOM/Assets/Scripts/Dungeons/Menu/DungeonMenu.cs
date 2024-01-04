using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems;
using Vaz.ManagerSingletons;

public class DungeonMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    EventSystem eventSystem;
    [SerializeField] GameObject currentTab;
    bool isPaused;
    [SerializeField] GameObject pauseTabs;
    [SerializeField] GameObject pauseBubbles;

    [SerializeField] Text textBox;

    [SerializeField] PartyMenu partyMenu;
    // switchmenu, etcmenu..

    void Start()
    {
        // get event system for assigning current selected UI element

        eventSystem = EventSystem.current;

        //Unpause(true);

        textBox.text = "What will you do?";
    }

    void Update()
    {
        if (!GameManager.singleton.canDungeonPause)
        {
            //Debug.Log("cannot pause outside the dungeon or is otherwise disallowed");
            if (isPaused)
            {
                Unpause(true);
            }
            return;
        }
        else
        {
            //Debug.Log("YES can pause");
        }

        partyMenu.InitialiseActors();

        if (cInput.GetKeyDown("Menu"))
        {
            //Debug.Log("pauseMenu check 2: " + pauseMenu.name);
            if (isPaused)
            {
                //Debug.Log("Menu is active. Closing..");
                Unpause(true);
                return;
            }
            else
            {
                //Debug.Log("Menu isn't active. Opening..");
                if (!GameManager.singleton.canDungeonPause || GameManager.singleton.isInputBlocked)
                { // if player already can't move (because of cinematic or colliding with enemy) then button does nothing
                    Debug.Log("NORMAL PILLS DETECTED");
                    return;
                }
                Pause();
                GameManager.singleton.isDungeonPaused = true;
                EventSystem.current.SetSelectedGameObject(currentTab);
                return;
            }
        }
    }

    public void Pause()
    {
        Debug.Log("MENU PAUSED BY " + this.gameObject.name);
        pauseTabs.SetActive(true);
        pauseMenu.SetActive(true);
        GameManager.singleton.PlaySound(2);
        GameManager.singleton.ToggleUI(GameManager.singleton.fpsCounter.activeSelf, 
                                        GameManager.singleton.dateDisplay.activeSelf, 
                                            GameManager.singleton.daysLeftDisplay.activeSelf, 
                                                GameManager.singleton.statusBusts.activeSelf);
        EventSystem.current.SetSelectedGameObject(currentTab);
        isPaused = true;
    }

    public void Unpause(bool canMove)
    { // can be called to hide menu when battle starts without allowing player to move again
        Debug.Log("MENU UNPAUSED BY " + this.gameObject.name);
        pauseMenu.SetActive(false);
        GameManager.singleton.PlaySound(3);
        GameManager.singleton.ToggleUI(GameManager.singleton.fpsCounter.activeSelf, 
                                        GameManager.singleton.dateDisplay.activeSelf, 
                                            GameManager.singleton.daysLeftDisplay.activeSelf, 
                                                GameManager.singleton.statusBusts.activeSelf);
        isPaused = false;
        
        GameManager.singleton.isDungeonPaused = !canMove;

        // disable children

        foreach (Transform child in pauseBubbles.transform)
            child.gameObject.SetActive(false);
    }

    public void SetCurrentTab(GameObject tab)
    {
        Debug.Log("SetCurrentTab() called with param: " + tab.name);
        currentTab = tab;
    }

    public void HealParty()
    {
        Debug.Log("Menu HealParty() called");
        for (int i = 0; i < 4; i += 1)
        {
            Debug.Log(GameManager.singleton.playerParty[i].seenName + "'s health was " + GameManager.singleton.playerParty[i].currentHP +
                        "It's now " + GameManager.singleton.playerParty[i].HealthPoints.Value);
            GameManager.singleton.playerParty[i].currentHP = (int)GameManager.singleton.playerParty[i].HealthPoints.Value;
            GameManager.singleton.playerParty[i].isBleeding = false;
            GameManager.singleton.playerParty[i].isBurning = false;
            GameManager.singleton.playerParty[i].isGravitated = false;
            GameManager.singleton.playerParty[i].isPoisoned = false;
            GameManager.singleton.playerParty[i].isFrostbitten = false;
            GameManager.singleton.playerParty[i].isShocked = false;
            GameManager.singleton.playerParty[i].isStunned = false;
            GameManager.singleton.playerParty[i].isDowned = false;
        }
    }

    public void TeleportToStart()
    { // Teleport to beginning of dungeon, assuming it is position 0,0,0
        Debug.Log("Menu TeleportToStart() called");
        GameManager.singleton.newPosition.Set(0, 0, 0);
        GameManager.singleton.newRotation.Set(0, 0, 0, GameManager.singleton.newRotation.w);

        GameManager.singleton.StartTeleport(GameManager.singleton.onStartTeleportEnable);
    }
}