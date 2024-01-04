using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class TransportPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private DungeonMovement dungeonMovement;
    [SerializeField] private GameObject canvasObj;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject transportLocation;

    [Tooltip("Objects that are activated when entering this teleporter.")]
    public List<GameObject> onEnterEnable = new List<GameObject>();

    private float fadeSpeed = 5f;
    char currentlyFading;

    private Vector3 transPosition;
    [SerializeField] private Quaternion transRotation;

    void Awake()
    {
        player = GameObject.Find("/Player");
        dungeonMovement = player.GetComponent(typeof(DungeonMovement)) as DungeonMovement;

        canvasObj = GameObject.Find("/DND_Canvas_1/Fade To Black");
        canvasGroup = canvasObj.GetComponent(typeof(CanvasGroup)) as CanvasGroup;

        //Debug.Log("Was transporting to... " + transPosition + ", " + newRotation);
        transPosition = transportLocation.transform.position;
        //transRotation = transportLocation.transform.rotation;
        //Debug.Log("Will now transport to... " + transPosition + ", " + newRotation);
    }

    /*
    void Update()
    {
        switch (currentlyFading)
        {
            case 'O':
                //Debug.Log("Fading Out");
                //Debug.Log(Time.fixedDeltaTime * fadeSpeed);
                canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                if (canvasGroup.alpha == 1)
                {
                    GameManager.singleton.newPosition.Set(transPosition.x, transPosition.y, transPosition.z);
                    //GameManager.singleton.newRotation.Set(transRotation.x, transRotation.y, transRotation.z);
                    GameManager.singleton.newRotation.Set(Quaternion.Euler(transRotation));
                    //Teleport();
                    currentlyFading = 'I';
                }
                //Debug.Log("out: " + canvasGroup.alpha);
                break;
            case 'I':
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
    }
    */

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject != player)
        {
            return;
        }

        Debug.Log(transRotation);

        GameManager.singleton.newPosition.Set(transPosition.x, transPosition.y, transPosition.z);
        GameManager.singleton.newRotation.Set(transRotation.x, transRotation.y, transRotation.z, transRotation.w);
        //GameManager.singleton.newRotation.Set(Quaternion.Euler(transRotation));
        GameManager.singleton.StartTeleport(onEnterEnable);

        //GameManager.singleton.currentlyFading = 'O';
        //GameManager.singleton.isInputBlocked = true;
    }

    // this function is currently deprecated, please refer to GameManager's Teleport function.
    void Teleport()
    {
        //Now that the black screen has finished, begin changing position.
        //Debug.Log("Teleporting: " + player.name + "...");

        player.transform.position = transPosition;
        player.transform.rotation = transRotation;

        
        dungeonMovement.SetPos(transPosition, transRotation);

        //Debug.Log("Teleported to position " + player.transform.position + " and rotation " + player.transform.rotation);

        GameManager.singleton.isInputBlocked = false;

        dungeonMovement.EmptyBuffer();
    }
}
