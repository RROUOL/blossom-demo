using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

// This is primarily used to allow encounters and
// items to be gained during visual novel sections.
// ExprFunctions.cs functions start encounters and item
// acquisition through reading this.

[CreateAssetMenu(fileName = "New Databank", menuName = "Databank")]
public class Databank : ScriptableObject
{
    // all available heroes
    public List<Hero> heroesBank = new List<Hero>();

    // all available foes
    //public List<Foe> foesBank = new List<Foe>();

    // all available encounters
    public List<Encounter> encounterBank = new List<Encounter>();

    // all available moves
    public List<Move> movesBank = new List<Move>();

    // all available dungeons
    public List<GameObject> dungeonsBank = new List<GameObject>();
    
    // all available key items
    public List<KeyItem> keysBank = new List<KeyItem>();

    // all available consumable items
    public List<Consumable> consumaBank = new List<Consumable>();

    // all available equippable items
    public List<Boon> equipBank = new List<Boon>();

    // all available social links
    //public List<Hero> socialBank;
}