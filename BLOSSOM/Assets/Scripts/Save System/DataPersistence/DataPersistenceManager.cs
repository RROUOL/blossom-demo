using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    public static DataPersistenceManager singleton { get; private set; }

    public static int numberOfSaves = 3;

    private GameData gameData;
    //public int activeFile = 0;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    private void Awake()
    {
    // ensures this remains a singleton
        if (singleton != null && singleton != this) 
        { 
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
            Destroy(this); 
        } 
        else 
        { 
            singleton = this; 
        }
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, ("temp" + fileName));
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void NewGame()
    {
        this.gameData = new GameData();

        Debug.Log("Save data created.");
    }

    public void LoadGame(int index)
    {

        Debug.Log("LoadGame(int index) called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        // load any saved data from a file using the data handler
        this.dataHandler.SetFileName((index + fileName));
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.LogWarning("No data found! Creating new save file..");
            NewGame();
        }

        // give loaded data to all other scripts that need it
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.LoadData(gameData);
        }

        Debug.Log("Save data " + index + " loaded. Loaded file: " + index + fileName);
    }

    public void SaveGame(int index)
    {
        Debug.Log("SaveGame(int index) called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            Debug.Log("dataPersistenceObject found");
            obj.SaveData(ref gameData);
        }

        //save this data to a file using the data handler
        this.dataHandler.SetFileName((index + fileName));
        dataHandler.Save(gameData);
        
        Debug.Log("Save data " + index + " saved. Saved file: " + index + fileName);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects 
            = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        Debug.Log("FindAllDataPersistenceObjects() called. Found " + dataPersistenceObjects.Count() + " objects.");
        return new List<IDataPersistence>(dataPersistenceObjects);
    }


    public GameData GetSaveFile(int index)
    {
        // change datahandler to read from chosen "Xsave.excal1" file
        this.dataHandler.SetFileName((index + fileName));

        // create blank gamedata to copy over
        GameData gD = new GameData();
        gD = this.dataHandler.Load();

        // return to temp file and return gD
        this.dataHandler.SetFileName(("temp" + fileName));

        return gD;
    }
}
