using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.Constants.Databank;

[System.Serializable]
public class GameData : MonoBehaviour
{
    //public Databank databank;

    public string playerName;
    public int playTime; 
    public int day;
    public int month;
    public int year;

    // flags
    public bool FLAG_DUNGEON_1x01;

    // current character stats

    // current party active
    // current party reserves

    // current items

    public GameData()
    {
        this.playerName = "Lorelei";
        playTime = 0;
        day = 23;
        month = 7;
        year = 2020;

        // set flags to default
        FLAG_DUNGEON_1x01 = false;

        // set playable characters to default values

        // Lorelei
        //databank.heroesBank[0].seenName = "Lorelei";

        // Oscar
        //databank.heroesBank[1].seenName = "Oscar";

        // Jasmine
        //databank.heroesBank[2].seenName = "Jasmine";

        // Lavinia (Cloud)
        //databank.heroesBank[3].seenName = "Lavinia";
    }
}