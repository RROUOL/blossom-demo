using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using ElementEnum;

public class Ailment : MonoBehaviour
{
    public Image icon;

    public bool activeBeforeTurn;
    public bool activeAfterTurn;

    public virtual void InitAilment(Entity e)
    { // give ailment to entity if not already present

        // .. and if not guarding
        if (e.isGuarding) 
            return;

        if (DoesListContain(e, this)) 
            e.AilmentList.Add(this);
    }

    public virtual void SufferBefore(Entity e)
    { // upon start of entity's turn

    }

    public virtual void SufferAfter(Entity e)
    { // upon end of entity's turn

    }

    public virtual void RemoveAilment(Entity e)
    {
        e.HealthPoints.RemoveAllModifiersFromSource(this);
        e.ManaPoints.RemoveAllModifiersFromSource(this);

        e.Vigour.RemoveAllModifiersFromSource(this);
        e.Mind.RemoveAllModifiersFromSource(this);
        e.Pugilism.RemoveAllModifiersFromSource(this);
        e.Occultism.RemoveAllModifiersFromSource(this);
        e.Constitution.RemoveAllModifiersFromSource(this);
        e.Negation.RemoveAllModifiersFromSource(this);
    }

    protected bool DoesListContain(Entity e, Ailment a)
    {
        bool alreadyInList = e.AilmentList.Contains(a);

        if (alreadyInList)
        {
            Debug.Log("ailment of type " + a + " already in " + e + "'s Ailment List. Will not add duplicate..");
        }
        else
        {
            Debug.Log("ailment of type " + a + " not in " + e + "'s Ailment List. Adding..");
        }

        return alreadyInList;
    }
}

public class Downed : Ailment
{
    public override void InitAilment(Entity e)
    {
        e.currentHP = 0;
        e.canMove = false;

        // all other ailments except permanents are removed when downed
        for (int i = 0; i < e.AilmentList.Count; i += 1)
        {
            e.AilmentList[i].RemoveAilment(e);
        }

        base.InitAilment(e);
    }

    public override void SufferBefore(Entity e)
    {
        e.currentHP = 0;
        e.canMove = false;
    }

    public override void SufferAfter(Entity e)
    {
        e.currentHP = 0;
        e.canMove = false;
    }

    public override void RemoveAilment(Entity e)
    {
        base.RemoveAilment(e);

        e.currentHP = 1;
        e.canMove = true;
    }
}

public class Bleeding : Ailment
{
    // BLEEDING
    //  InitAilment() - add 0.5x pugilism modifier
    //  SufferAfter() - reduce HP by 1/10th of max HP

    // SPECIAL
    //  InitAilment() - no pugilism modifier
    //  SufferAfter() - no health loss, pugilism raised by 1 stage

    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Burning : Ailment
{
    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Gravitated : Ailment
{
    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Poisoned : Ailment
{
    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Frostbitten : Ailment
{
    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Shocked : Ailment
{
    public override void InitAilment(Entity e)
    {
        
    }

    public override void SufferAfter(Entity e)
    {

    }
}

public class Stunned : Ailment
{
    // STUNNED
    //  InitAilment() - 
    //  SufferBefore() - can't move this turn, remove
    //  RemoveAilment() - add stun cooldown ailment

    // SPECIAL
    //  InitAilment() - 2x constitution, negation modifier

    public override void InitAilment(Entity e)
    {
        StunCooldown stunCooldown = new StunCooldown();
        if (DoesListContain(e, stunCooldown))
            return;

        if (e.Stunned == AilmentStatus.Immune)
            return;

        if (e.Stunned == AilmentStatus.Heal)
        {
            // heal 25% HP, doesn't get added to 
        }

        if (e.Stunned == AilmentStatus.Special)
        {
            
        }

        e.AilmentList.Add(this);
    }

    public override void SufferBefore(Entity e)
    {
        int enumValue = (int)e.Stunned;

        switch (enumValue)
        {
            case 0: // vulnerable, affected normally
                e.canMove = false;
                break;
            case 1: // immune, remove

                break;
            case 2: // heal, do the opposite of normal
                // takes effect and is removed in InitAilment(). treat as immune
                break;
            case 3: // special, very specific effect
                // stun special raises occultism of entity by 50%, happens in InitAilment
                break;
            default: // permanent or anything which somehow doesn't fit goes here. functionally same as vulnerable
                Debug.Log("Stunned enumvalue defaulted = " + (int)e.Stunned + " / " + e.Stunned);
                break;
        }
    }

    public override void RemoveAilment(Entity e)
    {
        if (e.Stunned == AilmentStatus.Permanent)
            return;

        base.RemoveAilment(e);
        StunCooldown stunCooldown = new StunCooldown();
        stunCooldown.InitAilment(e);
    }
}

public class StunCooldown : Ailment
{
    // this has no functionality for anything other than vulnerable because it will never need it.

    int cooldown = 1;
    public override void InitAilment(Entity e)
    {
        Stunned stunned = new Stunned();
        if (DoesListContain(e, stunned))
            return;

        base.InitAilment(e);
        
        cooldown = 1;
    }

    public override void SufferAfter(Entity e)
    {
        cooldown -= 1;

        if (cooldown < 1)
        {
            RemoveAilment(e);
        }
    }
}

// Buffs & Debuffs

public class StatBuff : Ailment
{ 
    // range between -2, 2. make it something huge like -5/+5 to minimise/maximise stat.  
    public int buffPugilism;
    public int buffOccultism;
    public int buffConstitution;
    public int buffNegation;
    
    public bool nullifyCurrentBuff; // whether or not it resets the buffs on the entity before applying

    public override void InitAilment(Entity e)
    { // unlike other ailments, this never interacts with AilmentList
        if (nullifyCurrentBuff)
        {
            e.InitBuffs();
        }

        e.AddBuffs(buffPugilism, buffOccultism, buffConstitution, buffNegation);
    }
}