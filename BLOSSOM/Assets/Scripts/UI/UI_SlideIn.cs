using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;

[RequireComponent(typeof(RectTransform))]
public class UI_SlideIn : MonoBehaviour
{
    public Vector2 offscreenOffset;

    Vector2 offscreenPos;
    
    Vector2 finalPos;

    public float moveSpeed = 10.0f;

    public float timeToStart = 0.0f;

    public AudioClip playOnFinish;

    RectTransform thisUI;

    bool isFinished = false;

    
/*
    // Start is called before the first frame update
    void Start()
    { // called every time the UI attached to this script is turned on
        thisUI = this.gameObject;
        finalPos = thisUI.transform.position;
    }
*/
    void Awake()
    {
        thisUI = GetComponent<RectTransform>();
        finalPos = thisUI.anchoredPosition;
    }

    void OnEnable()
    {
        //thisUI = GetComponent<RectTransform>();
        //finalPos = thisUI.anchoredPosition;

        isFinished = false;

        offscreenPos = thisUI.anchoredPosition+offscreenOffset;
        thisUI.anchoredPosition = offscreenPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(thisUI.name + " FINALPOS: " + finalPos);

        //Debug.Log(thisUI.name + " CURRENT POS: " + thisUI.anchoredPosition);

        if (isFinished) return;

        timeToStart -= Time.deltaTime;

        if (timeToStart <= 0f)
        {
            thisUI.anchoredPosition = Vector2.MoveTowards(thisUI.anchoredPosition, finalPos, moveSpeed);
        }

        if (thisUI.anchoredPosition == finalPos)
        {
            isFinished = true;

            if (playOnFinish != null)
            {
                GameManager.singleton.PlaySound(playOnFinish);
            }
        }
    }
}