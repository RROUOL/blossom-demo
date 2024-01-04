    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    namespace ElementEnum
    {
        public enum ElementStatus
        {
            SuperEffective, // 2x Damage + gain 1 Press Turn
            NormalEffective, // 1x Damage
            WeakEffective, // 0.5x Damage
            Immune, // 0x Damage + lose 1 Press Turn
            Deflect, // 0x Damage + 1x Damage deflected onto caster
            Drain // -1x Damage + target gains 1 Press Turn on next turn
        }

        public enum AilmentStatus
        {
            Vulnerable, // Affected by status normally
            Immune, // Status doesn't land 
            Heal, // (only use for damaging ailments, otherwise does nothing) -1x damage on ailment
            Special, // different for each ailment - e.g. Burning.Special gives 2x HP loss but can't go below 1HP + 1.5x damage boost
            Permanent // can never be removed
        }

        public enum ElementType
        {
            Fae,

            Slash, // physical
            Strike,
            Pierce,

            Air, // magical
            Fire,
            Earth,
            Life,
            Ice,
            Lightning,
            Noise
        }

        public enum AilmentType
        {
            Bleeding, // 0 - lose 1/10th HP at end of turn, physical attack halved
            Burning, // 1 - lose 1/10th HP at end of turn, magical attack halved
            Gravitas, // 2 - moves consume 1 extra press turn
            Poison, // 3 - lose 1/10th HP at end of turn, -1 press turns 
            Frostbite, // 4 - lose 1/10th HP at end of turn, physical defence halved
            Shock, // 5 - lose 1/10th HP at end of turn, magical defence halved
            Stunned, // 6 - cannot move, consume 1 press turn - treated as if the user automatically passed a turn
            Down // 7 - inflicted at 0HP automatically, can't move until healed
        }


        public enum StatType
        {
            Vigour, // health
            Mind, // mana
            Pugilism, // phys attack
            Occultism, // magi attack
            Constitution, // phys defence
            Negation, // magi defence
            Fate // fate
        }
    }