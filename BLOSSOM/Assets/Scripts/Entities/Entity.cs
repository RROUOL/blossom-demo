    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Kryz.CharacterStats;
    using ElementEnum;

    //[CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
    public abstract class Entity : ScriptableObject
    {
        [Header("Description")]
        public string entityDescription;
        public Sprite battleSprite;
        public Color healthBarColour = new Color (1, 1, 1, 1);
        public Transform currentLocation;

        [Header("Basic Stats")]
        public int Level = 5; // min = 1, max = 99
        public int PressTurns = 2; // default is 2, full party of 4 is 8. maximum amount of turns per side is 14.
        public int turnsToGain = 0; // gain 1 press turn every time an attack is drained in a given turn. set to 0 at end of every turn/battle
        public int BurstTurns = 1; // default is 1, full party of 4 is 4. maximum amount of turns per side is 7.
        public int burstsToGain = 0; // default is 0, each side gets +1 at the end of their turn by default. only bosses should have anything above 0.
        public bool canMove = true;
        public bool isGuarding = false;

        [Header("Gauge Stats")]
        public CharacterStat HealthPoints = new CharacterStat(99, 1, 999, 0);
        public CharacterStat ManaPoints = new CharacterStat(49, 1, 777, 0);
        public int currentHP = 49;
        public int currentMP = 24;

        [Header("Levelled Stats")]
        public CharacterStat Vigour = new CharacterStat(7, 1, 99, 0); // health
        public CharacterStat Mind = new CharacterStat(7, 1, 99, 0); // mind
        public CharacterStat Pugilism = new CharacterStat(7, 1, 99, 0); // phys attack
        public CharacterStat Occultism = new CharacterStat(7, 1, 99, 0); // magi attack
        public CharacterStat Constitution = new CharacterStat(7, 1, 99, 0); // phys defence
        public CharacterStat Negation = new CharacterStat(7, 1, 99, 0); // magi defence
        public CharacterStat Fate = new CharacterStat(7, 7, 7, 7); // fate

        [Space]

        // buff currently applied to each stat
        // ranges between -2 to +2
        // defaults to 0 at the start and end of every battle
        // when initialising during battle, call InitBuffs(); 
        // when updating during battle, call RecalcBuffs()
        public int currentPugilism;
        public int currentOccultism;
        public int currentConstitution;
        public int currentNegation;

        [Header("Elemental Affinities")]
        // physical
        public ElementStatus Slash;
        public ElementStatus Strike;
        public ElementStatus Pierce;
        [Space]
        // magical
        public ElementStatus Air;
        public ElementStatus Fire;
        public ElementStatus Earth;
        public ElementStatus Life;
        public ElementStatus Ice;
        public ElementStatus Lightning;
        public ElementStatus Noise;

        [Header("Ailment Affinities")]
        public List<Ailment> AilmentList = new List<Ailment>(0); // list of current ailments
        public AilmentStatus Bleeding; // lose 1/10th HP at end of turn, physical attack halved
        public bool isBleeding;
        public AilmentStatus Burning; // lose 1/10th HP at end of turn, magical attack halved
        public bool isBurning;
        public AilmentStatus Gravitas; // moves consume 1 extra press turn
        public bool isGravitated;
        public AilmentStatus Poison; //  lose 1/10th HP at end of turn 
        public bool isPoisoned;
        public AilmentStatus Frostbite; // lose 1/10th HP at end of turn, physical defence halved
        public bool isFrostbitten;
        public AilmentStatus Shock; // lose 1/10th HP at end of turn, magical defence halved
        public bool isShocked;
        public AilmentStatus Stunned; // cannot move, consume 1 press turn
        public bool isStunned;
        public AilmentStatus Down; // cannot move, skipped over
        public bool isDowned;

        [Header("Moves")]
        public Move[] basicAttacks = new Move[2];
        public Move[] moveList = new Move[8];

        public void RecalcBuffs()
        {
            Pugilism.RemoveAllModifiersFromSource(this);
            Occultism.RemoveAllModifiersFromSource(this);
            Constitution.RemoveAllModifiersFromSource(this);
            Negation.RemoveAllModifiersFromSource(this);

            StatModifier pug = new StatModifier((0.25f * currentPugilism), StatModType.PercentMult, 9, this);
            Pugilism.AddModifier(pug);
            StatModifier occ = new StatModifier((0.25f * currentOccultism), StatModType.PercentMult, 9, this);
            Occultism.AddModifier(occ);
            StatModifier con = new StatModifier((0.25f * currentConstitution), StatModType.PercentMult, 9, this);
            Constitution.AddModifier(con);
            StatModifier neg = new StatModifier((0.25f * currentNegation), StatModType.PercentMult, 9, this);
            Negation.AddModifier(neg);
        }

        public void InitBuffs()
        { // called at very beginning of battle (or when entity first appears) 
            currentPugilism = 0;
            currentOccultism = 0;
            currentConstitution = 0;
            currentNegation = 0;

            RecalcBuffs();
        }


        public void AddBuffs(int _pug, int _occ, int _con, int _neg)
        {
            currentPugilism += _pug;
            if (currentPugilism < -2) currentPugilism = -2;
            if (currentPugilism > 2) currentPugilism = 2;

            currentOccultism += _occ;
            if (currentOccultism < -2) currentOccultism = -2;
            if (currentOccultism > 2) currentOccultism = 2;

            currentConstitution += _con;
            if (currentConstitution < -2) currentConstitution = -2;
            if (currentConstitution > 2) currentConstitution = 2;

            currentNegation += _neg;
            if (currentNegation < -2) currentNegation = -2;
            if (currentNegation > 2) currentNegation = 2;

            RecalcBuffs();
        }

    }
