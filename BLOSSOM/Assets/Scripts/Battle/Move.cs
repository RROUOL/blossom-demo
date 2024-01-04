using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElementEnum;
using DamageNumbersPro;
using Kryz.CharacterStats;
using Vaz.Constants.Consts;
using Vaz.ManagerSingletons;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    [Header("Description")]
    public string moveDescription;

    [Header("Move Attributes")]
    [Tooltip("Number of press turns consumed when using the move (default = 2, a.k.a 1 press turn icon)")]
    public int consumedTurns = 2;
    private int turnsSpent = 2;
    [Space]
    public ElementType elementType;
    public StatType scalesWith; // attack stat used
    public StatType defendsWith; // defence stat used
    public int basePower;

    public int HealthPointCost;
    public int ManaPointCost;

    public bool isDamaging = true; // if false, move will heal

    public bool hitsAll = false;
    //public int numOfTargets = 1;

    public AilmentType[] recoveredStatus;
    public AilmentType[] inflictedStatus;

    // 0 = pugilism, 1 = occultism, 2 = constitution, 3 = negation
    public int[] inflictedBuffs;
    public int[] inflictedDebuffs;

    public StatModifier[] inflictedModifiers;

    private Entity userCasting;
    private Entity targetHitting;

    [Header("Sound Effects")]
    [Tooltip("The sound effect that plays when the move is used. Only plays once, as opposed to other sounds which play for every single target.")]
    [SerializeField] AudioClip moveUsedSound;
    
    [Tooltip("Normal hit sound effect.")]
    [SerializeField] AudioClip normalHitSound;
    [Tooltip("Supereffective hit sound effect.")]
    [SerializeField] AudioClip superHitSound;
    [Tooltip("Ineffective hit sound effect.")]
    [SerializeField] AudioClip weakHitSound;
    [Tooltip("Immune hit sound effect.")]
    [SerializeField] AudioClip immuneHitSound;
    [Tooltip("Deflect hit sound effect.")]
    [SerializeField] AudioClip deflectHitSound;
    [Tooltip("Drain hit sound effect.")]
    [SerializeField] AudioClip drainHitSound;
    [Tooltip("Overload hit sound effect.")]
    [SerializeField] AudioClip overloadHitSound;

    [Header("Popups")]
    public DamageNumber damageAlert;
    public DamageNumber healAlert;
    [Space]
    public DamageNumber downAlert;
    [Space]
    public DamageNumber bleedAlert;
    public DamageNumber burnAlert;
    public DamageNumber gravAlert;
    public DamageNumber poisonAlert;
    public DamageNumber frostAlert;
    public DamageNumber shockAlert;
    public DamageNumber stunAlert;
    [Space]
    public DamageNumber superEffAlert;
    public DamageNumber weakEffAlert;
    public DamageNumber immuneAlert;
    public DamageNumber deflectAlert;
    [Space]
    public DamageNumber drainAlert;
    public DamageNumber overloadAlert;
    bool itDrained = false;
    [Space]
    [Header("Popups - RNG")]
    public DamageNumber missAlert;
    bool itMissed = false;
    public int chanceToMiss = 10;
    public DamageNumber critAlert;
    bool itCritted = false;
    public int chanceToCrit = 10;

    public virtual int UseMove(Entity user, Entity[] target)
    {
        GameManager.singleton.PlaySound(moveUsedSound);

        //Debug.Log("I (" + this.name + ") HAVE THIS >> " + target.Length + " << MANY TARGETS");

        for (int i = 0; i < target.Length; i += 1)
        {
            if (target[i] != null)
            {
                //Debug.Log("Target " + i + ": " + target[i].name);
            }
            else
            {
                //Debug.Log("Target " + i + " is null.");
            }
        }


        userCasting = user;

        // consume HP and MP cost
        user.currentHP -= HealthPointCost;

        if (user.currentHP < 1)
        {
            user.currentHP = 1;
        }

        user.currentMP -= ManaPointCost;

        if (user.currentMP < 1)
        {
            user.currentMP = 0;
        }


        // for each enemy targetted..
        for (int i = 0; i < target.Length; i += 1)
        {
            if (target[i] == null)
            {
                //Debug.Log("Target No. " + i + " is null!");
            }
            else
            {
                //Debug.Log("Target " + target[i].name + " is not null!");

                targetHitting = target[i];


                itMissed = false;
                if (Consts.RNG_MISS_CRIT)
                {// if accuracy checks are used..
                    Debug.Log("CHECKING RNG MISS");

                    if (Consts.ProcChance(chanceToMiss))
                    { // X% chance to miss
                        //Debug.Log("That shit missed lol");
                        itMissed = true;
                    }
                    else
                    {
                        //Debug.Log("That DID NOT MISS lmao");
                        itMissed = false;
                    }
                }

                // if missed, immediately skip this enemy and all interactions
                if (itMissed)
                {
                    //Debug.Log("The move has missed " + targetHitting.name);
                    DamageNumber missNumber = missAlert.Spawn(targetHitting.currentLocation.position);
                    itMissed = false;
                }
                else 
                {
                    // recover statuses
                    for (int l = 0; l < recoveredStatus.Length; l += 1)
                    {
                        int enumValue = (int)recoveredStatus[l];

                        switch (enumValue)
                        {
                            case 0: // bleeding
                                targetHitting.isBleeding = false;
                                break;
                            case 1: // burning
                                targetHitting.isBurning = false;
                                break;
                            case 2: // gravitas
                                targetHitting.isGravitated = false;
                                break;
                            case 3: // poison
                                targetHitting.isPoisoned = false;
                                break;
                            case 4: // frostbite
                                targetHitting.isFrostbitten = false;
                                break;
                            case 5: // shock
                                targetHitting.isShocked = false;
                                break;
                            case 6: // stun
                                targetHitting.isStunned = false;
                                break;
                            case 7: // down
                                targetHitting.isDowned = false;
                                break;
                            
                        }
                    }

                    //Debug.Log(targetHitting.name + " HP before: " + targetHitting.currentHP);

                    if (targetHitting.isDowned)
                    {
                        //Debug.Log("The move has missed " + targetHitting.name + " because they're downed.");
                        DamageNumber missNumber = missAlert.Spawn(targetHitting.currentLocation.position);
                    }
                    else
                    {   
                        // in case of deflection during CalculateDamage, this code is 2 lines
                        int damageDealt = CalculateDamage();
                        targetHitting.currentHP -= damageDealt;

                        //Debug.Log(targetHitting.name + " HP after: " + targetHitting.currentHP);

                        if (targetHitting.currentHP < 1 && targetHitting.Down != AilmentStatus.Immune)
                        {
                            //Debug.Log(targetHitting.name + " has been downed after reaching " + targetHitting.currentHP + " HP!");
                            targetHitting.isDowned = true;
                            targetHitting.currentHP = 0;
                        }

                        if (targetHitting.currentHP > targetHitting.HealthPoints.Value)
                        {
                            targetHitting.currentHP = (int)targetHitting.HealthPoints.Value;
                            //Debug.Log(targetHitting.name + " has reached the maximum HP of " + targetHitting.HealthPoints.Value + ", " + targetHitting.currentHP + " HP!");
                        }

                        // apply statuses
                        for (int l = 0; l < inflictedStatus.Length; l += 1)
                        {
                            int enumValue = (int)inflictedStatus[l];

                            switch (enumValue)
                            {
                                case 0: // bleeding
                                    if (targetHitting.Bleeding != AilmentStatus.Immune)
                                    {
                                        targetHitting.isBleeding = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to bleeding.");
                                    }
                                    break;
                                case 1: // burning
                                    if (targetHitting.Burning != AilmentStatus.Immune)
                                    {
                                        targetHitting.isBurning = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to burning.");
                                    }
                                    break;
                                case 2: // gravitas
                                    if (targetHitting.Gravitas != AilmentStatus.Immune)
                                    {
                                        targetHitting.isGravitated = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to gravitas.");
                                    }
                                    break;
                                case 3: // poison
                                    if (targetHitting.Poison != AilmentStatus.Immune)
                                    {
                                        targetHitting.isPoisoned = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to poison.");
                                    }
                                    break;
                                case 4: // frostbite
                                    if (targetHitting.Frostbite != AilmentStatus.Immune)
                                    {
                                        targetHitting.isFrostbitten = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to frostbite.");
                                    }
                                    break;
                                case 5: // shock
                                    if (targetHitting.Shock != AilmentStatus.Immune)
                                    {
                                        targetHitting.isShocked = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to shock.");
                                    }
                                    break;
                                case 6: // stun
                                    if (targetHitting.Stunned != AilmentStatus.Immune)
                                    {
                                        targetHitting.isStunned = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to stun.");
                                    }
                                    break;
                                case 7: // down
                                    if (targetHitting.Down != AilmentStatus.Immune)
                                    {
                                        targetHitting.isDowned = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Target " + i + " is immune to being downed.");
                                    }
                                    break;
                                
                            }
                        }
                    }
                }

            }
        }

        return turnsSpent;
    }

    int CalculateDamage()
    {
        itCritted = false;
        if (Consts.RNG_MISS_CRIT)
        {// if critical checks are used..
            Debug.Log("CHECKING RNG CRIT");

            if (Consts.ProcChance(chanceToCrit))
            { // X% chance to crit
                itCritted = true;
            }
            else
            {
                itCritted = false;
            }
        }

        // check enemies element affinities
        float affMult;
        int enumVal;

        int dmgValue;

        if (itCritted)
        {
            dmgValue = 0;
        }
        else
        {
            dmgValue = (int)elementType;
        }

        switch (dmgValue)
        {
            case 0: // fae (always x2.0)
                affMult = 2.0f;
                DamageNumber critNumber = critAlert.Spawn(targetHitting.currentLocation.position);
                break;
            // PHYSICAL
            case 1: // slash
                enumVal = (int)targetHitting.Slash;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 2: // strike
                enumVal = (int)targetHitting.Strike;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 3: // pierce
                enumVal = (int)targetHitting.Pierce;
                affMult = checkEffectMultiplier(enumVal);
                break;

            // MAGICAL
            case 4: // air
                enumVal = (int)targetHitting.Air;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 5: // fire
                enumVal = (int)targetHitting.Fire;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 6: // earth
                enumVal = (int)targetHitting.Earth;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 7: // life
                enumVal = (int)targetHitting.Life;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 8: // ice
                enumVal = (int)targetHitting.Ice;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 9: // lightning
                enumVal = (int)targetHitting.Lightning;
                affMult = checkEffectMultiplier(enumVal);
                break;
            case 10: // noise
                enumVal = (int)targetHitting.Noise;
                affMult = checkEffectMultiplier(enumVal);
                break;
            default:
                affMult = 1.0f;
                break;
        }

        // get enumValue of damage type & scaling stat


        int atkValue = (int)scalesWith;
        int atkNum;

        switch (atkValue)
        {
            case 0: // vigour
                atkNum = (int)userCasting.Vigour.Value;
                break;
            case 1: // mind
                atkNum = (int)userCasting.Mind.Value;
                break;
            case 2: // pugilism
                atkNum = (int)userCasting.Pugilism.Value;
                break;
            case 3: // occultism
                atkNum = (int)userCasting.Occultism.Value;
                break;
            case 4: // constitution
                atkNum = (int)userCasting.Constitution.Value;
                break;
            case 5: // negation
                atkNum = (int)userCasting.Negation.Value;
                break;
            case 6: // fate
                atkNum = (int)userCasting.Fate.Value;
                break;
            default:
            atkNum = 1;
                break;
        }
        
        int defValue;
        int defNum;

        defValue = (int)defendsWith;
        switch (defValue)
        {
            case 0: // vigour
                defNum = (int)targetHitting.Vigour.Value;
                break;
            case 1: // mind
                defNum = (int)targetHitting.Mind.Value;
                break;
            case 2: // pugilism
                defNum = (int)targetHitting.Pugilism.Value;
                break;
            case 3: // occultism
                defNum = (int)targetHitting.Occultism.Value;
                break;
            case 4: // constitution
                defNum = (int)targetHitting.Constitution.Value;
                break;
            case 5: // negation
                defNum = (int)targetHitting.Negation.Value;
                break;
            case 6: // fate
                defNum = 1;
                break;
            default:
                defNum = 1;
                break;
        }


        float floatDamage = (((float)basePower * ((float)atkNum / (float)defNum)) + 1);
        //floatDamage = ((35 * (7 /  8)) + 1)

        Debug.Log(floatDamage + " = ((" + basePower + " * (" + atkNum + " /  " + defNum + ")) + 1");

        floatDamage = floatDamage * affMult;

        Debug.Log(floatDamage + " = (" + floatDamage + " * " + affMult + ")");


        if (hitsAll)
        {
            Debug.Log("Damage reduced because hitsAll");
            floatDamage = floatDamage * 0.75f;
        }

        if (!isDamaging)
        {
            Debug.Log("Damage reversed because Healing");
            floatDamage = floatDamage * -1.0f;
        }

        if (targetHitting.isGuarding)
        {
            Debug.Log("Damage reduced because target is guarding");
            floatDamage = floatDamage * 0.5f;
        }

        // after everything is calculated, return totalDamage
        int totalDamage = (int)System.Math.Round(floatDamage, 0);

        //Debug.Log("Total damage is: " + totalDamage);

        if (totalDamage >= 0)
        { // damage
            //Debug.Log("DAMAGE NUMBER SPAWNED");
            DamageNumber damageNumber = damageAlert.Spawn(targetHitting.currentLocation.position, totalDamage);
        }
        else
        {   // healing (can't miss)
            Debug.Log("HEALING NUMBER SPAWNED");
            DamageNumber healNumber = healAlert.Spawn(targetHitting.currentLocation.position, (totalDamage * -1));
        }

        // did it drain? if so, did it overload?

        if (itDrained)
        {
            if (totalDamage >= 0)
            {
                targetHitting.turnsToGain -= 1; // lose 1 turn if OVERLOAD (drain does reverse damage), otherwise gain 2
                Debug.Log("OVERLOAD ON " + targetHitting.name);
                DamageNumber overloadNumber = overloadAlert.Spawn(targetHitting.currentLocation.position);
                GameManager.singleton.PlaySound(overloadHitSound);
            }
            else
            {
                targetHitting.turnsToGain += 2;
                Debug.Log("DRAIN ON " + targetHitting.name);
                DamageNumber drainNumber = drainAlert.Spawn(targetHitting.currentLocation.position);
                GameManager.singleton.PlaySound(drainHitSound);
            }
        }

        itDrained = false;

        if (Consts.RNG_DMG_VARI)
        {
            totalDamage = (int)Consts.DamVariance(totalDamage);
        }

        return totalDamage;
    }

    float checkEffectMultiplier(int enumValue)
    {
        float affinityMultiplier;

        turnsSpent = consumedTurns;

        switch (enumValue)
        {
            // for cases below, send message that turns should be changed
            case 0: // super-effective    
                affinityMultiplier = 1.5f;
                if (!targetHitting.isGuarding)
                {
                    if (turnsSpent >= 2)
                    {
                        turnsSpent -= 1;
                    }
                    Debug.Log("SUPER ON " + targetHitting.name);
                    DamageNumber superNumber = superEffAlert.Spawn(targetHitting.currentLocation.position);
                    GameManager.singleton.PlaySound(superHitSound);
                }
                else
                {
                    Debug.Log("SUPER GUARDED ON " + targetHitting.name);
                    GameManager.singleton.PlaySound(normalHitSound);

                }
                break;
            case 1: // normal-effective
                affinityMultiplier = 1.0f;
                GameManager.singleton.PlaySound(normalHitSound);
                break;
            case 2: // weak-effective
                affinityMultiplier = 0.5f;
                turnsSpent += 1;
                Debug.Log("WEAK ON " + targetHitting.name + " || " + targetHitting.currentLocation + " || " + targetHitting.currentLocation.position);
                DamageNumber weakNumber = weakEffAlert.Spawn(targetHitting.currentLocation.position);
                GameManager.singleton.PlaySound(weakHitSound);
                break;
            case 3: // immune
                affinityMultiplier = 0.0f;
                turnsSpent += 2;
                Debug.Log("IMMUNE ON " + targetHitting.name);
                DamageNumber immuneNumber = immuneAlert.Spawn(targetHitting.currentLocation.position);
                GameManager.singleton.PlaySound(immuneHitSound);
                break;
            case 4: // deflect
                affinityMultiplier = 1.0f;
                Debug.Log("DEFLECT ON " + targetHitting.name);
                DamageNumber deflectNumber = deflectAlert.Spawn(targetHitting.currentLocation.position);
                targetHitting = userCasting;
                GameManager.singleton.PlaySound(deflectHitSound);
                break;
            case 5: // drain
                affinityMultiplier = -2.0f;
                itDrained = true;
                break;
            default:
                Debug.Log("How");
                affinityMultiplier = 1.0f;
                GameManager.singleton.PlaySound(normalHitSound);
                break;
        }

        return affinityMultiplier;
    }
}