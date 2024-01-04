using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public bool isBoss = false;

    public string encounterText = " attacks!";

    public Foe[] encounter = new Foe[7];

    public Material planeMat;

    public Material skyboxMat;

    public AudioClip music;

    public AudioClip enemySound;

    [Header("Victory Script")]
    public bool playVictoryScript = false;
    public string script;
    public string label;
}
