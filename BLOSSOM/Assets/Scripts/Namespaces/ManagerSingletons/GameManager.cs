using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Kryz.CharacterStats;
using Naninovel;
using Vaz.Constants.Consts;

namespace Vaz.ManagerSingletons
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager singleton { get; private set; }
        [Tooltip("Used by Naninovel to get encounters/items")]
        public Databank databank;
        public string playerName = "Lorelei";
        public Hero[] playerParty = new Hero[4];
        public Hero[] playerReserves = new Hero[7];
        [SerializeField] private Hero defaultChar;

        [HideInInspector] public Encounter currentEncounter;
        [Space]
        [Tooltip("The dungeon prefab loaded every time you choose to enter the Superstructure. Hidden on battle start.")]
        public GameObject currentDungeon;
        public List<EnemyBehaviour> currentEnemies = new List<EnemyBehaviour>();
        private GameObject playerObj;
        private DungeonMovement dungeonMovement;
        private GameObject cameraObj;
        private DistortCamera distortCam;
        // keep track of player's last active scene and position so that scene can be re-entered
        private string lastActiveScene;

        [Header("Hideable UI")]
        // determines whether or not the UI is currently being shown
        [HideInInspector] public bool canPause;
        public CanvasGroup canvasGroup;
        public GameObject fpsCounter;
        public GameObject dateDisplay;
        public GameObject daysLeftDisplay;
        [Tooltip("How many days left to defeat a domain boss.")]
        public Text textDaysLeft1;
        [Tooltip("'Days Left' written under the remaining days. Change to 'Day Left' at 1.")]
        public Text textDaysLeft2;
        public int daysLeft;
        public GameObject statusBusts;

        [Tooltip("Whether or not the FPS counter is allowed to be visible.")]
        public bool fpsVisible;
        [Tooltip("Whether or not the date is allowed to be visible.")]
        public bool dateVisible;
        [Tooltip("Whether or not the days counter is allowed to be visible.")]
        public bool daysLeftVisible;
        [Tooltip("Whether or not the status busts are allowed to be visible.")]
        public bool bustsVisible;

        [SerializeField] GameObject interactPrompt;
        public bool showInteractPrompt;
        public float interactTimeLeft = 0f;

        [Header("Date & Time")]

        public Text dateText;
        public Text dayText;
        [Space]
        public string weekday;
        public int dateDay;
        public int dateMonth;
        public int dateYear;

        [Header("Dungeon Movement")]

        public bool isInputBlocked;
        public bool canDungeonPause;
        public bool isDungeonPaused;

        [Header("Dungeon Teleport")]

        private float fadeSpeed = 5f;
        public char currentlyFading = 'U';
        public Vector3 newPosition;
        public Quaternion newRotation;
        [Tooltip("Should only be whatever rooms will be visible to the player upon teleporting.")]
        private List<GameObject> onTeleportEnable = new List<GameObject>();
        [Tooltip("Never change this one. It is set by dungeoninit.")]
        public List<GameObject> onTeleportDisable = new List<GameObject>();
        [Tooltip("ALSO never change this one. It is set by dungeoninit.")]
        public List<GameObject> onStartTeleportEnable = new List<GameObject>();
        private bool changeTeleportElements = false;

        [Header("Battle Stage")]
        public Material currentPlaneMat;

        public Material currentSkyboxMat;

        public AudioClip currentMusic;

        [Header("Audio")]
        GameObject managerObj;

        public AudioSource SFXplayer;

        public AudioSource BGMplayer;

        public AudioSource DIAplayer;

        [Header("Sound Effects")]
        public AudioClip battleStart;
        [SerializeField] AudioClip click;
        [SerializeField] AudioClip openMenu;
        [SerializeField] AudioClip closeMenu;
        [SerializeField] AudioClip confirm;

        [Header("Retain On Battle Enter")]
        public bool retainEnvironment;
        public bool retainMusic;

        [Header("Player Settings")]
        public bool retainLastSelected;

        [Header("Mouse Fix")]
        GameObject lastSelect;
        

        [Header("Inventory")]
        public List<KeyItem> invKeys = new List<KeyItem>(99);
        public List<Consumable> invConsumables = new List<Consumable>(99);
        public List<Boon> invAccessories = new List<Boon>(99);

        [Header("RNG")]
        [Tooltip("The encounters which can currently be found (changed by dungeoninit when travelling to another dungeon)")]
        public List<Encounter> possibleEncounters = new List<Encounter>(0);
        public List<int> possibleEncounterWeights = new List<int>(0);


        void Awake()
        {
            // ensures this remains a singleton
            if (singleton != null && singleton != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                singleton = this; 
            }

            DontDestroyOnLoad(this.gameObject);
            if (isArrayEmpty() == true)
            {
                playerParty[0] = defaultChar; // if party empty, Lorelei is placed in first position
            }

            // edit & toggle off UI on startup

            textDaysLeft1.text = "99";
            textDaysLeft2.text = "Days Left";
            ToggleUI(true, false, false, false);
            canPause = false;

            // disable that cunting mouse pt. 1

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lastSelect = this.gameObject;
        }

        void Update()
        {
            //Debug.Log("DaysRemaining: " + daysLeft);

            // disable that cunting mouse pt. 2

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelect);
            }
            else
            {
                //if ((lastSelect != EventSystem.current.currentSelectedGameObject) && !cInput.GetKey("Submit"))
                //{
                //    PlaySound(1);
                //}

                lastSelect = EventSystem.current.currentSelectedGameObject;
            }

            // check dungeon movement/pausing

            if (isDungeonPaused)
            {
                if (canDungeonPause)
                {
                    if (dungeonMovement != null)
                    {
                        dungeonMovement.canMove = false;
                    }
                }
                else
                {
                    isDungeonPaused = false;
                }
            }
            else
            {
                if (dungeonMovement != null)
                {
                    dungeonMovement.canMove = true;
                }
            }

            if (isInputBlocked)
            {
                if (dungeonMovement != null)
                {
                    dungeonMovement.canMove = false;
                    //Debug.Log("input is blocked: " + dungeonMovement.canMove);
                }
            }
            else
            {
                if (dungeonMovement != null)
                {
                    dungeonMovement.canMove = true;
                    //Debug.Log("input NOT blocked: " + dungeonMovement.canMove);
                }   
            }

            // check dungeon teleporting

            switch (currentlyFading)
            {
                case 'O':
                    isInputBlocked = true;
                    //Debug.Log("Fading Out");
                    //Debug.Log(Time.fixedDeltaTime * fadeSpeed);
                    canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                    if (canvasGroup.alpha == 1)
                    {
                        Teleport();
                        currentlyFading = 'I';
                    }
                    //Debug.Log("out: " + canvasGroup.alpha);
                    break;
                case 'I':
                    isInputBlocked = false;
                    //Debug.Log("Fading In");
                    //Debug.Log(Time.fixedDeltaTime * fadeSpeed);
                    canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                    if (canvasGroup.alpha == 0)
                    {
                        currentlyFading = 'U';
                    }
                    //Debug.Log("in: " + canvasGroup.alpha);
                    break;
            }

            if (interactPrompt != null)
            {
                if (interactTimeLeft > 0.0f)
                {
                    interactPrompt.SetActive(true);
                    interactTimeLeft -= Time.deltaTime;
                }
                else
                {
                    if (showInteractPrompt)
                    {
                        interactPrompt.SetActive(true);
                        interactTimeLeft = 0.05f;
                    }
                    else 
                    {
                        interactPrompt.SetActive(false);
                    }
                }

            }
        }

        public void PlaySound(int whichSound)
        {
            Debug.Log("GameManager - PlaySound called");
            switch (whichSound)
            {
                case 1:
                    SFXplayer.PlayOneShot(click, 1.0f);
                    break;
                case 2:
                    SFXplayer.PlayOneShot(openMenu, 1.0f);
                    break;
                case 3:
                    SFXplayer.PlayOneShot(closeMenu, 1.0f);
                    break;
                case 4:
                    SFXplayer.PlayOneShot(confirm, 1.0f);
                    break;
            }
        }

        public void PlaySound(AudioClip whichClip)
        {
            SFXplayer.PlayOneShot(whichClip, 1.0f);
        }

        public void SearchForPlayer()
        { // only called by other scripts when entering dungeon crawl scenes
            playerObj = GameObject.Find("/Player");
            dungeonMovement = playerObj.GetComponent(typeof(DungeonMovement)) as DungeonMovement;
            cameraObj = GameObject.Find("/Player/Main Camera");
            distortCam = cameraObj.GetComponent(typeof(DistortCamera)) as DistortCamera;
        }

        public void ForgetPlayer()
        { // only called when leaving dungeon crawl scenes
            playerObj = null;
            dungeonMovement = null;
            cameraObj = null;
            distortCam = null;
        }

        public void ChangeDate(int day, int month, int year, string weekday)
        { // Change Date (weekday manually changed)
            dateDay = day;
            dateMonth = month;
            dateYear = year;

            dateText.text = day + "/" + month;
            dayText.text = weekday;
            SetDaysLeft(daysLeft);
        }

        public void ChangeDate(int day, int month, int year, int remainingDays)
        { // Change Date (weekday automatically changed)
            dateDay = day;
            dateMonth = month;
            dateYear = year;
            daysLeft = remainingDays;

            dateText.text = day + "/" + month;
            SetDaysLeft(daysLeft);

            switch (weekday) 
            {
                case "Sunday":
                    weekday = "Monday";
                    break;
                case "Monday":
                    weekday = "Tuesday";
                    break;
                case "Tuesday":
                    weekday = "Wednesday";
                    break;
                case "Wednesday":
                    weekday = "Thursday";
                    break;
                case "Thursday":
                    weekday = "Friday";
                    break;
                case "Friday":
                    weekday = "Saturday";
                    break;
                case "Saturday":
                    weekday = "Sunday";
                    break;
                default:
                    Debug.LogError(weekday + " is not a recognised day of the week. How did this get here?");
                    BadBehaviourFound();
                    break;
            }
            
            dayText.text = weekday;
        }

        public void SetDaysLeft(int newAmount)
        {
            daysLeft = newAmount;
            textDaysLeft1.text = daysLeft.ToString();
            if (daysLeft == 1)
            {
                textDaysLeft1.text = "Final";
                textDaysLeft2.text = "Day";
            }
            else
            {
                textDaysLeft2.text = "Days";
            }

            if (daysLeft < 1)
            {
                Debug.Log("DaysLeft display turned off because it went below 0.");
                ToggleUI(fpsCounter.activeSelf, dateDisplay.activeSelf, false, statusBusts.activeSelf);
            }
        }

        public void StartTeleport()
        {
            currentlyFading = 'O';
        }

        public void StartTeleport(List<GameObject> _OnEnterEnable)
        {
            currentlyFading = 'O';

            onTeleportEnable = _OnEnterEnable;

            changeTeleportElements = true;
        }

        public void Teleport()
        {
            if (changeTeleportElements)
            {
                // deactivate all objects in OnEnterDisable

                foreach (GameObject go in onTeleportDisable)
                {
                    go.SetActive(false);
                }

                // next, activate all objects in onEnterEnable

                foreach (GameObject go in onTeleportEnable)
                {
                    go.SetActive(true);
                }

                changeTeleportElements = false;
            }

            //Debug.Log("Teleporting: " + player.name + "...");

            playerObj.transform.position = newPosition;
            playerObj.transform.rotation = newRotation;

            
            dungeonMovement.SetPos(newPosition, newRotation);

            Debug.Log("Teleported to position " + playerObj.transform.position + " and rotation " + playerObj.transform.rotation);

            isInputBlocked = false;

            dungeonMovement.EmptyBuffer();
        }

        private bool isArrayEmpty()
        {
            Debug.Log("checking for party members..");
            if (playerParty == null || playerParty.Length == 0) return true;
            for (int i = 0; i < playerParty.Length; i++)
            {
                if (playerParty[i] != null)
                {
                    Debug.Log("character(s) found in party.");
                    return false;
                }
            }
            Debug.Log("no characters found in party. assigning..");
            return true;
        }

        public void StartBattle(Encounter encounter)
        {
            // remember previous player scene
            lastActiveScene = SceneManager.GetActiveScene().name;

            SFXplayer.PlayOneShot(battleStart, 1.0f);

            dungeonMovement.canMove = false;
            distortCam.battleStarting = true;

            currentEncounter = encounter;

            if (retainEnvironment)
            {
                Debug.Log("NOTE: Environment is not overwritten");
            }
            else
            {
                currentPlaneMat = encounter.planeMat;
                currentSkyboxMat = encounter.skyboxMat;
                Debug.Log("currentSkyboxMat updated: " + currentSkyboxMat);
                //RenderSettings.skybox = currentSkyboxMat;
            }

            if (retainMusic)
            {
                Debug.Log("NOTE: Music is not overwritten");
            }
            else
            {
                currentMusic = encounter.music;
            }

        }

        public void EnterBattle()
        {
            // deactivate dungeon movement and dungeon layout
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
            //SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
            StartCoroutine(WaitForSceneLoad(SceneManager.GetSceneByName("BattleScene"), true));
            //Scene scene = SceneManager.GetSceneByBuildIndex(5); // BattleScene
            //SceneManager.SetActiveScene(scene);
            distortCam.battleStarting = false;

            // disable dungeon layout, dungeonmovement player, dungeon camera
            currentDungeon.SetActive(false);
            playerObj.SetActive(false);
            cameraObj.SetActive(false);

            ToggleUI(fpsVisible, dateVisible, daysLeftVisible, false);
        } 

        public void ExitBattle(int battleOutcome)
        {
            BGMplayer.Stop();

            // re-enable dungeon layout, dungeonmovement player, dungeon camera
            currentDungeon.SetActive(true);

            playerObj.SetActive(true);
            dungeonMovement.canMove = true;

            cameraObj.SetActive(true);

            ToggleUI(fpsVisible, dateVisible, daysLeftVisible, bustsVisible);

            //EnterLastActive();
            Scene scene = SceneManager.GetSceneByName(lastActiveScene);
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync("BattleScene");

            // finally, undo black screen if escaped or won

            if (battleOutcome == 0)
            {
                // escape
                Debug.Log("Battle ended via escaping.");
                currentlyFading = 'I';
            }
            else if (battleOutcome == 1)
            {
                // won
                Debug.Log("Battle ended via winning.");
                currentlyFading = 'I';

                if (currentEncounter.playVictoryScript)
                {
                    // I shouldn't need to teleport but I literally have no idea how to prevent the interact button from coming up. remove this when you learn how to actually program.
                    newPosition.Set(0, 0, 0);
                    newRotation.Set(0, 0, 0, GameManager.singleton.newRotation.w);
                    StartTeleport(GameManager.singleton.onStartTeleportEnable);

                    GameManager.singleton.isInputBlocked = true;
                    GameManager.singleton.canDungeonPause = false;
                    Debug.Log(this.name + " inputs turned off");

                    var inputManager = Engine.GetService<IInputManager>();
                    inputManager.ProcessInput = true;

                    var scriptPlayer = Engine.GetService<IScriptPlayer>();
                    scriptPlayer.PreloadAndPlayAsync(currentEncounter.script, label: currentEncounter.label).Forget();
                }
            }
            else if (battleOutcome == 2)
            {
                // lost
                Debug.Log("Battle ended via losing.");

                // teleport to beginning of dungeon
                newPosition.Set(0, 0, 0);
                newRotation.Set(0, 0, 0, GameManager.singleton.newRotation.w);
                StartTeleport(GameManager.singleton.onStartTeleportEnable);

                // end day
                GameManager.singleton.isInputBlocked = true;
                GameManager.singleton.canDungeonPause = false;
                Debug.Log(this.name + " inputs turned off");

                var inputManager = Engine.GetService<IInputManager>();
                inputManager.ProcessInput = true;

                var scriptPlayer = Engine.GetService<IScriptPlayer>();
                scriptPlayer.PreloadAndPlayAsync("Dungeon_Generic", label: "DefeatedInSuperstructure").Forget();
            }
            else
            {
                Debug.LogError("Battle ended with a strange outcome value. Are you sure you wanted this to happen?");
            }
        }

        public void EnterLastActive()
        {
            //SceneManager.LoadScene(lastActiveScene, LoadSceneMode.Additive);
            //StartCoroutine(WaitForSceneLoad(SceneManager.GetSceneByName(lastActiveScene)));


            //Scene scene = SceneManager.GetSceneByName(lastActiveScene);
            //SceneManager.SetActiveScene(scene);
        }

        public void EnterLastActiveSingle()
        {
            SceneManager.LoadScene(lastActiveScene, LoadSceneMode.Single);
        }

        public void SetLastActive(string newScene)
        {
            lastActiveScene = newScene;
        }
        
        public IEnumerator WaitForSceneLoad(Scene scene, bool loadingBattleScene)
        {
            while(!scene.isLoaded)
            {
                yield return null;
            }
            Debug.Log("Setting active scene..");
            SceneManager.SetActiveScene(scene);

            // change skybox now that battle scene is active
            if (loadingBattleScene)
            {
                RenderSettings.skybox = currentSkyboxMat;
            }
        }

        public void EnterSceneSingle(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void EnterMainMenu()
        {
            ToggleUI(fpsCounter.activeSelf, false, false, false);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        public CanvasGroup GetFadeBlack()
        {
            return canvasGroup;
        }

        public void ToggleUI(bool _fps, bool _date, bool _daysLeft, bool _busts)
        {
            fpsCounter.SetActive(_fps);
            dateDisplay.SetActive(_date);
            daysLeftDisplay.SetActive(_daysLeft);
            statusBusts.SetActive(_busts);
        }

        public void RefreshParty()
        { // called when leaving or entering a dungeon
            Debug.Log("RefreshParty() called.");

            // party HP = full
            // party MP = full

            foreach (Hero hero in playerParty)
            {
                if (hero != null)
                {
                    hero.currentHP = (int)hero.HealthPoints.Value;
                    hero.currentMP = (int)hero.ManaPoints.Value;
                    
                    hero.isBleeding = false;
                    hero.isBurning = false;
                    hero.isGravitated = false;
                    hero.isPoisoned = false;
                    hero.isFrostbitten = false;
                    hero.isShocked = false;
                    hero.isStunned = false;
                    hero.isDowned = false;
                }
            }
            foreach (Hero hero in playerReserves)
            {
                if (hero != null)
                {
                    hero.currentHP = (int)hero.HealthPoints.Value;
                    hero.currentMP = (int)hero.ManaPoints.Value;

                    hero.isBleeding = false;
                    hero.isBurning = false;
                    hero.isGravitated = false;
                    hero.isPoisoned = false;
                    hero.isFrostbitten = false;
                    hero.isShocked = false;
                    hero.isStunned = false;
                    hero.isDowned = false;
                }
            }

        }

        public void RefreshDungeon()
        { // called when leaving or entering a dungeon
            Debug.Log("RefreshDungeon() called.");

            // Reset enemy position and activity
            ReawakenEnemies();

            // Reset player position(?) - may be handled by ExprFunctions.cs already
        }

        public void ReawakenEnemies()
        {
            List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();

            foreach (EnemyBehaviour child in currentEnemies)
            {
                child.ReawakenEnemy();
            }
        }

        public void ProcEncounter()
        {
            if (!Consts.RNG_ENCNTR_RNDM)
            {
                Debug.Log("THIS SHOULDN'T HAVE BEEN CALLED BECAUSE RNG ENCOUNTERS ISN'T ENABLED");
                return;
            }

            int totalWeight = 0;

            for (int i = 0; i < possibleEncounters.Count; i += 1)
            {
                totalWeight += possibleEncounterWeights[i];
            }

            int randomNumber = Consts.GetRangeNum(totalWeight);

            for (int i = 1; i < possibleEncounters.Count; i += 1)
            {
                if (randomNumber <= possibleEncounterWeights[i])
                {
                    // load encounter
                    Debug.Log("LOADING ENCOUNTER " + i);
                    StartBattle(possibleEncounters[i]);
                    return;
                }
                else
                {
                    randomNumber -= possibleEncounterWeights[i];
                }
            }

            // if something messed up while choosing the encounter, default to encounter 0 in the list
            StartBattle(possibleEncounters[0]);
            Debug.Log("SOMETHING WENT WRONG, LOADED FIRST ENCOUNTER");
        }

        public void BadBehaviourFound()
        {
            // :)
            Debug.Log("BadBehaviourFound()");
        }
    }
}

