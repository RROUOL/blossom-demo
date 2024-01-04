using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.ManagerSingletons;

public class PartyMenu : MonoBehaviour
{
    [Header("Party")]
    [SerializeField] Image[] playerProfilePic = new Image[4];
    [SerializeField] Text[] playerCharacterName = new Text[4];
    [SerializeField] Image[] playerColour = new Image[4];
    [SerializeField] Slider[] playerHPSlider = new Slider[4];
    [SerializeField] Slider[] playerMPSlider = new Slider[4];
    [SerializeField] Image[] playerBoon = new Image[4];
    // status ailment display
    [Space]
    [SerializeField] Text[] playerVig = new Text[4];
    [SerializeField] Text[] playerMnd = new Text[4];
    [SerializeField] Text[] playerPug = new Text[4];
    [SerializeField] Text[] playerOcc = new Text[4];
    [SerializeField] Text[] playerCon = new Text[4];
    [SerializeField] Text[] playerNeg = new Text[4];

    [Header("Reserves")]
    [SerializeField] Image[] reserveProfilePic = new Image[7];
    [SerializeField] Text[] reserveCharacterName = new Text[7];
    [SerializeField] Image[] reserveColour = new Image[7];
    [SerializeField] Slider[] reserveHPSlider = new Slider[7];
    [SerializeField] Slider[] reserveMPSlider = new Slider[7];


    void Awake()
    {

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
                playerCharacterName[i].text = GameManager.singleton.playerParty[i].name;
                playerColour[i].color = GameManager.singleton.playerParty[i].healthBarColour;

                playerHPSlider[i].maxValue = GameManager.singleton.playerParty[i].HealthPoints.Value;
                playerHPSlider[i].value = GameManager.singleton.playerParty[i].currentHP;

                playerMPSlider[i].maxValue = GameManager.singleton.playerParty[i].ManaPoints.Value;
                playerMPSlider[i].value = GameManager.singleton.playerParty[i].currentMP;

                playerVig[i].text = GameManager.singleton.playerParty[i].Vigour.Value.ToString();
                playerMnd[i].text = GameManager.singleton.playerParty[i].Mind.Value.ToString();
                playerPug[i].text = GameManager.singleton.playerParty[i].Pugilism.Value.ToString();
                playerOcc[i].text = GameManager.singleton.playerParty[i].Occultism.Value.ToString();
                playerCon[i].text = GameManager.singleton.playerParty[i].Constitution.Value.ToString();
                playerNeg[i].text = GameManager.singleton.playerParty[i].Negation.Value.ToString();

                playerBoon[i].enabled = true;
                if (GameManager.singleton.playerParty[i].equippedItems[0] != null)
                {
                    playerBoon[i].sprite = GameManager.singleton.playerParty[i].equippedItems[0].itemSprite;
                }
                else
                {
                    playerBoon[i].enabled = false;
                }
                // show boon, status ailments..
            }
            else
            {
                playerProfilePic[i].enabled = false;
                playerCharacterName[i].text = "EMPTY";
                playerColour[i].color = Color.black;

                playerHPSlider[i].maxValue = 999;
                playerHPSlider[i].value = 0;

                playerMPSlider[i].maxValue = 999;
                playerMPSlider[i].value = 0;

                playerVig[i].text = "--";
                playerMnd[i].text = "--";
                playerPug[i].text = "--";
                playerOcc[i].text = "--";
                playerCon[i].text = "--";
                playerNeg[i].text = "--";

                playerBoon[i].enabled = false;
            }
        }
        /*
        // initialise reserves

        for (int i = 0; i < GameManager.singleton.playerReserves.Length; i+= 1)
        {
            if (GameManager.singleton.playerReserves[i] != null)
            {
                //GameManager.singleton.playerReserves[i].currentLocation = reserveLocations[i];
            }
        }
        */
    }
}
