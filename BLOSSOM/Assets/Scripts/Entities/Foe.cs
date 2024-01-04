using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vaz.Constants.Consts;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Entity", menuName = "Foe")]
public class Foe : Entity
{
    [Header("Foe Stats")]
    [Tooltip("The weights for each individual move: 0-1: basic attacks, 2-9: special moves")]
    public int[] baseMoveWeights = new int[10];
    private int[] calcMoveWeights = new int[10];

    [Tooltip("The weights for each individual target: 0-6: ally foes, 7-10: enemy heroes")]
    private int[] baseTargetWeights = new int[11];
    private int[] calcTargetWeights = new int[11];

    public void resetBattleWeights()
    {
        for (int i = 0; i < calcMoveWeights.Length; i += 1)
        {
            calcMoveWeights[i] = baseMoveWeights[i];
        }

        for (int i = 0; i < calcTargetWeights.Length; i += 1)
        {
            calcTargetWeights[i] = 50;
        }

        Debug.Log(this.name + " has had their weights reset.");
    }

    public virtual int DecideBattleAction(BattleInfo battleInfo, Text textBox)
    {
        Debug.Log(this.name + " decides battle action..");
        //Debug.Log(this.name + " Slash weight is " + calcMoveWeights[0]);

        Move selectedMove = new Move();
        List<Entity> selectedTargets = new List<Entity>();

        // Placeholder - automatically use basic attack 1 on lorelei
        //selectedMove = basicAttacks[0];
        //selectedTargets.Add(battleInfo.enemyHeroes[0]);
        //Debug.Log("Using " + selectedMove.name + " on " + battleInfo.enemyHeroes[0].seenName);

        

        // decide which move should be weighted most
        // choose move and targets according to which decision is taken

        for (int i = 0; i < basicAttacks.Length; i += 1)
        {
            if (basicAttacks[i] != null)
            {
                if (calcMoveWeights[i] <= 0)
                {
                    calcMoveWeights[i] = 1;
                }
                if (i == 0)
                {
                    //Debug.Log("Slash being evaluated. " + calcMoveWeights[i]);
                }
                EvaluateMove(basicAttacks[i], i, battleInfo);
            }
        } 
        for (int i = 0; i < moveList.Length; i += 1)
        {
            if (moveList[i] != null)
            {
                if (calcMoveWeights[i+2] <= 0) 
                {
                    calcMoveWeights[i] = 1;
                }
                EvaluateMove(moveList[i], i+2, battleInfo);
            }
        }

        int chosenIndex = 0;

        if (Consts.RNG_ENEM_TABL)
        { // randomly choose move from calcMoveWeights
            Debug.Log("RNG ENEMY BEHAVIOUR - MOVE DECISION");
            int totalWeight = 0;
            selectedMove = basicAttacks[0];

            for (int i = 0; i < calcMoveWeights.Length; i += 1)
            {
                totalWeight += calcMoveWeights[0];
            }

            int randomNumber = Consts.GetRangeNum(totalWeight);
            Debug.Log("Random move number generated: " + randomNumber);

            for (int i = 1; i < calcMoveWeights.Length; i += 1)
            {
                if (i < 2)
                { // basic moves
                    if (basicAttacks[i] != null)
                    {
                        if (randomNumber <= calcMoveWeights[i])
                        {
                            chosenIndex = i;
                            selectedMove = basicAttacks[i];
                            Debug.Log("Selected Move is now " + basicAttacks[i] + " with weight " + calcMoveWeights[i] + " after " + randomNumber);
                        }
                        else
                        {
                            randomNumber -= calcMoveWeights[i];
                            Debug.Log("Random number reduced to " + randomNumber);
                        }
                    }
                }
                else
                { // special moves
                    if (moveList[i-2] != null)
                    {
                        if (randomNumber <= calcMoveWeights[i])
                        {
                            chosenIndex = i;
                            selectedMove = moveList[i-2];
                            Debug.Log("Selected Move is now " + moveList[i-2] + " with weight " + calcMoveWeights[i] + " after " + randomNumber);
                        }
                        else
                        {
                            randomNumber -= calcMoveWeights[i];
                            Debug.Log("Random number reduced to " + randomNumber);
                        }
                    }
                }
            }

            Debug.Log("CHOSEN MOVE IS NOW " + selectedMove);
        }
        else
        { // choose highest calcMoveWeights
            Debug.Log("FIXED ENEMY BEHAVIOUR - MOVE DECISION");
            int highestWeight = calcMoveWeights[0];
            selectedMove = basicAttacks[0];
            Debug.Log(selectedMove.name + " weight is " + highestWeight);

            for (int i = 1; i < calcMoveWeights.Length; i += 1)
            {
                if (i < 2)
                { // basic moves
                    if (basicAttacks[i] != null)
                    {
                        Debug.Log("Evaluating if " + highestWeight + " <= element " + i + " " + calcMoveWeights[i]);
                        if (highestWeight <= calcMoveWeights[i])
                        {
                            chosenIndex = i;
                            selectedMove = basicAttacks[i];
                            Debug.Log("Selected Move is now " + basicAttacks[i] + " with weight " + calcMoveWeights[i]);
                        }
                    }
                }
                else
                { // special moves
                    if (moveList[i-2] != null)
                    {
                        Debug.Log("Evaluating if " + highestWeight + " <= element " + i + " " + calcMoveWeights[i]);
                        if (highestWeight <= calcMoveWeights[i])
                        {
                            chosenIndex = i;
                            selectedMove = moveList[i-2];
                            Debug.Log("Selected Move is now " + moveList[i-2] + " with weight " + calcMoveWeights[i]);
                        }
                    }
                }
            }
        }

        // reset chosen weight, virtually guarenteeing that the move is least likely to be used next turn
        //calcMoveWeights[chosenIndex] = baseMoveWeights[chosenIndex];
        calcMoveWeights[chosenIndex] = 1;

        // now that the selectedMove has been found, choose the target
        if (selectedMove.hitsAll)
        { // if hits all, bypass weight system
            if (selectedMove.isDamaging)
            {
                // target hero team
                for (int i = 0; i < battleInfo.enemyHeroes.Length; i += 1)
                {
                    selectedTargets.Add(battleInfo.enemyHeroes[i]);
                }
            }
            else
            {
                // target all ally foes
                for (int i = 0; i < battleInfo.allyFoes.Length; i += 1)
                {
                    selectedTargets.Add(battleInfo.allyFoes[i]);
                }
            }
        }
        else
        { // else, consider weight system

            // update weights based on move used
            if (selectedMove.isDamaging)
            {
                // weight enemy heroes
                for (int i = 0; i < battleInfo.enemyHeroes.Length; i += 1)
                {
                    if (battleInfo.enemyHeroes[i] != null)
                    {
                        if (!battleInfo.enemyHeroes[i].isDowned)
                        {
                            calcTargetWeights[i+7] += 5;

                            int dmgValue = (int)selectedMove.elementType;
                            int enumVal;

                            switch (dmgValue)
                            {
                                case 0: // fae (always x1.5)
                                    calcTargetWeights[i+7] += 25;
                                    break;
                                // PHYSICAL
                                case 1: // slash
                                    enumVal = (int)battleInfo.enemyHeroes[i].Slash;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 2: // strike
                                    enumVal = (int)battleInfo.enemyHeroes[i].Strike;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 3: // pierce
                                    enumVal = (int)battleInfo.enemyHeroes[i].Pierce;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;

                                // MAGICAL
                                case 4: // air
                                    enumVal = (int)battleInfo.enemyHeroes[i].Air;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 5: // fire
                                    enumVal = (int)battleInfo.enemyHeroes[i].Fire;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 6: // earth
                                    enumVal = (int)battleInfo.enemyHeroes[i].Earth;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 7: // life
                                    enumVal = (int)battleInfo.enemyHeroes[i].Life;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 8: // ice
                                    enumVal = (int)battleInfo.enemyHeroes[i].Ice;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 9: // lightning
                                    enumVal = (int)battleInfo.enemyHeroes[i].Lightning;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                case 10: // noise
                                    enumVal = (int)battleInfo.enemyHeroes[i].Noise;
                                    calcTargetWeights[i+7] += checkAffMult(enumVal);
                                    break;
                                default:
                                    calcTargetWeights[i+7] += 0;
                                    break;
                            }
                        }
                        else
                        {
                            calcTargetWeights[i+7] -= 35;
                            if (calcTargetWeights[i+7] < 1) calcTargetWeights[i+7] = 1;
                            Debug.Log("target wasn't evaluated as a target because they're down");
                        }
                    }
                }
            }
            else
            {
                // weight ally foes
                for (int i = 0; i < battleInfo.allyFoes.Length; i += 1)
                {
                    if (battleInfo.allyFoes[i] != null)
                    {
                        Debug.Log(battleInfo.allyFoes[i]);
                        int dmgValue = (int)selectedMove.elementType;
                        int enumVal;

                        switch (dmgValue)
                        {
                            case 0: // fae (always x1.5)
                                calcTargetWeights[i] += 25;
                                break;
                            // PHYSICAL
                            case 1: // slash
                                enumVal = (int)battleInfo.allyFoes[i].Slash;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 2: // strike
                                enumVal = (int)battleInfo.allyFoes[i].Strike;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 3: // pierce
                                enumVal = (int)battleInfo.allyFoes[i].Pierce;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;

                            // MAGICAL
                            case 4: // air
                                enumVal = (int)battleInfo.allyFoes[i].Air;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 5: // fire
                                enumVal = (int)battleInfo.allyFoes[i].Fire;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 6: // earth
                                enumVal = (int)battleInfo.allyFoes[i].Earth;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 7: // life
                                enumVal = (int)battleInfo.allyFoes[i].Life;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 8: // ice
                                enumVal = (int)battleInfo.allyFoes[i].Ice;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 9: // lightning
                                enumVal = (int)battleInfo.allyFoes[i].Lightning;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            case 10: // noise
                                enumVal = (int)battleInfo.allyFoes[i].Noise;
                                calcTargetWeights[i] += checkAffMult(enumVal);
                                break;
                            default:
                                calcTargetWeights[i] += 0;
                                break;
                        }
                    }
                }
            }
            
            // now decide how the weights will be used
            if (Consts.RNG_ENEM_TABL)
            { // randomly choose target from calcTargetWeights
                Debug.Log("RNG ENEMY BEHAVIOUR - TARGET DECISION");
                int totalWeight = 0;
                int chosenTarget;



                if (selectedMove.isDamaging)
                {
                    chosenTarget = 4;
                    for (int i = 7; i < 11; i += 1)
                    {
                        totalWeight += calcTargetWeights[i];
                    }
                    int randomNumber = Consts.GetRangeNum(totalWeight);
                    Debug.Log("Random target number for all heroes generated: " + randomNumber);

                    for (int i = 7; i < 11; i += 1)
                    {
                        if (battleInfo.enemyHeroes[i-7] != null)
                        {
                            Debug.Log("Getting index " + i + ", " + (i-7) + ", name: " + battleInfo.enemyHeroes[i-7].name + " with weight: " + calcTargetWeights[i]);

                            if (randomNumber <= calcTargetWeights[i])
                            {
                                chosenTarget = i;
                                Debug.Log("Selected Target contains " + selectedTargets.Count + " characters and has added " + battleInfo.enemyHeroes[i-7]);
                            }
                            else
                            {
                                randomNumber -= calcTargetWeights[i];
                                Debug.Log("Random number reduced to " + randomNumber);
                            }
                        }
                    }
                    Debug.Log("Random number final for choosing heroes target: " + randomNumber);
                }
                else
                {
                    chosenTarget = 0;
                    for (int i = 0; i < 6; i += 1)
                    {
                        totalWeight += calcTargetWeights[i];
                    }
                    int randomNumber = Consts.GetRangeNum(totalWeight);
                    Debug.Log("Random target number for all foes generated: " + randomNumber);

                    for (int i = 0; i < 6; i += 1)
                    {
                        if (battleInfo.allyFoes[i] != null)
                        {
                            Debug.Log("Getting index " + i + ", name: " + battleInfo.allyFoes[i].name + " with weight: " + calcTargetWeights[i]);

                            if (randomNumber <= calcTargetWeights[i])
                            {
                                chosenTarget = i;
                                Debug.Log("Selected Target contains " + selectedTargets.Count + " characters and has added " + battleInfo.allyFoes[i]);
                            }
                            else
                            {
                                randomNumber -= calcTargetWeights[i];
                            }
                        }
                    }
                    Debug.Log("Random number final for choosing foes target: " + randomNumber);
                }


                Debug.Log("chosenIndex is: " + chosenTarget + ", " + calcTargetWeights[chosenTarget] + " weight returned to 1");
                calcTargetWeights[chosenTarget] = 1;

                Debug.Log("chosen index has landed on " + chosenTarget);
                if (chosenTarget > 6)
                {
                    Debug.Log("Index higher than 6! Adding hero " + battleInfo.enemyHeroes[chosenTarget-7]);
                    selectedTargets.Add(battleInfo.enemyHeroes[chosenTarget-7]);
                }
                else
                {
                    Debug.Log("Index lower than 7! Adding foe " + battleInfo.allyFoes[chosenTarget]);
                    selectedTargets.Add(battleInfo.allyFoes[chosenTarget]);
                }
            }
            else
            { // choose highest calcTargetWeights
                Debug.Log("FIXED ENEMY BEHAVIOUR - TARGET DECISION");
                int chosenTarget = 0;
                int highestWeight = calcTargetWeights[0];

                if (selectedMove.isDamaging)
                {
                    for (int i = 7; i < 11; i += 1)
                    {
                        if (battleInfo.enemyHeroes[i-7] != null)
                        {
                            if (highestWeight <= calcTargetWeights[i])
                            {
                                chosenTarget = i;
                                highestWeight = calcTargetWeights[i];
                                Debug.Log("Selected Target is now " + battleInfo.enemyHeroes[i-7] + " with weight " + calcTargetWeights[i]);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i += 1)
                    {
                        if (battleInfo.allyFoes[i] != null)
                        {
                            Debug.Log("Evaluating potential target " + battleInfo.allyFoes[i] + " with weight " + calcTargetWeights[i] + " VS current target " + battleInfo.allyFoes[chosenIndex] + " with weight " + calcTargetWeights[chosenIndex]);
                            if (highestWeight < calcTargetWeights[i])
                            {
                                chosenTarget = i;
                                highestWeight = calcTargetWeights[i];
                                Debug.Log("Selected Target is now " + battleInfo.allyFoes[i] + " with weight " + calcTargetWeights[i]);
                            }
                        }
                    }
                }

                Debug.Log("chosenIndex is: " + chosenTarget + ", " + calcTargetWeights[chosenTarget] + " weight returned to 1");
                calcTargetWeights[chosenTarget] = 1;

                Debug.Log("chosen index has landed on " + chosenTarget);
                if (chosenTarget > 6)
                {
                    Debug.Log("Index higher than 6! Adding hero " + battleInfo.enemyHeroes[chosenTarget-7]);
                    selectedTargets.Add(battleInfo.enemyHeroes[chosenTarget-7]);
                }
                else
                {
                    Debug.Log("Index lower than 7! Adding foe " + battleInfo.allyFoes[chosenTarget]);
                    selectedTargets.Add(battleInfo.allyFoes[chosenTarget]);
                }
            }
        }
    
        textBox.text = this.name + " uses " + selectedMove.name + "!";

        if (!selectedMove.hitsAll)
        {
            Debug.Log(this.name + " uses " + selectedMove + " on " + selectedTargets[0]);
        }
        else
        {
            Debug.Log(this.name + " uses " + selectedMove);
        }

        Entity[] targetsArray = selectedTargets.ToArray();

        return selectedMove.UseMove(this, targetsArray);
    }
    
    private void EvaluateMove(Move evalMove, int moveIndex, BattleInfo battleInfo)
    {
        //if (move is taunt/gurgle)
        //{
        //    baseMoveWeights[i] += 25;
        //    (baseMoveWeights[i] is changed to 0 upon using this taunt/gurgle)
        //}

        if (!evalMove.isDamaging)
        {
            for (int i = 0; i < battleInfo.allyFoes.Length; i += 1)
            {
                if (battleInfo.allyFoes[i] != null)
                {
                    float hpPercent = (battleInfo.allyFoes[i].currentHP / battleInfo.allyFoes[i].HealthPoints.Value);
                    if (hpPercent <= 0.25f)
                    {
                        calcMoveWeights[moveIndex] += (50 - (int)(hpPercent * 100));
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < battleInfo.enemyHeroes.Length; i += 1)
            {
                if (battleInfo.enemyHeroes[i] != null)
                {
                    Debug.Log("Evaluating move " + moveIndex + " on target " + battleInfo.enemyHeroes[i].seenName);
                    //Debug.Log("before: " + calcMoveWeights[moveIndex]);
                    if (!battleInfo.enemyHeroes[i].isDowned)
                    {
                        int dmgValue = (int)evalMove.elementType;
                        int enumVal;

                        switch (dmgValue)
                        {
                            case 0: // fae (always x1.5)
                                calcMoveWeights[moveIndex] += 25;
                                break;
                            // PHYSICAL
                            case 1: // slash
                                enumVal = (int)battleInfo.enemyHeroes[i].Slash;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 2: // strike
                                enumVal = (int)battleInfo.enemyHeroes[i].Strike;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 3: // pierce
                                enumVal = (int)battleInfo.enemyHeroes[i].Pierce;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;

                            // MAGICAL
                            case 4: // air
                                enumVal = (int)battleInfo.enemyHeroes[i].Air;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 5: // fire
                                enumVal = (int)battleInfo.enemyHeroes[i].Fire;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 6: // earth
                                enumVal = (int)battleInfo.enemyHeroes[i].Earth;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 7: // life
                                enumVal = (int)battleInfo.enemyHeroes[i].Life;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 8: // ice
                                enumVal = (int)battleInfo.enemyHeroes[i].Ice;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 9: // lightning
                                enumVal = (int)battleInfo.enemyHeroes[i].Lightning;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            case 10: // noise
                                enumVal = (int)battleInfo.enemyHeroes[i].Noise;
                                calcMoveWeights[moveIndex] += checkAffMult(enumVal);
                                break;
                            default:
                                calcMoveWeights[moveIndex] += 0;
                                break;
                        }
                        if (calcMoveWeights[moveIndex] <= 0)
                        {
                            calcMoveWeights[moveIndex] = 1;
                        }
                    }
                    //Debug.Log("after: " + calcMoveWeights[moveIndex]);
                }
            }
        }
    }

    private int checkAffMult(int enumValue)
    {
        int weighting = 0;

        switch (enumValue)
        {
            // for cases below, send message that turns should be changed
            case 0: // super-effective
                weighting += 30;
                break;
            case 1: // normal-effective
                weighting += 15;
                break;
            case 2: // weak-effective
                weighting -= 15;
                break;
            case 3: // immune
                weighting -= 30;
                break;
            case 4: // deflect
                weighting -= 30;
                break;
            case 5: // drain
                weighting -= 45;
                break;
            default:
                Debug.Log("How");
                break;
        }

        return weighting;
    }
}