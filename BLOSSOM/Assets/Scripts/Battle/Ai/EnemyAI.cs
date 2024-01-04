using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.Constants.Consts;

/*
public class EnemyAI : MonoBehaviour
{
    public int total;
    public int randomNum;
    // 2 basic attacks, 8 moves
    public int[] moveTable = { 10, 10, 
                                17, 15, 13, 11, 9, 7, 5, 3 };

    // Start is called before the first frame update
    void Start()
    {
        if (Consts.RNG_ENEM_TABL)
        {// if random move tables are used..
            foreach(int move in moveTable)
            {
                total += move;
            }

            randomNum = Random.Range(0, total);
        }
    }

    // Update is called once per frame
    void UpdateWeights()
    {
        // for context-dependent events that may change the likelihood to use certain moves
    }

    public int GetChosenMove()
    {
        if (Consts.RNG_ENEM_TABL)
        { // if weighted random move is used..
            for (int i = 0; i < moveTable.Length; i += 1)
            {
                if (randomNum < moveTable[i])
                {
                    // return i as the index of the move that'll be used
                    Debug.Log("Randomly returning move index " + i);
                    return i;
                }
                else
                {
                    randomNum -= moveTable[i];
                }
            }
        }
        else
        { // else, choose the largest weight
            int largestIndex;
            int largestIndexVal = -1;

            for (int i = 0; i < moveTable.Length; i += 1)
            {
                if (largestIndexVal < moveTable[i])
                {
                    Debug.Log("New largest index " + i);
                    largestIndex = i;
                }
            }

            Debug.Log("Returning largest weight index " + i);
        }
    }
}
*/