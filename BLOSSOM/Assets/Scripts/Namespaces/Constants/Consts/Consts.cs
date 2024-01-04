using UnityEngine;

namespace Vaz.Constants.Consts
{
    public static class Consts
    {
        // RNG, SET TRUE TO ACTIVATE RNG FOR THE FOLLOWING MECHANICS:

        public const bool RNG_MISS_CRIT = true;       // instead of never missing or critting, attacks have a random chance to miss or crit
        public const bool RNG_DMG_VARI = true;        // instead of set damage, attacks will deal a random amount between 0.85x and 1.05x
        
        public const bool RNG_ENEM_TABL = true;       // instead of fixed behaviour, will use a weighted table for behaviour assigned to enemies
        public const bool RNG_ENCNTR_RNDM = true;     // instead of visible enemies, will proc an enemy encounter when moving in dungeons

        public const bool RNG_ESCAPE_PROC = true;     // instead of guaranteed escape for non-bosses, will have a chance to fail escape and begin enemy turn



        //public const bool RNG_PROC = false;            // instead of buildup, attacks with secondary effects will randomly give those effects
        //public const bool RNG_LOOT_TABL = false;       // instead of fixed drops, will use a weighted table for loot drops assigned to encounters
        
        /*
        static void Awake()
        {
            // ensures this remains a singleton
            if (singleton != null && singleton != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                singleton = this; 
            }
        }
        */

        public static int GetRangeNum(int rangeNum)
        {
            return Random.Range(0, rangeNum);
        }

        public static bool ProcChance(int percentChance)
        {
            //Debug.Log("RANDOM CHANCE CHECK");

            int randomInt = Random.Range(1, 101);

            if (randomInt <= percentChance) // X/100 chance
            {
                Debug.Log("RANDOM PROC'D: " + randomInt + " <= " + percentChance);
                return true;
            }
            else
            {
                Debug.Log("RANDOM FAILED: " + randomInt + " > " + percentChance);
                return false;
            }

        }

        public static float DamVariance(float currentDamage)
        {
            float randomFlo = Random.Range(0.85f, 1.05f);
            
            currentDamage *= randomFlo;

            return currentDamage;
        }

        public static bool EncounterCheck(int percentChance)
        {
            //Debug.Log("RANDOM ENCOUNTER CHECK");

            float randomInt = Random.Range(1, 101);

            if (randomInt <= percentChance) // X/100 chance
            {
                //Debug.Log("RANDOM PROC'D: " + randomInt + " <= " + percentChance);
                return true;
            }
            else
            {
                //Debug.Log("RANDOM FAILED: " + randomInt + " > " + percentChance);
                return false;
            }
        }
    }


       
}