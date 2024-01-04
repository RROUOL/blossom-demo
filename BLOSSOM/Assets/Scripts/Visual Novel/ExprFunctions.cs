using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using Vaz.ManagerSingletons;

[Naninovel.ExpressionFunctions]
public static class ExprFunctions
{
    // "return nothing" functions are functionally void. Naninovel doesn't let me use pure void functions so I need it to return some garbage data to trick it

    public static string RefreshParty()
    {
        GameManager.singleton.RefreshParty();

        return "nothing";
    }

    public static string TeachMove(int heroIndex, int moveIndex)
    {
        bool moveTaught = false;

        for (int i = 0; i < GameManager.singleton.databank.heroesBank[heroIndex].moveList.Length; i += 1)
        {
            if (GameManager.singleton.databank.heroesBank[heroIndex].moveList[i] == null)
            {
                GameManager.singleton.databank.heroesBank[heroIndex].moveList[i] = GameManager.singleton.databank.movesBank[moveIndex];
                Debug.Log(GameManager.singleton.databank.movesBank[moveIndex].name + " taught to " + GameManager.singleton.databank.heroesBank[heroIndex].name);
                moveTaught = true;
                break;
            }
            else
            {
                Debug.Log(GameManager.singleton.databank.heroesBank[heroIndex].moveList[i].name + " already occupies this slot.");
            }
        }

        if (!moveTaught)
        {
            Debug.Log("Move couldn't be taught to " + GameManager.singleton.databank.heroesBank[heroIndex].seenName);
        }

        return "nothing";
    }

    public static string ClearPartyMoveList()
    {   // reset the scriptables for the party
        foreach (Hero hero in GameManager.singleton.databank.heroesBank)
        {
            Debug.Log("Clearing movelist of: " + hero.seenName);
            hero.moveList = new Move[8];
        }

        return "nothing";
    }

    public static string EndDay()
    { // Shortcut to skip to the next calendar day. Called when leaving the dungeon.
        // Toggle Dungeon UI off
        ToggleUI(true, true, false);

        // Fade to black
        Fading(true);

        // Increment day forward
        IncrementDate();

        GameManager.singleton.RefreshParty();
        GameManager.singleton.RefreshDungeon();

        // Fade back to normal
        Fading(false);

        return "nothing";
    }

    public static string Fading(bool fadingToBlack)
    { // Completely halts game while fading in/out. No other code can run until this is finished.
        if (fadingToBlack)
        {
            // turns to black
            while (GameManager.singleton.canvasGroup.alpha < 1.0f)
            {
                Debug.Log("ExprFunctions.cs Fading() to black!!");
                GameManager.singleton.canvasGroup.alpha += Time.deltaTime;
            }
            Debug.Log("ExprFunctions.cs Fading() finished fading in");
        }
        else
        {
            // can see again
            while (GameManager.singleton.canvasGroup.alpha > 0.0f)
            {
                Debug.Log("ExprFunctions.cs Fading() to normal!!");
                GameManager.singleton.canvasGroup.alpha -= Time.deltaTime;
            }
            Debug.Log("ExprFunctions.cs Fading() finished fading out");
        }
        return "nothing";
    }

    public static string BeginTeleport(int x1, int y1, int z1, int x2, int y2, int z2)
    { // Teleport to specific place
        var gameManager = Object.FindObjectOfType<GameManager>();
        gameManager.newPosition.Set(x1, y1, z1);
        gameManager.newRotation.Set(x2, y2, z2, gameManager.newRotation.w);
        gameManager.StartTeleport();
        return "nothing";
    }

    public static string ToggleUI(bool date_, bool _daysLeft, bool _busts)
    { // Toggle whether or not UI is being shown (date, pause menu, stat busts, )
        GameManager.singleton.ToggleUI(GameManager.singleton.fpsCounter.activeSelf, date_, _daysLeft, _busts);
        return "nothing";
    }


    // Instantiate dungeon prefab to load levels


