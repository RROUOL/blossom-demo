using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class InteractTrigger : DialogueTrigger
{
    private bool withinRange = false;

    public Vector3 directionToFace;

    private void OnTriggerEnter(Collider collision)
    {
        // if collision is Player: withinRange = true;
        if (collision.gameObject.tag != "Player")
        {
            //Debug.Log("Collider not player");
            return;
        }

        withinRange = true;
        //Debug.Log("PLAYER TRIGGERED!");
    }

    private void OnTriggerExit(Collider collision)
    {
        // if collision is Player: withinRange = false;
        if (collision.gameObject.tag != "Player")
        {
            //Debug.Log("Collider not player");
            return;
        }

        withinRange = false;
        //Debug.Log("PLAYER EXITED!");
    }

    private void LateUpdate()
    {
        if (withinRange && !GameManager.singleton.isInputBlocked)
        {
            GameObject playerObj = GameObject.Find("Player");

            if (playerObj.transform.eulerAngles != directionToFace)
            {
                //Debug.Log("not facing correct direction xx - " + playerObj.transform.eulerAngles + " - " + directionToFace);
                GameManager.singleton.showInteractPrompt = false;
                return;
            }
            //Debug.Log("facing correct direction - " + playerObj.transform.eulerAngles + " - " + directionToFace);

            GameManager.singleton.interactTimeLeft = 0.05f;
            GameManager.singleton.showInteractPrompt = true;
        }
        else
        {
            GameManager.singleton.showInteractPrompt = false;
        }

        if (cInput.GetKey("Interact") && GameManager.singleton.showInteractPrompt)
        {
            PlayNani();
            withinRange = false;
        }
    }
}
