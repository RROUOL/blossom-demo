	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;

	namespace Kryz.CharacterStats
	{
		[Serializable]
		public class CharacterStat
		{
			public float BaseValue;

			[Tooltip("softMax is how high it can go without modifications - e.g. stats cannot go above 99 without modifications")]
			public float softMaxValue;
			[Tooltip("hardMax is how high it can go after modifications - e.g. stats cannot go above 198 with modifications")]
			public float hardMaxValue;
			public float MinValue;

			public int NumOfDecimals; // determines how many decimals the finalvalue is rounded down to after calculations

			protected bool isDirty = true;
			protected float lastBaseValue;

			protected float _value;
			public virtual float Value {
				get {
					if(isDirty || lastBaseValue != BaseValue) {
						lastBaseValue = BaseValue;
						_value = CalculateFinalValue();
						isDirty = false;
					}
					return (float)Math.Round(_value, NumOfDecimals);
				}
			}

			protected readonly List<StatModifier> statModifiers;
			public readonly ReadOnlyCollection<StatModifier> StatModifiers;

			public CharacterStat()
			{
				statModifiers = new List<StatModifier>();
				StatModifiers = statModifiers.AsReadOnly();
			}

			public CharacterStat(float baseValue, float minValue, float maxValue, int numOfDecimals) : this()
			{
				BaseValue = baseValue;
				MinValue = minValue;
				softMaxValue = maxValue;
				hardMaxValue = softMaxValue * 2;
				NumOfDecimals = numOfDecimals;
			}

			public CharacterStat(float baseValue, float minValue, float softmaxValue, float hardmaxValue, int numOfDecimals) : this()
			{
				BaseValue = baseValue;
				MinValue = minValue;
				softMaxValue = softmaxValue;
				hardMaxValue = hardmaxValue;
				NumOfDecimals = numOfDecimals;
			}

			public virtual void AddModifier(StatModifier mod)
			{
				isDirty = true;
				statModifiers.Add(mod);
				CalculateFinalValue();
			}

			public virtual bool RemoveModifier(StatModifier mod)
			{
				if (statModifiers.Remove(mod))
				{
					isDirty = true;
					return true;
				}
				CalculateFinalValue();
				return false;
			}

			public virtual bool RemoveAllModifiersFromSource(object source)
			{
				int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

				if (numRemovals > 0)
				{
					isDirty = true;
					return true;
				}
				return false;
			}

			protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
			{
				if (a.Order < b.Order)
					return -1;
				else if (a.Order > b.Order)
					return 1;
				return 0; //if (a.Order == b.Order)
			}
			
			protected virtual float CalculateFinalValue()
			{
				float finalValue = BaseValue;
				float sumPercentAdd = 0;

				statModifiers.Sort(CompareModifierOrder);

				for (int i = 0; i < statModifiers.Count; i++)
				{
					StatModifier mod = statModifiers[i];

					if (mod.Type == StatModType.Flat)
					{
						finalValue += mod.Value;
					}
					else if (mod.Type == StatModType.PercentAdd)
					{
						sumPercentAdd += mod.Value;

						if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
						{
							finalValue *= 1 + sumPercentAdd;
							sumPercentAdd = 0;
						}
					}
					else if (mod.Type == StatModType.PercentMult)
					{
						finalValue *= 1 + mod.Value;
					}
				}

				// check if the value goes over the maximum or minimum values

				if (finalValue > hardMaxValue)
				{
					finalValue = hardMaxValue;
				}

				if (finalValue < MinValue)
				{
					finalValue = MinValue;
				}

				Debug.Log(this + " value changed to " + finalValue);

				// Workaround for float calculation errors, like displaying 12.00001 instead of 12, rounds to however many decimals are chosen when constructed
				return (float)Math.Round(finalValue, NumOfDecimals);
			}
		}
	}
