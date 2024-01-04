using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.EventSystems;
using Vaz.ManagerSingletons;
using Vaz.Constants.Consts;

// for damage/ailments, 
//  replace GameManager.singleton.currentEncounter.encounter[i]
//  with    instantiateFoes[i]

public enum BattleState
{
    PlayerTurn,
    EnemyTurn
}
public enum BattleSubstate
{
    PreTurn,    // Ailment.SufferBefore()
    Turn,       // Player can move
    PostTurn    // Ailment.SufferAfter()
}
public class AilmentDisplay
{
    public Image[] _icons = new Image[20];

    [SerializeField] Image Pug_minusTwoIcon;
    [SerializeField] Image Pug_minusOneIcon;
    [SerializeField] Image Pug_plusOneIcon;
    [SerializeField] Image Pug_plusTwoIcon;

    [SerializeField] Image Occ_minusTwoIcon;
    [SerializeField] Image Occ_minusOneIcon;
    [SerializeField] Image Occ_plusOneIcon;
    [SerializeField] Image Occ_plusTwoIcon;

    [SerializeField] Image Con_minusTwoIcon;
    [SerializeField] Image Con_minusOneIcon;
    [SerializeField] Image Con_plusOneIcon;
    [SerializeField] Image Con_plusTwoIcon;

    [SerializeField] Image Neg_minusTwoIcon;
    [SerializeField] Image Neg_minusOneIcon;
    [SerializeField] Image Neg_plusOneIcon;
    [SerializeField] Image Neg_plusTwoIcon;

    public void UpdateFrames(Entity e)
    {
        // ailments
        int i = 0;
        for (int j = 0; j < e.AilmentList.Count; j += 1)
        {
            _icons[j] = e.AilmentList[j].icon;
            i = j;
        }
        // now, do buffs

        // pugilism

        switch (e.currentPugilism)
        {
            case -2:
                _icons[i] = Pug_minusTwoIcon;
                i += 1;
                break;
            case -1:
                _icons[i] = Pug_minusOneIcon;
                i += 1;
                break;
            case 1:
                _icons[i] = Pug_plusOneIcon;
                i += 1;
                break;
            case 2:
                _icons[i] = Pug_plusTwoIcon;
                i += 1;
                break;
            default:
                Debug.Log("Pugilism isn't displayed because the value is " + e.currentPugilism);
                break;
        }

        // occultism

        switch (e.currentOccultism)
        {
            case -2:
                _icons[i] = Occ_minusTwoIcon;
                i += 1;
                break;
            case -1:
                _icons[i] = Occ_minusOneIcon;
                i += 1;
                break;
            case 1:
                _icons[i] = Occ_plusOneIcon;
                i += 1;
                break;
            case 2:
                _icons[i] = Occ_plusTwoIcon;
                i += 1;
                break;
            default:
                Debug.Log("Occultism isn't displayed because the value is " + e.currentOccultism);
                break;
        }

        // constitution

        switch (e.currentConstitution)
        {
            case -2:
                _icons[i] = Con_minusTwoIcon;
                i += 1;
                break;
            case -1:
                _icons[i] = Con_minusOneIcon;
                i += 1;
                break;
            case 1:
                _icons[i] = Con_plusOneIcon;
                i += 1;
                break;
            case 2:
                _icons[i] = Con_plusTwoIcon;
                i += 1;
                break;
            default:
                Debug.Log("Constitution isn't displayed because the value is " + e.currentConstitution);
                break;
        }

        // negation

        switch (e.currentNegation)
        {
            case -2:
                _icons[i] = Neg_minusTwoIcon;
                i += 1;
                break;
            case -1:
                _icons[i] = Neg_minusOneIcon;
                i += 1;
                break;
            case 1:
                _icons[i] = Neg_plusOneIcon;
                i += 1;
                break;
            case 2:
                _icons[i] = Neg_plusTwoIcon;
                i += 1;
                break;
            default:
                Debug.Log("Negation isn't displayed because the value is " + e.currentNegation);
                break;
        }

    }
}

public class BattleInfo
{ // info given to enemies to decide what their next action should be
    public Hero[] enemyHeroes = new Hero[4];
    public Foe[] allyFoes = new Foe[7];
    public int turnCounter = 0;
}

public class BattleHandler : MonoBehaviour
{
    [SerializeField] BattleState battleState = BattleState.PlayerTurn;
    [SerializeField] BattleSubstate battleSubstate = BattleSubstate.PreTurn;
    [SerializeField] Text textBox;

    [SerializeField] CanvasGroup canvasGroup;

    int currentPressTurns;

    [SerializeField] public PressTurnDisplay playerPresses;
    [SerializeField] public PressTurnDisplay enemyPresses;

    [Space]

    [Tooltip("Determines what side moves first in the battle. Set by encounter.")]
    [SerializeField] int startingSide = 1; // 1 for player side, 2 for enemy side, 3 for domain (?)

    [Header("Heroes")]
    [SerializeField] Sprite emptyProfilePic;
    [SerializeField] Image[] playerProfilePic = new Image[4];
    [SerializeField] Text[] playerCharacterName = new Text[4];
    [SerializeField] Image[] playerColour = new Image[4];
    [SerializeField] Slider[] playerHPSlider = new Slider[4];
    [SerializeField] Text[] playerHPText = new Text[4];
    [SerializeField] Slider[] playerMPSlider = new Slider[4];
    [SerializeField] Text[] playerMPText = new Text[4];
    [SerializeField] Transform[] playerLocations = new Transform[4];

    [SerializeField] AilmentDisplay[] playerAilDisplay = new AilmentDisplay[4];

    [Header("Reserves")]
    [SerializeField] Transform[] reserveLocations = new Transform[7];

    [Header("Enemies")]
    [SerializeField] Foe[] instantiatedFoes = new Foe[7];
    [SerializeField] SpriteRenderer[] enemySprite = new SpriteRenderer[7];
    //[SerializeField] Text e1Name;
    [SerializeField] Image[] enemyColour = new Image[7];
    [SerializeField] Slider[] enemyHPSlider = new Slider[7];
    //[SerializeField] Slider[] enemyBGSlider = new Slider[7];
    //[SerializeField] AilmentDisplay[] enemyAilDisplay = new AilmentDisplay[7];

    [Header("Environment")]
    [SerializeField] Renderer battlePlane;

    [Header("Battle Settings")]
    [Tooltip("Time that the battle start splash text remains on screen.")]
    [SerializeField] float timeToStart = 3f;
    [Tooltip("The index of the actor that moves first on battle start. Never make this anything other than 0.")]
    public int currentActor = 0;
    public float timeToNextRound = 0f;
    [Tooltip("The index of the ailment that's first checked on battle start. Never make this anything other than 0.")]
    public int currentAilment = 0;
    public float timeToEndBattle = 0f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip confirm;
    [SerializeField] AudioClip reject;
    [SerializeField] AudioClip runAway;
    [SerializeField] AudioClip click;

    [Header("UI Menus")]
    public EventSystem eventSystem;
    [SerializeField] StandaloneInputModule inputModule;

    [SerializeField] GameObject interactiveBox;

    [SerializeField] public GameObject reservesOrEnemiesBox;
    [SerializeField] public GameObject sk_ROE_Default;

    [SerializeField] public GameObject targetSingleBox;
    [SerializeField] public GameObject sk_TS_Default;
    [SerializeField] public GameObject targetGroupBox;
    [SerializeField] public GameObject sk_TG_Default;

    [SerializeField] public GameObject actionBox;
    [SerializeField] public GameObject aDefault;
    
    [SerializeField] public GameObject skillBox;
    [SerializeField] public GameObject skDefault;

    [SerializeField] public GameObject guardBox;
    [SerializeField] public GameObject gDefault;

    [SerializeField] public GameObject passBox;
    [SerializeField] public GameObject pDefault;

