using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Index", menuName = "Data Index")]
public class DataIndex : ScriptableObject // stores information of all 
{
    public Hero[] heroIndex;

    public Foe[] foeIndex;

    public Move[] moveIndex;
}
