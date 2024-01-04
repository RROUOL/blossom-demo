using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

public class DistortCamera : MonoBehaviour
{
    [SerializeField] Camera thisCam;
    [SerializeField] GameObject canvasObj;
    [SerializeField] CanvasGroup canvasGroup;

    public bool battleStarting = false;
    private float fadeSpeed = 1f;

    void Awake()
    {
        canvasObj = GameObject.Find("/DND_Canvas_1/Fade To Black");

        if (canvasObj == null)
        {
            Debug.LogError("LETHAL ERROR: Canvas Group not found!");
            return;
        }
        else
        {
            canvasGroup = canvasObj.GetComponent(typeof(CanvasGroup)) as CanvasGroup;
        }

    }

    void FixedUpdate()
    {
        if (!battleStarting)
        {
            // give original FOV (60) and original rotation (save before editing)
            //Camera.main.fieldOfView = 60;
            thisCam.fieldOfView = 60;
            thisCam.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            return;
        }
        else
        {
            //Camera.main.fieldOfView -= fadeSpeed;
            thisCam.fieldOfView -= fadeSpeed;
            thisCam.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + fadeSpeed * 2);
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                if (canvasGroup.alpha == 1)
                {
                    GameManager.singleton.EnterBattle();
                }
        }
    }
}