    [SerializeField] public GameObject switch1Box;
    [SerializeField] public GameObject sw1Default;
    [SerializeField] public GameObject switch2Box;
    [SerializeField] public GameObject sw2Default;

    [SerializeField] public GameObject itemBox;
    [SerializeField] public GameObject iDefault;

    [SerializeField] public GameObject escapeBox;
    [SerializeField] public GameObject eDefault;

    [SerializeField] TargetDisplay switch1Display;
    [SerializeField] TargetDisplay switch2Display;

    [SerializeField] public TargetDisplay singleDisplay;
    [SerializeField] public TargetDisplay groupDisplay;

    [Header("Battle Cursor")]
    [SerializeField] GameObject battleCursor;
    [SerializeField] TextMeshPro battleCursorText;

    [Header("Other Handlers")]

    // switch handler
    bool partyCharSelected;
    int storedPos1 = -1;
    int storedPos2 = -1;

    // temp data which is saved for targeting, clear these constantly to be safe
    [Header("Misc.")]

    [SerializeField] GameObject rotator;   
    public Move currentlySelectedMove;
    //Item currentlySelectedItem;

    // 0 = escaped, 1 = won, 2 = lost
    private int battleOutcome = 0;

    void Awake()
    {
        eventSystem = EventSystem.current;

        canvasGroup = GameManager.singleton.GetFadeBlack();
        canvasGroup.alpha = 1;
        
        //if (canvasGroup == null)
        //{
        //    Debug.LogError("LETHAL ERROR: Canvas Group not found!");
        //    return;
        //}

        InitialiseActors();
        InitialiseEnvironment();
        battleOutcome = 0;

        if (GameManager.singleton.currentEncounter.enemySound != null)
        {
            GameManager.singleton.PlaySound(GameManager.singleton.currentEncounter.enemySound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(battleState + ", " + battleSubstate + ", " + currentActor);

        UpdateBars();
        RefreshSelectedUI();
        CheckCountdowns();
        ShouldInteractiveBeActive();
    }

    public void PlaySound(int whichSound)
    {
        //Debug.Log("BattleHandler - PlaySound called");
        switch (whichSound)
        {
            case 1:
                GameManager.singleton.SFXplayer.PlayOneShot(confirm, 1.0f);
                break;
            case 2:
                GameManager.singleton.SFXplayer.PlayOneShot(reject, 1.0f);
                break;
            case 3:
                GameManager.singleton.SFXplayer.PlayOneShot(runAway, 1.0f);
                break;
            case 4:
                GameManager.singleton.SFXplayer.PlayOneShot(click, 1.0f);
                break;
        }
    }
    
    public void InitialiseActors()
    {
        // initialise party members
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i+= 1)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {
                playerProfilePic[i].sprite = GameManager.singleton.playerParty[i].battleSprite;
                playerCharacterName[i].text = GameManager.singleton.playerParty[i].seenName;
                playerColour[i].color = GameManager.singleton.playerParty[i].healthBarColour;

                playerHPSlider[i].maxValue = GameManager.singleton.playerParty[i].HealthPoints.Value;
                playerHPSlider[i].value = GameManager.singleton.playerParty[i].currentHP;

                playerMPSlider[i].maxValue = GameManager.singleton.playerParty[i].ManaPoints.Value;
                playerMPSlider[i].value = GameManager.singleton.playerParty[i].currentMP;
                GameManager.singleton.playerParty[i].currentLocation = playerLocations[i];

                GameManager.singleton.playerParty[i].turnsToGain = 0;
            }
            else
            {
                playerProfilePic[i].sprite = emptyProfilePic;
                playerCharacterName[i].text = "EMPTY";
                playerColour[i].color = Color.black;

                playerHPSlider[i].maxValue = 999;
                playerHPSlider[i].value = 0;

                playerMPSlider[i].maxValue = 999;
                playerMPSlider[i].value = 0;
            }
        }

        // initialise reserves

        for (int i = 0; i < GameManager.singleton.playerReserves.Length; i+= 1)
        {
            if (GameManager.singleton.playerReserves[i] != null)
            {
                GameManager.singleton.playerReserves[i].currentLocation = reserveLocations[i];
            }
        }

        // initialise enemy encounter


        for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i+= 1)
        {
            if (GameManager.singleton.currentEncounter.encounter[i] != null)
            {
                //instantiate all into instantiatedFoes so that it doesn't overwrite encounter/foe

                instantiatedFoes[i] = Instantiate(GameManager.singleton.currentEncounter.encounter[i]);
                instantiatedFoes[i].name = instantiatedFoes[i].name.Replace("(Clone)","").Trim();
                Debug.Log("Creating new member of instantiatedFoes! - " + i + " - " + instantiatedFoes[i].name);

                // from here on out, only refer to instantiatedfoe
                // reset health and visuals of enemy

                instantiatedFoes[i].currentHP = (int)GameManager.singleton.currentEncounter.encounter[i].HealthPoints.Value;
                instantiatedFoes[i].currentMP = (int)GameManager.singleton.currentEncounter.encounter[i].ManaPoints.Value;
                enemyHPSlider[i].value = instantiatedFoes[i].currentHP;

                instantiatedFoes[i].isBleeding = false;
                instantiatedFoes[i].isBurning = false;
                instantiatedFoes[i].isGravitated = false;
                instantiatedFoes[i].isPoisoned = false;
                instantiatedFoes[i].isFrostbitten = false;
                instantiatedFoes[i].isShocked = false;
                instantiatedFoes[i].isStunned = false;
                instantiatedFoes[i].isDowned = false;

                instantiatedFoes[i].currentLocation = enemySprite[i].transform;
                //GameManager.singleton.currentEncounter.encounter[i].currentLocation = enemySprite[i].transform;


                //instantiate visuals of enemy
                enemySprite[i].sprite = instantiatedFoes[i].battleSprite;

                enemyColour[i].color = instantiatedFoes[i].healthBarColour;

                enemyHPSlider[i].maxValue = instantiatedFoes[i].HealthPoints.Value;

                instantiatedFoes[i].turnsToGain = 0;

                // reset AI weights
                instantiatedFoes[i].resetBattleWeights();
            }
            else
            {
                enemySprite[i].gameObject.SetActive(false);
                enemyColour[i].gameObject.SetActive(false);
                enemyHPSlider[i].gameObject.SetActive(false);
            }
        }

