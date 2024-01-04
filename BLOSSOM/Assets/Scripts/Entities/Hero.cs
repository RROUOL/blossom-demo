using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using ElementEnum;

[CreateAssetMenu(fileName = "New Entity", menuName = "Hero")]
public class Hero : Entity
{
    [Header("Hero Stats")]
    public string seenName;

    [Space]

    public Boon[] equippedItems = new Boon[1];

    [Header("Base Elemental Res.")]
    [SerializeField] public ElementStatus baseSlash;
    [SerializeField] public ElementStatus baseStrike;
    [SerializeField] public ElementStatus basePierce;
    [Space]
    [SerializeField] public ElementStatus baseAir;
    [SerializeField] public ElementStatus baseFire;
    [SerializeField] public ElementStatus baseEarth;
    [SerializeField] public ElementStatus baseLife;
    [SerializeField] public ElementStatus baseIce;
    [SerializeField] public ElementStatus baseLightning;
    [SerializeField] public ElementStatus baseNoise;
}
