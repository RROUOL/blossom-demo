using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.ManagerSingletons;
using Vaz.Constants.Consts;


public class DungeonMovement : MonoBehaviour
{
    public bool canMove = true;

    LayerMask mask;

    private Vector3 targetGridPos;
    private Vector3 prevTargetGridPos;
    private Vector3 targetRotation;

    [Header("Movement Settings")]
    [SerializeField] private bool isMoving;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 250f;

    [Header("Buffer Settings")]
    [SerializeField] private float bufferTime = 0.167f;
    private float turnLeftBufferTime = 1.0f;
    private float turnRightBufferTime = 1.0f;
    private float moveForwardBufferTime = 1.0f;
    private float moveBackBufferTime = 1.0f;
    private float moveLeftBufferTime = 1.0f;
    private float moveRightBufferTime = 1.0f;

    [Header("Random Encounter Settings")]
    [Tooltip("Determines the minimum amount of steps that must be taken before another encounter has a chance of happening.")]
    public int encounterStepsRequired = 45;
    private bool hasMoved = false;


    void Awake()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
        targetRotation = transform.eulerAngles;

        mask = LayerMask.GetMask("Ignore Raycast");
        mask = ~mask;

        Debug.Log("Layer Mask created: " + mask.value);

        GameManager.singleton.SearchForPlayer();
    }

    // void FixedUpdate()
    void Update()
    {
        turnLeftBufferTime += Time.fixedDeltaTime;
        turnRightBufferTime += Time.fixedDeltaTime;
        moveForwardBufferTime += Time.fixedDeltaTime;
        moveBackBufferTime += Time.fixedDeltaTime;
        moveLeftBufferTime += Time.fixedDeltaTime;
        moveRightBufferTime += Time.fixedDeltaTime;

        // update movement if isMoving

        prevTargetGridPos = targetGridPos;

        //Vector3 targetPosition = targetGridPos;

        if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if (targetRotation.y < 0f) targetRotation.y = 270f;

        transform.position = Vector3.MoveTowards(transform.position, /*targetPosition*/ targetGridPos, Time.deltaTime * movementSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotationSpeed);

        // if movement is finished, character is no longer moving

        if ((Vector3.Distance(transform.position, targetGridPos) < 0.1f) &&
            (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.1f))
        {
            isMoving = false;
        }

        if (!canMove) 
        {
            //Debug.Log("DungeonMovement halted by canMove");
            return;
        }
        if (GameManager.singleton.isDungeonPaused)
        {
            //Debug.Log("DungeonMovement halted by canDungeonPause");
            return;
        }
        if (GameManager.singleton.isInputBlocked)
        {
            //Debug.Log("DungeonMovement halted by isInputBlocked");
            return;
        }

        //Debug.Log("moving");

        // movement buffers

        if (cInput.GetKey("Up"))
        {
            moveForwardBufferTime = 0.0f;
        }
        if (cInput.GetKey("Down"))
        {
            moveBackBufferTime = 0.0f;
        }
        if (cInput.GetKey("Left"))
        {
            moveLeftBufferTime = 0.0f;
        }
        if (cInput.GetKey("Right"))
        {
            moveRightBufferTime = 0.0f;
        }
        
        if (cInput.GetKey("Turn Left"))
        {
            turnLeftBufferTime = 0.0f;
        }
        if (cInput.GetKey("Turn Right"))
        {
            turnRightBufferTime = 0.0f;
        }

        // if already moving, cant move until finished
        if (isMoving) return;
        isMoving = true;
        // else, check buffers and move accordingly

        // raycast check
        char direction = '0';

        if ((moveForwardBufferTime < bufferTime) && (moveLeftBufferTime < bufferTime)) { moveForwardBufferTime = bufferTime; moveLeftBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos += (transform.forward * 2 - transform.right * 2); direction = 'Q'; } }
        if ((moveForwardBufferTime < bufferTime) && (moveRightBufferTime < bufferTime)) { moveForwardBufferTime = bufferTime; moveRightBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos += (transform.forward * 2 + transform.right * 2); direction = 'E'; } }

        if ((moveBackBufferTime < bufferTime) && (moveLeftBufferTime < bufferTime)) { moveBackBufferTime = bufferTime; moveLeftBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos -= (transform.forward * 2 + transform.right * 2); direction = 'Z'; } }
        if ((moveBackBufferTime < bufferTime) && (moveRightBufferTime < bufferTime)) { moveBackBufferTime = bufferTime; moveRightBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos -= (transform.forward * 2 - transform.right * 2); direction = 'C'; } }

        if (moveForwardBufferTime < bufferTime) { moveForwardBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos += (transform.forward * 2); direction = 'W'; } }
        if (moveBackBufferTime < bufferTime) { moveBackBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos -= (transform.forward * 2); direction = 'S'; } }
        if (moveLeftBufferTime < bufferTime) { moveLeftBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos -= (transform.right * 2); direction = 'A'; } }
        if (moveRightBufferTime < bufferTime) { moveRightBufferTime = bufferTime; 
            if (!CheckForRandomEncounter()) { targetGridPos += (transform.right * 2); direction = 'D'; } }

        if (turnLeftBufferTime < bufferTime) { targetRotation -= Vector3.up * 90f; }
        if (turnRightBufferTime < bufferTime) { targetRotation += Vector3.up * 90f; }

        // if X or Z position of targetGridPos isn't divisible by 2, targetGridPos = prevTargetGridPos;

        if (Mathf.RoundToInt(targetGridPos.x)%2 != 0 || Mathf.RoundToInt(targetGridPos.z)%2 != 0)
        {
            //Debug.Log("can't move here: " + targetGridPos);
            targetGridPos = prevTargetGridPos;
            return;
        }

        // if moving in a cardinal..
        switch (direction)
        {
            case 'Q': // check raycast top-left
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'E': // check raycast top-right
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'Z': // check raycast bottom-left
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back + Vector3.left), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'C': // check raycast bottom-right
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back + Vector3.right), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;
            
            case 'W': // check raycast ahead
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'S': // check raycast behind
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'A': // check raycast to the left
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            case 'D': // check raycast to the right
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), 2, mask, QueryTriggerInteraction.Ignore)) 
                {
                    targetGridPos = prevTargetGridPos;
                }
                else
                {
                    hasMoved = true;
                }
                break;

            default:

                break;
        }

    }

    public void SetPos(Vector3 aTargetPos, Quaternion aTargetRot)
    {
        // change position
        transform.position = aTargetPos;
        transform.rotation = aTargetRot;

        Vector3 Rot = aTargetRot * transform.eulerAngles;

        targetGridPos = aTargetPos;
        targetRotation = Rot;
    }

    public void EmptyBuffer()
    {
        turnLeftBufferTime = 1.0f;
        turnRightBufferTime = 1.0f;
        moveForwardBufferTime = 1.0f;
        moveBackBufferTime = 1.0f;
        moveLeftBufferTime = 1.0f;
        moveRightBufferTime = 1.0f;
    }

    private bool CheckForRandomEncounter()
    {
        if (Consts.RNG_ENCNTR_RNDM)
        {
            if (!hasMoved)
            {
                return false;
            }

            hasMoved = false;
            encounterStepsRequired -= 1;

            if (encounterStepsRequired <= 0)
            {
                if (Consts.EncounterCheck(7))
                {
                    Debug.Log("ENCOUNTER PROC'D");

                    encounterStepsRequired = 75;

                    // load encounter

                    GameManager.singleton.ProcEncounter();

                    return true;
                }
            }
        }

        return false;
    }

    public void RefreshEncounterPool()
    {
        if (Consts.RNG_ENCNTR_RNDM)
        {
            // TODO - change encounter pool for different dungeons
        }
    }
}