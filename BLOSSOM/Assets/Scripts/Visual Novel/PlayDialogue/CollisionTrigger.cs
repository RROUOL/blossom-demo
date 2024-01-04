using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : DialogueTrigger
{

    private void OnTriggerEnter (Collider collision)
    {
        Debug.Log("COLLIDED WITH DIALOGUE TRIGGER");

        // if collision is Player: PlayNani();
        if (collision.gameObject.tag != "Player")
        {
            Debug.Log("Collider not player");
            return;
        }

        Debug.Log("PLAYER COLLIDED!");
        PlayNani();
    }
}
