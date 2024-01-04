using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressTurnDisplay : MonoBehaviour
{
    public int currentTurns;
    public int numOfTurns;

    public Image[] icons = new Image[7];

    [SerializeField] Sprite fullTurn;
    [SerializeField] Sprite semiTurn;
    [SerializeField] Sprite emptyTurn;

    void LateUpdate()
    {
        if (currentTurns > numOfTurns)
        {
            currentTurns = numOfTurns;
        }

        bool isOdd = (currentTurns % 2 == 1);

        int tempCount = currentTurns;

        for (int i = 0; i < icons.Length; i += 1)
        {
            if (tempCount > 0)
            {
                if (tempCount == 1)
                {
                    icons[i].sprite = semiTurn;
                }
                else
                {
                    icons[i].sprite = fullTurn;
                }

                tempCount -= 2;
            }
            else
            {
                icons[i].sprite = emptyTurn;
            }

            if (i < ((numOfTurns + 1) / 2))
            {
                icons[i].enabled = true;
            }
            else
            {
                icons[i].enabled = false;
            }
        }
        /*
        for (int i = 0; i < icons.Length; i += 1)
        {
            if ((i == (currentTurns+1) / 2) && (isOdd))
            {
                icons[(int)i/2].sprite = semiTurn;
            }
            else if (i < (currentTurns / 2))
            {
                icons[(int)i].sprite = fullTurn;
                /*
                if (i % 2 == 0)
                {
                    icons[(int)i].sprite = fullTurn;
                }
                else
                {
                    icons[(int)i].sprite = semiTurn;
                }
                
            }
            else
            {
                icons[(int)i].sprite = emptyTurn;
            }

            if (i < (numOfTurns / 2))
            {
                icons[(int)i].enabled = true;
            }
            else
            {
                icons[(int)i].enabled = false;
            }
        }
        /*

        /*
        for (int i = 0; i < icons.Length; i += 1)
        {
            if ((i == currentTurns) && (isOdd))
            {
                icons[(int)i/2].sprite = semiTurn;
            }
            else if (i < (currentTurns / 2))
            {
                icons[(int)i].sprite = fullTurn;
                //UNUSED START
                if (i % 2 == 0)
                {
                    icons[(int)i].sprite = fullTurn;
                }
                else
                {
                    icons[(int)i].sprite = semiTurn;
                }
                //UNUSED END
            }
            else
            {
                icons[(int)i].sprite = emptyTurn;
            }

            if (i < (numOfTurns / 2))
            {
                icons[(int)i].enabled = true;
            }
            else
            {
                icons[(int)i].enabled = false;
            }
        }
        */
    }
}