    public static string SetDate(string day, string month, string year)
    { // Change day, month, year in variables
        var variableManager = Engine.GetService<ICustomVariableManager>();
        variableManager.SetVariableValue("G_DateDay", day);
        variableManager.SetVariableValue("G_DateMonth", month);
        variableManager.SetVariableValue("G_DateYear", year);
        return "nothing";
    }

    // Set current date day, month, weekday in gamemanager
    public static string ChangeDate(string weekday)
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        int day = int.Parse(variableManager.GetVariableValue("G_DateDay"));
        int month = int.Parse(variableManager.GetVariableValue("G_DateMonth"));
        int year = int.Parse(variableManager.GetVariableValue("G_DateYear"));
        GameManager.singleton.ChangeDate(day, month, year, weekday);
        
        int remainingDays = int.Parse(variableManager.GetVariableValue("G_RemainingDays"));
        GameManager.singleton.SetDaysLeft(remainingDays);
        
        return "nothing";
    }

    // Increment current day, or month
    public static string IncrementDate()
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        int day = int.Parse(variableManager.GetVariableValue("G_DateDay"));
        int month = int.Parse(variableManager.GetVariableValue("G_DateMonth"));
        int year = int.Parse(variableManager.GetVariableValue("G_DateYear"));
        int remainingDays = int.Parse(variableManager.GetVariableValue("G_RemainingDays"));

        day += 1;
        remainingDays -= 1; // This is already decremented in GameManager.

        // February 2 (leap year) - 29 days
        if (month == 2 && year % 4 == 0)
        {
            if (day > 29)
            {
                day = 1;
                month = 3;
            }
        } // February 2 - 28 days
        else if (month == 2) 
        {
            if (day > 28)
            {
                day = 1;
                month = 3;
            }
        } // April 4, June 6, September 9, November 11 - 30 days
        else if (month == 4 || month == 6 || month == 9 || month == 11)
        {
            if (day > 30)
            {
                day = 1;
                month += 1;
            }
        } // January 1, March 3, May 5, July 7, August 8, October 10, December 12 - 31 days
        else if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
        {
            if (day > 31)
            {
                day = 1;
                month += 1;

                if (month > 12)
                {
                    month = 1;
                    year += 1;
                    Debug.Log("Happy New Year!");
                }
            }
        }
        else
        {
            Debug.LogError(month + " is not a valid month.");
        }

        Debug.Log("It's a new day: " + day + "/" + month + "/" + year + ". There are " + remainingDays);
        variableManager.SetVariableValue("G_DateDay", day.ToString());
        variableManager.SetVariableValue("G_DateMonth", month.ToString());
        variableManager.SetVariableValue("G_DateYear", year.ToString());
        variableManager.SetVariableValue("G_RemainingDays", remainingDays.ToString());
        GameManager.singleton.ChangeDate(day, month, year, remainingDays);

        return "nothing";
    }

    // Get current date day, month, year
    public static int GetDay()
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        var myValue = variableManager.GetVariableValue("G_DateDay");
        int num = int.Parse(myValue);
        return num;
    }

    public static int GetMonth()
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        var myValue = variableManager.GetVariableValue("G_DateMonth");
        int num = int.Parse(myValue);
        return num;
    }

    public static int GetYear()
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        var myValue = variableManager.GetVariableValue("G_DateYear");
        int num = int.Parse(myValue);
        return num;
    }

    public static int ObtainItem(int itemNum)
    {
        // search item databank for item by number

        return 0;
    }

    public static int ObtainItem(string itemName)
    {
        // search item databank for item by name

        return 0;
    }

    public static int StartEncounter(int encIndex)
    {
        // search encounter databank for encounter by number
        Debug.Log("Starting encounter: " + GameManager.singleton.databank.encounterBank[encIndex].name);
        GameManager.singleton.StartBattle(GameManager.singleton.databank.encounterBank[encIndex]);

        return 0;
    }

}

