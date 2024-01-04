using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class BustDisplay : MonoBehaviour
{
    [Header("Party")]
    [SerializeField] Image[] playerProfilePic = new Image[4];
    [SerializeField] Image[] playerColour = new Image[4];
    [SerializeField] Slider[] playerHPSlider = new Slider[4];
    [SerializeField] Slider[] playerMPSlider = new Slider[4];

    public void Update()
    {
        InitialiseActors();
    }

    public void InitialiseActors()
    {
        // initialise party members
        for (int i = 0; i < GameManager.singleton.playerParty.Length; i+= 1)
        {
            if (GameManager.singleton.playerParty[i] != null)
            {
                playerProfilePic[i].enabled = true;
                playerProfilePic[i].sprite = GameManager.singleton.playerParty[i].battleSprite;
                playerColour[i].color = GameManager.singleton.playerParty[i].healthBarColour;

                playerHPSlider[i].maxValue = GameManager.singleton.playerParty[i].HealthPoints.Value;
                playerHPSlider[i].value = GameManager.singleton.playerParty[i].currentHP;

                playerMPSlider[i].maxValue = GameManager.singleton.playerParty[i].ManaPoints.Value;
                playerMPSlider[i].value = GameManager.singleton.playerParty[i].currentMP;

                // show boon, status ailments..
            }
            else
            {
                playerProfilePic[i].enabled = false;
                playerColour[i].color = Color.black;

                playerHPSlider[i].maxValue = 999;
                playerHPSlider[i].value = 0;

                playerMPSlider[i].maxValue = 999;
                playerMPSlider[i].value = 0;
            }
        }
    }
}
