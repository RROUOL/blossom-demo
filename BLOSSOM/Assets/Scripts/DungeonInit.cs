using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class DungeonInit : MonoBehaviour
{   // ensures that only one dungeon layout can be active at a time

    public List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    public List<Encounter> randomEncounters = new List<Encounter>();
    public List<int> encounterWeights = new List<int>();

    public List<GameObject> startingRoom = new List<GameObject>();
    public List<GameObject> allRooms = new List<GameObject>();

    void Awake()
    {
        // find all other objects with "Dungeon Layout" tag
        GameObject[] dungeons = GameObject.FindGameObjectsWithTag("Dungeon Layout");

        // deactivate those objects
        foreach (GameObject dungeon in dungeons)
        {
            Debug.Log(dungeon.name + " has been deactivated by DungeonInit");
            dungeon.SetActive(false);
        }

        // make this object the current gameObject
        GameManager.singleton.currentDungeon = this.gameObject;

        GameManager.singleton.currentDungeon.SetActive(true);
        Debug.Log("Overworld enemies updated.");
        GameManager.singleton.currentEnemies = enemies;
        Debug.Log("Random Encounters updated.");
        GameManager.singleton.possibleEncounters = randomEncounters;
        GameManager.singleton.possibleEncounterWeights = encounterWeights;

        GameManager.singleton.onStartTeleportEnable = startingRoom;
        GameManager.singleton.onTeleportDisable = allRooms;
    }

    
}
