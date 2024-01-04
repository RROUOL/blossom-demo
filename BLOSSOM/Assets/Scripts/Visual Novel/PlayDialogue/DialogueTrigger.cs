using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using Vaz.ManagerSingletons;

[RequireComponent(typeof(BoxCollider))]
public abstract class DialogueTrigger : MonoBehaviour, IDataPersistence
{
    [Header("Script")]
    public string ScriptName;
    public string Label;

    [Header("Specific Date")]
    public bool specificDateOnly;
    [Space]
    public int day = 1;
    public int month = 1;
    public int year = 1111;

    [Header("Exhaust")]
    public bool canExhaust;
    public bool needsExhaustScript;
    public string exhaustScript;
    public bool isExhausted;

    //[Header("Encounter")]
    
    //public bool startsEncounter;
    //[Space]
    //[SerializeField] Encounter thisEncounter;
    //public bool startsOnExhaust;

    [Header("Environment Changes")]

    public bool environmentChanged = false;

    [Tooltip("Objects which are visible by default. e.g. a closed chest.")]
    public List<GameObject> offAppearObjects = new List<GameObject>();
    [Tooltip("Objects which are visible when dialogue is triggered. e.g. an opened chest.")]
    public List<GameObject> onAppearObjects = new List<GameObject>();

    void Awake()
    {
        ChangeEnvironment(environmentChanged);
    }

    public void LoadData(GameData data)
    {
        //this.foobarFlag = data.foobarFlag;
    }

    public void SaveData(ref GameData data)
    {
        //data.foobarFlag = this.foobarFlag;
    }
    
    protected void PlayNani()
    {

        if (isExhausted)
        {
            Debug.Log("dialogue exhausted");
            return;
        }

        if (specificDateOnly)
        {
            if ((GameManager.singleton.dateDay == day) 
                && (GameManager.singleton.dateMonth == month) 
                    && (GameManager.singleton.dateYear == year))
            {
                Debug.LogWarning("date matches: " + GameManager.singleton.dateDay + " - " + day + " | " 
                    + GameManager.singleton.dateMonth + " - " + month + " | " 
                        + GameManager.singleton.dateYear + " - " + year);
            }
            else
            {
                Debug.LogWarning("date does not match: " + GameManager.singleton.dateDay + " - " + day + " | " 
                    + GameManager.singleton.dateMonth + " - " + month + " | " 
                        + GameManager.singleton.dateYear + " - " + year);
                return;
            }
        }

        GameManager.singleton.isInputBlocked = true;
        GameManager.singleton.canDungeonPause = false;
        Debug.Log(this.name + " inputs turned off");

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;

        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label).Forget();

        Debug.Log(scriptPlayer);

        if (canExhaust)
        {
            this.enabled = false;
            //if (needsExhaustScript && (scriptPlayer.HasPlayed(exhaustScript)))
            //    isExhausted = true;

            //if (startsEncounter)
            //{
            //    if (startsOnExhaust) 
            //    {
            //        GameManager.singleton.StartBattle(thisEncounter);
            //    }
            //}
        }

        //if (startsEncounter)
        //{
        //    if (!startsOnExhaust)
        //   {
        //        GameManager.singleton.StartBattle(thisEncounter);
        //    }
        //}

        ChangeEnvironment(!environmentChanged);
    }

    protected void ChangeEnvironment(bool b_)
    {
        if (!b_)
        {
            environmentChanged = false;
            for (int i = 0; i < offAppearObjects.Count; i += 1)
            {
                offAppearObjects[i].SetActive(true);
            }
            for (int i = 0; i < onAppearObjects.Count; i += 1)
            {
                onAppearObjects[i].SetActive(false);
            }
        }
        else
        {
            environmentChanged = true;
            for (int i = 0; i < offAppearObjects.Count; i += 1)
            {
                offAppearObjects[i].SetActive(false);
            }
            for (int i = 0; i < onAppearObjects.Count; i += 1)
            {
                onAppearObjects[i].SetActive(true);
            }
        }
    }
}