using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;
using Vaz.Constants.Consts;

public class EnemyBehaviour : MonoBehaviour
{
    bool playerSpotted = false;

    [Tooltip("Sound made when spotting the player")]
    [SerializeField] AudioClip triggeredGurgle;

    // no RNG
    [SerializeField] Encounter thisEncounter;

    [SerializeField] GameObject[] roamPoints;

    int currentDestination = 0;

    [SerializeField] int moveSpeed = 2;
    [SerializeField] int rotateSpeed = 1;

    void Awake()
    {
        currentDestination = 0;
        if (Consts.RNG_ENCNTR_RNDM)
        {
            Debug.Log("Random Encounters on. This enemy is deactivated.");
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Random Encounters off. This enemy is activated.");
            this.gameObject.SetActive(true);
        }
    }

    public void ReawakenEnemy()
    { // called when restarting day
        Debug.Log(this.name + " was reactivated.");

        this.gameObject.SetActive(true);
        this.gameObject.transform.position = roamPoints[0].transform.position;
        currentDestination = 0;

        if (Consts.RNG_ENCNTR_RNDM)
        {
            Debug.Log("Random Encounters on. This enemy is deactivated.");
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Random Encounters off. This enemy is activated.");
            this.gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(this.gameObject.transform.position, roamPoints[currentDestination].transform.position) < 0.1f)
        {
            NewWaypoint();
        }
        else
        {   
            MoveEnemy();
        }

    }

    void MoveEnemy()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, roamPoints[currentDestination].transform.position, moveSpeed * Time.deltaTime);

        this.gameObject.transform.LookAt(roamPoints[currentDestination].transform.position);

        //Vector3 targetDirection = (roamPoints[currentDestination].transform.position - this.gameObject.transform.position).normalized;
        
        //Vector3 targetDirection = (roamPoints[currentDestination].transform.position - this.gameObject.transform.position);
        //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation , Time.deltaTime);
 
    }

    void NewWaypoint()
    { // called when reaching the current roaming point

        //Debug.Log("NewWaypoint() called");

        currentDestination += 1;

        if (currentDestination > roamPoints.Length-1)
        {
            //Debug.Log("NewWaypoint() RESET TO 0");
            currentDestination = 0;
        }

        //Debug.Log("New Destination: Waypoint " + currentDestination);
    }

    void OnCollisionEnter(Collision collision)
    {
        // start battle scene if detecting Player tag
        if (collision.gameObject.tag != "Player")
        {
            //Debug.Log("Collider not player");
            return;
        }

        //Debug.Log("PLAYER COLLIDED!");
        GameManager.singleton.StartBattle(thisEncounter);
        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    { // detection box (area around enemy which can be walked through, circle is slightly front-of-center)
        // alert enemy if detecting Player tag
        if (other.gameObject.tag != "Player")
        {
            //Debug.Log("Trigger not player");
            return;
        }

        //Debug.Log("PLAYER TRIGGERED!");
        GameManager.singleton.DIAplayer.PlayOneShot(triggeredGurgle, 1.0f);

        GameManager.singleton.StartBattle(thisEncounter);
        this.gameObject.SetActive(false);
    }
}
