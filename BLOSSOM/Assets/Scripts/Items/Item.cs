using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElementEnum;
using UnityEngine.UI; 

namespace Kryz.CharacterStats
{
    public class Item : ScriptableObject
    {
        public Sprite itemSprite;

        public string itemDescription;

        public bool usableInBattle = true;
    }

    public class KeyItem : Item
    { // key item

    }

    public class Consumable : Item
    { // consumable item
        [Tooltip("How many times the consumable can be used per day.")]
        public int Charges = 1;
    }

    public class Boon : Item
    { // equippable item

        // vig, mnd, pug, occ, con, neg

        public int[] FlatBonuses = new int[6];

        public float[] PercentBonuses = new float[6];

        public ElementStatus newSlash;
        public ElementStatus newStrike;
        public ElementStatus newPierce;
        
        public ElementStatus newAir;
        public ElementStatus newFire;
        public ElementStatus newEarth;
        public ElementStatus newLife;
        public ElementStatus newIce;
        public ElementStatus newLightning;
        public ElementStatus newNoise;
        

        public void Equip(Hero c)
        {
            // int bonus

            if (FlatBonuses[0] != 0)
                c.Vigour.AddModifier(new StatModifier(FlatBonuses[0], StatModType.Flat, this));

            if (FlatBonuses[1] != 0)
                c.Mind.AddModifier(new StatModifier(FlatBonuses[1], StatModType.Flat, this));

            if (FlatBonuses[2] != 0)
                c.Pugilism.AddModifier(new StatModifier(FlatBonuses[2], StatModType.Flat, this));

            if (FlatBonuses[3] != 0)
                c.Occultism.AddModifier(new StatModifier(FlatBonuses[3], StatModType.Flat, this));

            if (FlatBonuses[4] != 0)
                c.Constitution.AddModifier(new StatModifier(FlatBonuses[4], StatModType.Flat, this));

            if (FlatBonuses[5] != 0)
                c.Negation.AddModifier(new StatModifier(FlatBonuses[5], StatModType.Flat, this));

            // float bonus

            if (PercentBonuses[0] != 0)
                c.Vigour.AddModifier(new StatModifier(PercentBonuses[0], StatModType.PercentMult, this));

            if (PercentBonuses[1] != 0)
                c.Mind.AddModifier(new StatModifier(PercentBonuses[1], StatModType.PercentMult, this));

            if (PercentBonuses[2] != 0)
                c.Pugilism.AddModifier(new StatModifier(PercentBonuses[2], StatModType.PercentMult, this));

            if (PercentBonuses[3] != 0)
                c.Occultism.AddModifier(new StatModifier(PercentBonuses[3], StatModType.PercentMult, this));

            if (PercentBonuses[4] != 0)
                c.Constitution.AddModifier(new StatModifier(PercentBonuses[4], StatModType.PercentMult, this));

            if (PercentBonuses[5] != 0)
                c.Negation.AddModifier(new StatModifier(PercentBonuses[5], StatModType.PercentMult, this));

            c.Slash = newSlash;
            c.Strike = newStrike;
            c.Pierce = newPierce;
            
            c.Air = newAir;
            c.Fire = newFire;
            c.Earth = newEarth;
            c.Life = newLife;
            c.Ice = newIce;
            c.Lightning = newLightning;
            c.Noise = newNoise;
        }

        public void Unequip(Hero c)
        {
            c.HealthPoints.RemoveAllModifiersFromSource(this);
            c.ManaPoints.RemoveAllModifiersFromSource(this);

            c.Vigour.RemoveAllModifiersFromSource(this);
            c.Mind.RemoveAllModifiersFromSource(this);
            c.Pugilism.RemoveAllModifiersFromSource(this);
            c.Occultism.RemoveAllModifiersFromSource(this);
            c.Constitution.RemoveAllModifiersFromSource(this);
            c.Negation.RemoveAllModifiersFromSource(this);

            c.Slash = c.baseSlash;
            c.Strike = c.baseStrike;
            c.Pierce = c.basePierce;
            
            c.Air = c.baseAir;
            c.Fire = c.baseFire;
            c.Earth = c.baseEarth;
            c.Life = c.baseLife;
            c.Ice = c.baseIce;
            c.Lightning = c.baseLightning;
            c.Noise = c.baseNoise;
        }
    }
}