        textBox.text = GameManager.singleton.currentEncounter.encounterText;
    }

    public void InitialiseEnvironment()
    {
        Debug.Log("InitialiseEnvironment() called.");

        battlePlane.material = GameManager.singleton.currentPlaneMat;
        //RenderSettings.skybox = GameManager.singleton.currentSkyboxMat;
        //Debug.Log("skybox changed: " + RenderSettings.skybox);

        if (GameManager.singleton.currentEncounter.isBoss)
        {
            rotator.transform.rotation = Quaternion.Euler(270,0,0);
        }
        else
        {
            //rotator.transform.rotation = Quaternion.Euler(0,0,0);
            rotator.transform.rotation = Quaternion.Euler(270,0,0);
        }

        GameManager.singleton.BGMplayer.clip = GameManager.singleton.currentMusic;

        GameManager.singleton.BGMplayer.Play(0);

        playerPresses.numOfTurns = 0;
        enemyPresses.numOfTurns = 0;
    }
    
    void UpdateBars()
    { // updates sliders for all allies and enemies
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i+= 1)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {

                playerHPSlider[i].maxValue = GameManager.singleton.playerParty[i].HealthPoints.Value;
                playerHPSlider[i].value = GameManager.singleton.playerParty[i].currentHP;
                playerHPText[i].text = "HP: " + GameManager.singleton.playerParty[i].currentHP.ToString();

                playerMPSlider[i].maxValue = GameManager.singleton.playerParty[i].ManaPoints.Value;
                playerMPSlider[i].value = GameManager.singleton.playerParty[i].currentMP;
                playerMPText[i].text = "MP: " + GameManager.singleton.playerParty[i].currentMP.ToString();

                // if down..
                if (GameManager.singleton.playerParty[i].isDowned)
                {
                    playerProfilePic[i].color = new Color(0, 0, 0, 0.5f);
                }
                else
                {
                    playerProfilePic[i].color = new Color(1,1,1,1);
                }
            }
        }

        for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i+= 1)
        {
            if (instantiatedFoes[i] != null)
            {
                enemyHPSlider[i].maxValue = instantiatedFoes[i].HealthPoints.Value;
                enemyHPSlider[i].value = instantiatedFoes[i].currentHP;

                // if down..
                if (instantiatedFoes[i].isDowned)
                {
                    enemySprite[i].color = new Color(0, 0, 0, 0.5f);
                }
                else
                {
                    enemySprite[i].color = new Color(1,1,1,1);
                }
            }
        }
    }

    void RefreshSelectedUI()
    {
        if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject.transform.parent != null)
        {
            //Debug.Log("Current Active UI Object: " + eventSystem.currentSelectedGameObject.name + " | Parent of Object: " + eventSystem.currentSelectedGameObject.transform.parent.name);
        }
    }

    void CheckCountdowns()
    {
        if (timeToStart > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / 2;

            timeToStart -= Time.deltaTime;
            if (timeToStart > 0f)
            {
                return;
            }
            Debug.Log("timeToStart has finished.");

            // ensures that NextActor() rolls over to the 0th person in the list
            //currentActor = 0;
            battleSubstate = BattleSubstate.PreTurn;
            SwitchState(startingSide);
        }

        //Debug.Log(battleState + ", " + battleSubstate + ", " + currentActor + " reaches 1");

        if (timeToNextRound > 0f)
        {
            //Debug.Log(battleState + ", " + battleSubstate + ", " + currentActor + " reaches 2");
            
            timeToNextRound -= Time.deltaTime;
            if (timeToNextRound > 0f)
            {
                return;
            }
            Debug.Log("timeToNextRound has finished.");

            if (battleState == BattleState.PlayerTurn)
            {
                BeginPlayerTurn();
            }
            else if (battleState == BattleState.EnemyTurn)
            {
                Debug.Log("current enemy index: " + currentActor);
                BeginEnemyTurn();
                //textBox.text = GameManager.singleton.currentEncounter.encounter[currentActor].name + " attacks!";
            }
        }
        
        if (timeToEndBattle > 0f)
        {
            canvasGroup.alpha += Time.deltaTime / 2;
            
            if (canvasGroup.alpha == 1)
            {
                timeToEndBattle -= Time.deltaTime;
                if (timeToEndBattle > 0f)
                {
                    return;
                }
                Debug.Log("timeToEndBattle has finished.");

                EndBattle();
            }
        }
    }

    void ShouldInteractiveBeActive()
    {
        
        if (battleState == BattleState.PlayerTurn && battleSubstate == BattleSubstate.Turn)
        {
            //Debug.Log("Conditions met!? battleState.PlayerTurn = " + battleState + ", battleSubstate.Turn = " + battleSubstate);
            interactiveBox.SetActive(true);
        }
        else
        {
            //Debug.Log("Conditions NOT met. battleState.PlayerTurn != " + battleState + ", battleSubstate.Turn != " + battleSubstate);
            interactiveBox.SetActive(false);
        }
    }

    bool EvalBattleEnd()
    { // check if battle should end via winning or losing

        // if all players in party + reserves are at 0 HP,
        // or all enemies are SetActive(false)
        // end game accordingly
        bool isPartyWiped = true;
        bool isReservesWiped = true;

        bool isEnemiesWiped = true;

        // check if all 4 active party members are at 0 HP
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {
                if (GameManager.singleton.playerParty[i].currentHP > 0)
                {
                    isPartyWiped = false;
                }
            }
        }

        // if party wiped, check if all party members in reserve are at 0 HP
        if (isPartyWiped)
        {
            for (int i = 0; i < GameManager.singleton.playerReserves.Length; i += 1)
            {
                if (GameManager.singleton.playerReserves[i] != null)
                {
                    if (GameManager.singleton.playerReserves[i].currentHP > 0)
                    {
                        isReservesWiped = false;
                    }
                }
            }
        }

        // if party and reserves wiped, game over
        if (isPartyWiped && isReservesWiped)
        {
            Debug.Log("The enemies beat your bitch ass");
            OnBattleLose();
            return true;
        }

        // if reserves not wiped, swap all members
        if (isPartyWiped && !isReservesWiped)
        {
            // swap swap swap (later zzzz..)
        }

        // now check if enemies are wiped
        for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
        {
            if (instantiatedFoes[i] != null)
            {
                if (!instantiatedFoes[i].isDowned)
                {
                    //Debug.Log(instantiatedFoes[i] + " is NOT down!");
                    isEnemiesWiped = false;
                }
                else
                {
                    //Debug.Log("instantiatedFoes[i].isDowned: " + instantiatedFoes[i].isDowned);
                }
            }
        }

        if (isEnemiesWiped)
        {
            //Debug.Log("You beat their bitch ass!");
            OnBattleWin();
            return true;
        }
        
        // it has been established that game is not over, start new round
        return false;
    }

    void BeginRound()
    {// checks if battle should end, then begins round if not

        if (EvalBattleEnd()) return;

        if (battleState == BattleState.PlayerTurn)
        {
            // set all party isGuarding to false
            
            for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
            {
                if (GameManager.singleton.playerParty[i] != null)
                {
                    GameManager.singleton.playerParty[i].isGuarding = false;
                }
            }

            for (int i = 0; i < GameManager.singleton.playerReserves.Length; i += 1)
            {
                if (GameManager.singleton.playerReserves[i] != null)
                {
                    GameManager.singleton.playerReserves[i].isGuarding = false;
                }
            }

            int turnCount = 0;
            for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
            {
                if ((GameManager.singleton.playerParty[i] != null) && !GameManager.singleton.playerParty[i].isDowned)
                {
                    turnCount += GameManager.singleton.playerParty[i].PressTurns;
                    turnCount += GameManager.singleton.playerParty[i].turnsToGain;
                    GameManager.singleton.playerParty[i].turnsToGain = 0;
                }
            }
            Debug.Log("Player Presses: " + turnCount);
            playerPresses.numOfTurns = turnCount;
            playerPresses.currentTurns = playerPresses.numOfTurns;
            enemyPresses.numOfTurns = 0;

            BeginPlayerTurn();
        }
        if (battleState == BattleState.EnemyTurn)
        {
            // set all enemy isGuarding to false

            for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
            {
                if (instantiatedFoes[i] != null)
                {
                    instantiatedFoes[i].isGuarding = false;
                }
            }

            int turnCount = 0;
            for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
            {
                if ((instantiatedFoes[i] != null) && (!instantiatedFoes[i].isDowned))
                {
                    turnCount += instantiatedFoes[i].PressTurns;
                    turnCount += instantiatedFoes[i].turnsToGain;
                    instantiatedFoes[i].turnsToGain = 0;
                }
                else
                {
                    Debug.Log("Enemy index " + i + " skipped because they're down or nonexistent.");
                }
            }
            Debug.Log("Enemy Presses: " + turnCount);
            enemyPresses.numOfTurns = turnCount;
            enemyPresses.currentTurns = enemyPresses.numOfTurns;
            playerPresses.numOfTurns = 0;

            // enemy stuff
        }

        // ensure this comes *after* the press turn calculations! else, it'll automatically loop as if the team had 0 press turns.
        //currentActor -= 1;
        //NextActor();
    }

    void PreTurnAilments()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            if ((GameManager.singleton.playerParty[currentActor] == null) 
                || (!GameManager.singleton.playerParty[currentActor].canMove) 
                    || (GameManager.singleton.playerParty[currentActor].isDowned) 
                        || (GameManager.singleton.playerParty[currentActor].isStunned)
                            || (GameManager.singleton.playerParty[currentActor].isGuarding))
            {
                if (GameManager.singleton.playerParty[currentActor] != null)
                {
                    Debug.Log("PreTurnAilments has decided that this actor can't move: " + GameManager.singleton.playerParty[currentActor].seenName);
                }
                NextActor(true);
                currentAilment = 0;
                timeToNextRound = 1.0f;
                return;
            }

            Debug.Log("The player is suffering from preturn ailments - checking index " + currentAilment + " vs ailment count " + GameManager.singleton.playerParty[currentActor].AilmentList.Count);

            if (currentAilment >= GameManager.singleton.playerParty[currentActor].AilmentList.Count)
            {
                Debug.Log("There are no more ailments in this list. Starting turn...");
                battleSubstate = BattleSubstate.Turn;
                Debug.Log("swaos " + battleSubstate);
                BeginPlayerTurn();
                currentAilment = 0;
            }
            else
            {
                if (!GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment].activeBeforeTurn)
                {
                    Debug.Log("Ailment #" + currentAilment + " does not cause sufferbefore!");
                }
                else
                {
                    textBox.text = GameManager.singleton.playerParty[currentActor] + " is " + GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment] + "!";
                    GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment].SufferBefore(GameManager.singleton.playerParty[currentActor]);
                }
                currentAilment += 1;
                Debug.Log("PLAYER TIMETONEXTROUND UPDATED - PreTurnAilments - " + battleSubstate);
                timeToNextRound = 1.0f;
            }
        }
        else if (battleState == BattleState.EnemyTurn)
        {
            if ((GameManager.singleton.currentEncounter.encounter[currentActor] == null) 
                || (!instantiatedFoes[currentActor].canMove) 
                    || (instantiatedFoes[currentActor].isDowned) 
                        || (instantiatedFoes[currentActor].isStunned)
                            || (instantiatedFoes[currentActor].isGuarding))
            {
                if (instantiatedFoes[currentActor] != null)
                {
                   Debug.Log("PreTurnAilments has decided that this actor can't move: " + instantiatedFoes[currentActor].name);
                }
                NextActor(true);
                currentAilment = 0;
                timeToNextRound = 1.0f;
                return;
            }

            Debug.Log("The enemy is suffering from preturn ailments - checking index " + currentAilment + " vs ailment count " + instantiatedFoes[currentActor].AilmentList.Count);

            if (currentAilment >= instantiatedFoes[currentActor].AilmentList.Count)
            {
                Debug.Log("There are no more ailments in this list. Starting turn...");
                battleSubstate = BattleSubstate.Turn;
                Debug.Log("laos " + battleSubstate);
                BeginEnemyTurn();
                currentAilment = 0;
            }
            else
            {
                if (!instantiatedFoes[currentActor].AilmentList[currentAilment].activeBeforeTurn)
                {
                    Debug.Log("Ailment #" + currentAilment + " does not cause sufferbefore!");
                }
                else
                {
                    textBox.text = instantiatedFoes[currentActor] + " is " + instantiatedFoes[currentActor].AilmentList[currentAilment] + "!";
                    instantiatedFoes[currentActor].AilmentList[currentAilment].SufferBefore(instantiatedFoes[currentActor]);
                }
                currentAilment += 1;
                Debug.Log("ENEMY TIMETONEXTROUND UPDATED - PreTurnAilments - " + battleSubstate);
                timeToNextRound = 1.0f;
            }
        }
        else
        {
            Debug.Log("What are you doing here?");
        }
    }

    public void BeginPlayerTurn()
    { // use when player returns to screen, or is starting a turn with a new actor
        Debug.Log("BeginPlayerTurn() called. CurrentActor = " + currentActor + ". Turn Substate = " + battleSubstate);

        if (EvalBattleEnd()) return;

        // if party member 0 can't move at the start of the round, check for the next available actor
        /*
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
        {
            if ((GameManager.singleton.playerParty[currentActor] == null) 
                    || (!GameManager.singleton.playerParty[currentActor].canMove) 
                            || (GameManager.singleton.playerParty[currentActor].isDowned) 
                                || (GameManager.singleton.playerParty[currentActor].isStunned)
                                    || (GameManager.singleton.playerParty[currentActor].isGuarding))
            {
                currentActor += 1;   
            }
            else
            {
                break;
            }
        }
        */

        if (battleSubstate == BattleSubstate.PreTurn)
        {
            //Debug.Log("The player is waiting for preturn ailments");
            PreTurnAilments();
        }
        else if (battleSubstate == BattleSubstate.Turn)
        {
            //Debug.Log("The player can move now");
            currentAilment = 0;
            BeginPlayerAction();
        }
        else if (battleSubstate == BattleSubstate.PostTurn)
        {
            //Debug.Log("The player is waiting for postturn ailments");
            PostTurnAilments();
        }
    }

    public void BeginPlayerAction()
    { // use when player begins to move.
        Debug.Log("BeginPlayerAction() called. CurrentActor = " + currentActor + ". Turn Substate = " + battleSubstate);
        textBox.text = "What will " + playerCharacterName[currentActor].text + " do?";

        // enable action box
        actionBox.SetActive(true);
        eventSystem.SetSelectedGameObject(aDefault);

        // disable every other box
        reservesOrEnemiesBox.SetActive(false);
        targetSingleBox.SetActive(false);
        targetGroupBox.SetActive(false);
        skillBox.SetActive(false);
        guardBox.SetActive(false);
        passBox.SetActive(false);
        switch1Box.SetActive(false);
        switch2Box.SetActive(false);
        //itemBox.SetActive(false);
        escapeBox.SetActive(false);

        // disable battle cursor
        HideBattleCursor();
    }

    void PostTurnAilments()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            Debug.Log("The player is suffering from postturn ailments - checking index " + currentAilment + " vs ailment count " + GameManager.singleton.playerParty[currentActor].AilmentList.Count);

            
            if (currentAilment >= GameManager.singleton.playerParty[currentActor].AilmentList.Count)
            {
                Debug.Log("There are no more ailments in this list. Starting turn...");
                NextActor(true);
                currentAilment = 0;
            }
            else
            {
                if (!GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment].activeAfterTurn)
                {
                    Debug.Log("Ailment #" + currentAilment + " does not cause sufferafter!");
                }
                else
                {
                    textBox.text = GameManager.singleton.playerParty[currentActor] + " is " + GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment] + "!";
                    GameManager.singleton.playerParty[currentActor].AilmentList[currentAilment].SufferAfter(GameManager.singleton.playerParty[currentActor]);
                }
                currentAilment += 1;
            }
            Debug.Log("TIMETONEXTROUND UPDATED - PostTurnAilments");
            timeToNextRound = 1.0f;
        }
        else if (battleState == BattleState.EnemyTurn)
        {
            Debug.Log("The enemy is suffering from postturn ailments - checking index " + currentAilment + " vs ailment count " + instantiatedFoes[currentActor].AilmentList.Count);

            if (currentAilment >= instantiatedFoes[currentActor].AilmentList.Count)
            {
                Debug.Log("There are no more ailments in this list. Starting turn...");
                NextActor(true);
                currentAilment = 0;
            }
            else
            {
                if (!instantiatedFoes[currentActor].AilmentList[currentAilment].activeAfterTurn)
                {
                    Debug.Log("Ailment #" + currentAilment + " does not cause sufferafter!");
                }
                else
                {
                    textBox.text = instantiatedFoes[currentActor] + " is " + instantiatedFoes[currentActor].AilmentList[currentAilment] + "!";
                    instantiatedFoes[currentActor].AilmentList[currentAilment].SufferAfter(instantiatedFoes[currentActor]);
                }
                currentAilment += 1;
            }
            if (enemyPresses.currentTurns > 0)
            {
                Debug.Log("TIMETONEXTROUND UPDATED - PostTurnAilments");
                timeToNextRound = 1.0f;
            }
        }
    }

    public int HowManyActive()
    { // checks how many active player party members there are (add enemyTurn version?)
        int numOfActive = 0;
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i += 1)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {
                numOfActive += 1;
            }
        }
        
        return numOfActive;
    }

    void NextActor(bool _increment)
    { // cycle through actors for next turn. use increment = false to check if current actor is down without incrementing to the next.
        Debug.Log("NextActor() called. " + HowManyActive());
        battleSubstate = BattleSubstate.PreTurn; // make sure that whatever happens, the next actor starts in preturn
        currentAilment = 0;
        bool newActorFound = false;
        if (_increment)
        {
            currentActor += 1;
        }
        int countDown = 0; // if this goes past 30 (meaning definitely no available actors), end turn
        while (!newActorFound)
        {
            countDown += 1;
            if (countDown > 30)
            { // it has cycled through too many times and can't find anyone, therefore everybody is blocking/can't move
                Debug.Log("Can't find active actors. Swapping sides.");
                SwitchState();
                break;
            }
            if (battleState == BattleState.PlayerTurn)
            {
                if (playerPresses.currentTurns <= 0)
                {
                    Debug.Log("Player run out of press turns. Swapping sides.");
                    SwitchState();
                    break;
                }
                if (HowManyActive() <= 0)
                {
                    Debug.Log("0 active actors on player side. Swapping sides.");
                    SwitchState();
                    break;
                }
                if (currentActor > 3)
                { // return to beginning of party array
                    currentActor = 0;
                }
                if ((GameManager.singleton.playerParty[currentActor] == null) 
                    || (!GameManager.singleton.playerParty[currentActor].canMove) 
                        || (GameManager.singleton.playerParty[currentActor].isDowned) 
                            || (GameManager.singleton.playerParty[currentActor].isStunned)
                                || (GameManager.singleton.playerParty[currentActor].isGuarding))
                {
                    if (GameManager.singleton.playerParty[currentActor] != null)
                    {
                        Debug.Log("Skipping over: " + GameManager.singleton.playerParty[currentActor].seenName);
                    }
                    currentActor += 1;
                }
                else
                {
                    newActorFound = true;
                }
            }
            if (battleState == BattleState.EnemyTurn)
            {
                if (enemyPresses.currentTurns <= 0)
                {
                    Debug.Log("Enemy run out of press turns. Swapping sides.");
                    SwitchState();
                    break;
                }
                if (currentActor > 6)
                { // return to beginning of enemy array
                    currentActor = 0;
                }
                if ((GameManager.singleton.currentEncounter.encounter[currentActor] == null) 
                    || (!instantiatedFoes[currentActor].canMove) 
                        || (instantiatedFoes[currentActor].isDowned) 
                            || (instantiatedFoes[currentActor].isStunned)
                                || (instantiatedFoes[currentActor].isGuarding))
                {
                    if (instantiatedFoes[currentActor] != null)
                    {
                        Debug.Log("Skipping over: " + instantiatedFoes[currentActor].name);
                    }
                    currentActor += 1;
                }
                else
                {
                    newActorFound = true;
                }
            }
        }
        //Debug.Log("Current actor is now: " + GameManager.singleton.playerParty[currentActor].seenName);
        //TurnCooldown();
    }

    void BeginEnemyTurn()
    { // use when starting a turn with a new actor while its the enemy turn
        Debug.Log("BeginEnemyTurn() called. CurrentActor = " + currentActor + ". Turn Substate = " + battleSubstate);

        if (EvalBattleEnd()) return;

        if (battleSubstate == BattleSubstate.PreTurn)
        {
            Debug.Log("The player is waiting for preturn ailments");
            PreTurnAilments();
        }
        else if (battleSubstate == BattleSubstate.Turn)
        {
            Debug.Log("The player can move now");
            currentAilment = 0;
            BeginEnemyAction();
        }
        else if (battleSubstate == BattleSubstate.PostTurn)
        {
            Debug.Log("The player is waiting for postturn ailments");
            PostTurnAilments();
        }
    }

    void BeginEnemyAction()
    {
        Debug.Log("BeginEnemyAction() called. CurrentActor = " + currentActor + ". Turn Substate = " + battleSubstate);

        // TODO: retrieve battle info

        BattleInfo battleInfo = new BattleInfo();

        battleInfo.allyFoes = instantiatedFoes;
        battleInfo.enemyHeroes = GameManager.singleton.playerParty;

        // Decide battle
        enemyPresses.currentTurns -= instantiatedFoes[currentActor].DecideBattleAction(battleInfo, textBox);

        ActionTaken();
    }

    public void ActionTaken()
    { // called after any action is taken. do SufferAfter() for each ailment on current actor
        actionBox.SetActive(false);
        reservesOrEnemiesBox.SetActive(false);
        targetSingleBox.SetActive(false);
        targetGroupBox.SetActive(false);
        skillBox.SetActive(false);
        guardBox.SetActive(false);
        passBox.SetActive(false);
        switch1Box.SetActive(false);
        switch2Box.SetActive(false);
        //itemBox.SetActive(false);
        escapeBox.SetActive(false);

        battleSubstate = BattleSubstate.PostTurn;
        currentAilment = 0;
        Debug.Log("TIMETONEXTROUND UPDATED - ActionTaken");
        timeToNextRound = 1.0f;

    }
    
    void SwitchState()
    { // if round is over, switch
        currentActor = 0;

        if (battleState == BattleState.PlayerTurn)
        {
            Debug.Log("SWITCHING STATE TO ENEMY TURN");
            battleState = BattleState.EnemyTurn;
            //currentActor = -1;
            //Debug.Log("IF IT FROZE AGAIN IT WAS PROBABLY HERE. - PLAYER TURN");
            //NextActor(true);
            BeginRound();
        }
        else if (battleState == BattleState.EnemyTurn)
        {
            Debug.Log("SWITCHING STATE TO PLAYER TURN");
            battleState = BattleState.PlayerTurn;
            //currentActor = -1;
            //Debug.Log("IF IT FROZE AGAIN IT WAS PROBABLY HERE. - ENEMY TURN");
            //NextActor(true);
            BeginRound();
        }
        else
        {
            Debug.Log("What? How did you reach here? I haven't added a third side yet.");
        }
    }

    void SwitchState(int aWhichSide)
    {   // for if you want a switch to a specific side
        currentActor = 0;

        if (aWhichSide == 1)
        {   // switch to player side
            Debug.Log("SWITCHING STATE TO PLAYER TURN");
            battleState = BattleState.PlayerTurn;
            BeginRound();
        }
        else if (aWhichSide == 2)
        {   // switch to enemy side
            Debug.Log("SWITCHING STATE TO ENEMY TURN");
            battleState = BattleState.EnemyTurn;
            BeginRound();
        }
        else
        {
            Debug.Log("What? How did you reach here? I haven't added a third side yet.");
        }

    }

    public void TurnCooldown()
    { // called after every actor turn..
        Debug.Log("TurnCooldown() called.");

        Debug.Log("TIMETONEXTROUND UPDATED - TurnCooldown");
        timeToNextRound = 1.0f;
        //battleSubstate = BattleSubstate.PreTurn;

        singleDisplay.displayReserves = false;
        groupDisplay.displayReserves = false;
    }

    public void DoIDisplayReserves(bool reservesChosen)
    { // check if reserves are displayed when using team-wide moves
        PlaySound(1);
        if (reservesChosen)
        {
            singleDisplay.displayReserves = true;
            groupDisplay.displayReserves = true;
        }
        else
        {
            singleDisplay.displayReserves = false;
            groupDisplay.displayReserves = false;
        }
        ShowTargets(currentlySelectedMove.hitsAll);
    }

    public void ShowTargets(bool moveHitsAll)
    { // show targets when move is selected
        //RememberSelection();

        reservesOrEnemiesBox.SetActive(false);
        if (moveHitsAll)
        {
            targetGroupBox.SetActive(true);
            groupDisplay.DisplayTargets();
            eventSystem.SetSelectedGameObject(sk_TG_Default);
        }
        else
        {
            targetSingleBox.SetActive(true);
            singleDisplay.DisplayTargets();
            eventSystem.SetSelectedGameObject(sk_TS_Default);
        }
    
    }

    // Other functionality outside of the main loop here:

    // SKILL

    public void UseMove(int numOfMove)
    { // player using move function

        if (numOfMove < 2)
        { // use basic skill
            if (GameManager.singleton.playerParty[currentActor].basicAttacks[numOfMove] != null)
            {
                currentlySelectedMove = GameManager.singleton.playerParty[currentActor].basicAttacks[numOfMove];
                
                Debug.Log("USING MOVE: " + currentlySelectedMove.name);
                PlaySound(1);
                textBox.text = currentlySelectedMove.name + ": " + currentlySelectedMove.moveDescription;


                skillBox.SetActive(false);
            }
            else
            {
                Debug.Log("ERROR: NO MOVE SELECTED");
                PlaySound(2);
                return;
            }
        }
        else
        { // use advanced skill
            if (GameManager.singleton.playerParty[currentActor].moveList[numOfMove-2] != null)
            {
                currentlySelectedMove = GameManager.singleton.playerParty[currentActor].moveList[numOfMove-2];

                Debug.Log("USING MOVE: " + currentlySelectedMove.name);
                PlaySound(1);
                textBox.text = currentlySelectedMove.name + ": " + currentlySelectedMove.moveDescription;


                skillBox.SetActive(false);
            }
            else
            {
                Debug.Log("ERROR: NO MOVE SELECTED AT INDEX " + (numOfMove-2));
                PlaySound(2);
                return;
            }
        }

        // if healing..
        if (!currentlySelectedMove.isDamaging)
        {
            reservesOrEnemiesBox.SetActive(true);
            eventSystem.SetSelectedGameObject(sk_ROE_Default);
        }
        else
        {
            singleDisplay.displayReserves = false;
            groupDisplay.displayReserves = false;

            ShowTargets(currentlySelectedMove.hitsAll);
        }

    }

    public void Skill(int chosenIndex)
    {
        List<Entity> skillTargets = new List<Entity>();

        if (chosenIndex < 4) // targeting allies
        {
            if (currentlySelectedMove.hitsAll)
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
                    PlaySound(2);
                    return;
                }
            }
        }
        else // targeting enemies
        {
            if (groupDisplay.displayReserves)
            { // if reserves are showing, add ally to skillTargets
                if (currentlySelectedMove.hitsAll)
                {
                    for (int i = 0; i < GameManager.singleton.playerReserves.Length; i += 1)
                    {
                        //originally was GameManager.singleton.playerReserves
                        skillTargets.Add(GameManager.singleton.playerReserves[chosenIndex-4]);
                    }
                }
                else
                {
                    if (GameManager.singleton.playerReserves[chosenIndex-4] != null)
                    {   
                        skillTargets.Add(instantiatedFoes[chosenIndex-4]);
                    }
                    else
                    {
                        PlaySound(2);
                        return;
                    }
                }
            }
            else
            { // if not, add enemy to skillTargets
                if (currentlySelectedMove.hitsAll)
                {
                    for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
                    {
                        // change to instantiateFoes
                        skillTargets.Add(instantiatedFoes[i]);
                    }
                }
                else
                {
                    if (GameManager.singleton.currentEncounter.encounter[chosenIndex-4] != null)
                    {   
                        skillTargets.Add(instantiatedFoes[chosenIndex-4]);
                    }
                    else
                    {
                        PlaySound(2);
                        return;
                    }
                }
            }

        }

        if (GameManager.singleton.playerParty[currentActor].currentMP < currentlySelectedMove.ManaPointCost)
        {
            textBox.text = "Insufficient MP!";
            PlaySound(2);
            return;
        }
        else
        {
            PlaySound(1);
            textBox.text = GameManager.singleton.playerParty[currentActor].seenName + " uses " + currentlySelectedMove.name + "!";
            Entity[] targetsArray = skillTargets.ToArray();
            playerPresses.currentTurns -= currentlySelectedMove.UseMove(GameManager.singleton.playerParty[currentActor], targetsArray);

            ActionTaken();
        }
    }

    public void SetBattleCursor(int chosenIndex)
    { // called when selecting single enemy
        Debug.Log("SetBattleCursor() called.");

        battleCursor.SetActive(true);

        Entity chosenEntity;

        // place cursor on position of selection + extra Y height

        if (chosenIndex < 4)
        {
            if (GameManager.singleton.playerParty[chosenIndex] != null)
            {   
                switch (chosenIndex)
                {
                    case 0:
                        battleCursor.transform.position = new Vector3(-6f, 6f, 3.5f);
                        break;
                    case 1:
                        battleCursor.transform.position = new Vector3(-2f, 6f, 3.5f);
                        break;
                    case 2:
                        battleCursor.transform.position = new Vector3(2f, 6f, 3.5f);
                        break;
                    case 3:
                        battleCursor.transform.position = new Vector3(6f, 6f, 3.5f);
                        break;
                    default:
                        Debug.Log("How did this even happen?");
                        battleCursor.SetActive(false);
                        break;
                }
                chosenEntity = GameManager.singleton.playerParty[chosenIndex];
            }
            else
            {
                Debug.Log("SetBattleCursor() has hidden the battle cursor for index " + chosenIndex);
                battleCursor.SetActive(false);
                return;
            }
        }
        else
        {
            if (instantiatedFoes[chosenIndex-4] != null)
            {   
                switch (chosenIndex)
                {
                    case 4:
                        battleCursor.transform.position = new Vector3(0f, 6f, 0f);
                        break;
                    case 5:
                        battleCursor.transform.position = new Vector3(-2f, 6f, 0f);
                        break;
                    case 6:
                        battleCursor.transform.position = new Vector3(2f, 6f, 0f);
                        break;
                    case 7:
                        battleCursor.transform.position = new Vector3(-3.5f, 6f, 0f);
                        break;
                    case 8:
                        battleCursor.transform.position = new Vector3(3.5f, 6f, 0f);
                        break;
                    case 9:
                        battleCursor.transform.position = new Vector3(-5f, 6f, 0f);
                        break;
                    case 10:
                        battleCursor.transform.position = new Vector3(5f, 6f, 0f);
                        break;
                    default:
                        Debug.Log("How did this even happen?");
                        battleCursor.SetActive(false);
                        break;
                }
                chosenEntity = instantiatedFoes[chosenIndex-4];
            }
            else
            {
                Debug.Log("SetBattleCursor() has hidden the battle cursor for index " + (chosenIndex-4));
                battleCursor.SetActive(false);
                return;
            }
        }

        // check affinity of move compared to resistance of enemy

        int enumVal;
        int dmgValue = (int)currentlySelectedMove.elementType;

        switch (dmgValue)
        {
            case 0: // fae (always x1.5)
                Debug.Log("currentlySelectedMove is fae type.");
                battleCursorText.SetText("???");
                break;
            // PHYSICAL
            case 1: // slash
                Debug.Log("currentlySelectedMove is slash type.");
                enumVal = (int)chosenEntity.Slash;
                checkBattleCursorAffinity(enumVal);
                break;
            case 2: // strike
                Debug.Log("currentlySelectedMove is strike type.");
                enumVal = (int)chosenEntity.Strike;
                checkBattleCursorAffinity(enumVal);
                break;
            case 3: // pierce
                Debug.Log("currentlySelectedMove is pierce type.");
                enumVal = (int)chosenEntity.Pierce;
                checkBattleCursorAffinity(enumVal);
                break;

            // MAGICAL
            case 4: // air
                Debug.Log("currentlySelectedMove is air type.");
                enumVal = (int)chosenEntity.Air;
                checkBattleCursorAffinity(enumVal);
                break;
            case 5: // fire
                Debug.Log("currentlySelectedMove is fire type.");
                enumVal = (int)chosenEntity.Fire;
                checkBattleCursorAffinity(enumVal);
                break;
            case 6: // earth
                Debug.Log("currentlySelectedMove is earth type.");
                enumVal = (int)chosenEntity.Earth;
                checkBattleCursorAffinity(enumVal);
                break;
            case 7: // life
                Debug.Log("currentlySelectedMove is life type.");
                enumVal = (int)chosenEntity.Life;
                checkBattleCursorAffinity(enumVal);
                break;
            case 8: // ice
                Debug.Log("currentlySelectedMove is ice type.");
                enumVal = (int)chosenEntity.Ice;
                checkBattleCursorAffinity(enumVal);
                break;
            case 9: // lightning
                Debug.Log("currentlySelectedMove is lightning type.");
                enumVal = (int)chosenEntity.Lightning;
                checkBattleCursorAffinity(enumVal);
                break;
            case 10: // noise
                Debug.Log("currentlySelectedMove is noise type.");
                enumVal = (int)chosenEntity.Noise;
                checkBattleCursorAffinity(enumVal);
                break;
            default:
                Debug.Log("currentlySelectedMove is a completely unknown type.");
                battleCursorText.SetText("Come again?");
                break;
        }
    }

    void checkBattleCursorAffinity(int enumValue)
    {
        switch (enumValue)
        {
            // for cases below, send message that turns should be changed
            case 0: // super-effective
                battleCursorText.SetText("Super-Effective!");
                break;
            case 1: // normal-effective
                battleCursorText.SetText("Effective");
                break;
            case 2: // weak-effective
                battleCursorText.SetText("Ineffective");
                break;
            case 3: // immune
                battleCursorText.SetText("Immune");
                break;
            case 4: // deflect
                battleCursorText.SetText("Deflect");
                break;
            case 5: // drain
                battleCursorText.SetText("Drain");
                break;
            default:
                Debug.Log("How");
                break;
        }
    }

    public void HideBattleCursor()
    { // called when confirming move, backing out of target selection
        //Debug.Log("HideBattleCursor() called.");
        battleCursor.SetActive(false);
    }

    

    // GUARD

    public void Guard()
    {
        playerPresses.currentTurns -= 2;
        GameManager.singleton.playerParty[currentActor].isGuarding = true;
        textBox.text = GameManager.singleton.playerParty[currentActor].seenName + " begins guarding!";
        ActionTaken();
    }

    // PASS

    public void Pass()
    {
        playerPresses.currentTurns -= 1;
        ActionTaken();
        textBox.text = "Passing to next ally..";
    }

    // SWITCH

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
                PlaySound(2);
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
                PlaySound(2);
                return;
            }
        }

        PlaySound(1);
        switch1Box.SetActive(false);
        switch2Box.SetActive(true);
        storedPos1 = chosenIndex;
    }

    public void PickWhoToSwap2(int chosenIndex)
    {
        if (storedPos1 == chosenIndex)
        {
            PlaySound(2);
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
                if (HowManyActive() <= 1 && partyCharSelected)
                {
                    textBox.text = "Cannot have an empty party.";
                    PlaySound(2);
                    return;
                }
            }
        }

        PlaySound(1);
        storedPos2 = chosenIndex;
        switch2Box.SetActive(false);
        Switch();
    }

    void Switch()
    {
        playerPresses.currentTurns -= 1;
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
        InitialiseActors();
        // end turn
        ActionTaken();
    }

    // ITEM



    // ESCAPE

    public void Escape()
    {
        if (GameManager.singleton.currentEncounter.isBoss)
        {
            textBox.text = "You can't escape!";
            TurnCooldown();
        }
        else
        {
            // tally up all remaining enemy HP then make it a percentage
            float totalHP = 0f;
            float remainingHP = 0f;

            foreach (Foe foe in instantiatedFoes)
            {
                if (foe != null)
                {
                    totalHP += (float)foe.HealthPoints.Value;
                    remainingHP += (float)foe.currentHP;
                }
            }

            float percentage = (remainingHP / totalHP) * 100;

            if (Consts.RNG_ESCAPE_PROC)
            {
                // use percentage as a chance to escape + 25% (e.g. 70% remaining = 30% + 25% = 55% chance of escape. escape guaranteed after 25% remaining)
                percentage = (100 - percentage) + 25;
                if (Consts.ProcChance((int)percentage))
                {
                    OnBattleEscape();
                }
                else
                {
                    textBox.text = "The enemies are too healthy for you to escape!";
                    playerPresses.currentTurns -= 14;
                    ActionTaken();
                }
            }
            else
            {
                // if percentage below 50, succesfully escape, else fail and begin enemy turn
                if (percentage < 50)
                {
                    // success
                    OnBattleEscape();
                }
                else
                {
                    // fail, begin enemy turn
                    textBox.text = "The enemies are too healthy for you to escape!";
                    playerPresses.currentTurns -= 14;
                    ActionTaken();
                }
            }
        }
    }

    void OnBattleEscape()
    {
        // if no RNG, enemy that initiated combat disappears until next day

        battleOutcome = 0;
        textBox.text = "You ran away!";
        PlaySound(3);
        timeToEndBattle = 2f;
    }

    void OnBattleWin()
    {
        // if no RNG, enemy that initiated combat disappears until next day

        battleOutcome = 1;
        textBox.text = "You won the battle!";
        //PlaySound(3);
        timeToEndBattle = 2f;
    }

    void OnBattleLose()
    {
        // "With your party unable to continue further, you retreat for the day."

        battleOutcome = 2;
        textBox.text = "You were defeated.";
        //PlaySound(3);
        timeToEndBattle = 2f;
    }

    void EndBattle()
    {
        Debug.Log("EndBattle() called");
        GameManager.singleton.ExitBattle(battleOutcome);
        GameManager.singleton.EnterLastActive();
    }
}

    /*
        public void DoIDisplayReserves(bool reservesChosen)
    {
        battleHandler.PlaySound(1);
        if (reservesChosen)
        {
            battleHandler.singleDisplay.displayReserves = true;
            battleHandler.groupDisplay.displayReserves = true;
        }
        else
        {
            battleHandler.singleDisplay.displayReserves = false;
            battleHandler.groupDisplay.displayReserves = false;
        }
        ShowTargets(battleHandler.currentlySelectedMove.hitsAll);
    }
    */

    /*
    public void RememberSelection()
    { // remember the last box selected
        if (!GameManager.singleton.retainLastSelected)
        {
            Debug.Log("does not retain last selected object");
            return;
        }

        Debug.Log("currentSelectedGameObject.transform.parent.name is: " + eventSystem.currentSelectedGameObject.transform.parent.name);

        if (eventSystem.currentSelectedGameObject.transform.parent.name == "ActionBox")
        {
            aDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Action Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "SkillBox")
        {
            skDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Skill Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "SkillReservesOrEnemiesBox")
        {
            sk_ROE_Default = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Skill ROE Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "SkillTargetSingleBox")
        {
            sk_TS_Default = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Skill TARGET SINGLE Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "SkillTargetGroupBox")
        {
            sk_TG_Default = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Skill TARGET GROUP Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "GuardBox")
        {
            gDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Guard Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "PassBox")
        {
            pDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Pass Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "Switch1Box")
        {
            sw1Default = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Switch 1 Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "Switch2Box")
        {
            sw2Default = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Switch 2 Default");
        } 
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "ItemBox")
        {
            iDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Item Default");
        } 
        
        if (eventSystem.currentSelectedGameObject.transform.parent.name == "EscapeBox")
        {
            eDefault = eventSystem.currentSelectedGameObject;
            Debug.Log("last selected object retained - Escape Default");
            
        } 
        
    }
    */
    /*
    public void ChooseActiveUI(int fedInt)
    { // set selected UI button
        RememberSelection();

        switch (fedInt)
        {
            case 0: // skill
                eventSystem.SetSelectedGameObject(skDefault);
                break;
            case 1: // guard
                eventSystem.SetSelectedGameObject(gDefault);
                break;
            case 2: // pass
                eventSystem.SetSelectedGameObject(pDefault);
                break;
            case 3: // switch
                eventSystem.SetSelectedGameObject(sw1Default);
                break;
            case 4: // item
                eventSystem.SetSelectedGameObject(iDefault);
                break;
            case 5: // escape
                eventSystem.SetSelectedGameObject(eDefault);
                break;
            case 10: // skill - reserves or enemies
                eventSystem.SetSelectedGameObject(sk_ROE_Default);
                break;
            case 11: // skill - target single
                eventSystem.SetSelectedGameObject(sk_TS_Default);
                break;
            case 12: // skill - target group
                eventSystem.SetSelectedGameObject(sk_TG_Default);
                break;
            default:
                Debug.Log("You inputted straight garbage bro");
                break;
        }

    }
    */

            // skill
        /*
        public void UseMove(int numOfMove)
        { // player using move function

            if (numOfMove < 2)
            { // use basic skill
                if (GameManager.singleton.playerParty[currentActor].basicAttacks[numOfMove] != null)
                {
                    currentlySelectedMove = GameManager.singleton.playerParty[currentActor].basicAttacks[numOfMove];
                    
                    Debug.Log("USING MOVE: " + currentlySelectedMove.name);
                    PlaySound(1);
                    textBox.text = currentlySelectedMove.name + ": " + currentlySelectedMove.moveDescription;


                    skillBox.SetActive(false);
                }
                else
                {
                    Debug.Log("ERROR: NO MOVE SELECTED");
                    PlaySound(2);
                    return;
                }
            }
            else
            { // use advanced skill
                if (GameManager.singleton.playerParty[currentActor].moveList[numOfMove-2] != null)
                {
                    currentlySelectedMove = GameManager.singleton.playerParty[currentActor].moveList[numOfMove-2];

                    Debug.Log("USING MOVE: " + currentlySelectedMove.name);
                    PlaySound(1);
                    textBox.text = currentlySelectedMove.name + ": " + currentlySelectedMove.moveDescription;


                    skillBox.SetActive(false);
                }
                else
                {
                    Debug.Log("ERROR: NO MOVE SELECTED");
                    PlaySound(2);
                    return;
                }
            }

            // if healing..
            if (!currentlySelectedMove.isDamaging)
            {
                reservesOrEnemiesBox.SetActive(true);
                eventSystem.SetSelectedGameObject(sk_ROE_Default);
            }
            else
            {
                singleDisplay.displayReserves = false;
                groupDisplay.displayReserves = false;

                ShowTargets(currentlySelectedMove.hitsAll);
            }

        }

        public void DoIDisplayReserves(bool reservesChosen)
        {
            PlaySound(1);
            if (reservesChosen)
            {
                singleDisplay.displayReserves = true;
                groupDisplay.displayReserves = true;
            }
            else
            {
                singleDisplay.displayReserves = false;
                groupDisplay.displayReserves = false;
            }
            ShowTargets(currentlySelectedMove.hitsAll);
        }

        // if multihit..
        void ShowTargets(bool moveHitsAll)
        {
            RememberSelection();

            reservesOrEnemiesBox.SetActive(false);
            if (moveHitsAll)
            {
                targetGroupBox.SetActive(true);
                groupDisplay.DisplayTargets();
                eventSystem.SetSelectedGameObject(sk_TG_Default);
            }
            else
            {
                targetSingleBox.SetActive(true);
                singleDisplay.DisplayTargets();
                eventSystem.SetSelectedGameObject(sk_TS_Default);
            }
        }

        public void Skill(int chosenIndex)
        {
            List<Entity> skillTargets = new List<Entity>();

            if (chosenIndex < 4)
            {
                if (currentlySelectedMove.hitsAll)
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
                        PlaySound(2);
                        return;
                    }
                }
            }
            else
            {
                if (groupDisplay.displayReserves)
                { // if reserves are showing, add ally to skillTargets
                    if (currentlySelectedMove.hitsAll)
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
                            PlaySound(2);
                            return;
                        }
                    }
                }
                else
                { // if not, add enemy to skillTargets
                    if (currentlySelectedMove.hitsAll)
                    {
                        for (int i = 0; i < GameManager.singleton.currentEncounter.encounter.Length; i += 1)
                        {
                            skillTargets.Add(instantiatedFoes[i]);
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
                            PlaySound(2);
                            return;
                        }
                    }
                }

            }

            PlaySound(1);
            textBox.text = GameManager.singleton.playerParty[currentActor].seenName + " uses " + currentlySelectedMove.name + "!";
            Entity[] targetsArray = skillTargets.ToArray();
            currentlySelectedMove.UseMove(GameManager.singleton.playerParty[currentActor], targetsArray);

            // update healthbars, also add background healthbars which catch up to the foreground ones
            UpdateBars();
        }
        */

        // guard
        /*
        public void Guard()
        {
            playerPresses.currentTurns -= 2;
            GameManager.singleton.playerParty[currentActor].isGuarding = true;
            textBox.text = GameManager.singleton.playerParty[currentActor].seenName + " begins guarding!";
            IsTurnOver();
        }
        */

        // pass
        /*
        public void Pass()
        {
            playerPresses.currentTurns -= 1;
            IsTurnOver();
            textBox.text = "Passing to " + GameManager.singleton.playerParty[currentActor].seenName + ".";
        }
        */

        // switch
        /*
        public void ShowReserves()
        { // display party and reserves when switch is pressed
            switch1Display.DisplayTargets();
            switch2Display.DisplayTargets();
        }

        bool partyCharSelected;
        int storedPos1 = -1;
        int storedPos2 = -1;

        public void ClearStoredChar()
        {
            partyCharSelected = false;
            storedPos1 = -1;
            storedPos2 = -1;
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
                    PlaySound(2);
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
                    PlaySound(2);
                    return;
                }
            }

            PlaySound(1);
            switch1Box.SetActive(false);
            switch2Box.SetActive(true);
            storedPos1 = chosenIndex;
        }

        public void PickWhoToSwap2(int chosenIndex)
        {
            if (storedPos1 == chosenIndex)
            {
                PlaySound(2);
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
                    if (HowManyActive() <= 1 && partyCharSelected)
                    {
                        textBox.text = "Cannot have an empty party.";
                        PlaySound(2);
                        return;
                    }
                }
            }

            PlaySound(1);
            storedPos2 = chosenIndex;
            switch2Box.SetActive(false);
            Switch();
        }


        void Switch()
        {
            playerPresses.currentTurns -= 1;
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
            InitialiseActors();
            // end turn
            IsTurnOver();
        }
        */

        // item

        // escape
        /*
        public void Escape()
        {
            if (GameManager.singleton.currentEncounter.isBoss)
            {
                textBox.text = "You can't escape!";
                TurnCooldown();
            }
            else
            {
                textBox.text = "You ran away!";
                PlaySound(3);
                timeToEndBattle = 1f;
            }
        }
        */