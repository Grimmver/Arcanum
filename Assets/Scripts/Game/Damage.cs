using System;
using UnityEngine;

public enum DamageType
{
	Fire,
	Ice,
	Lighting,
	Physical,
	Blood,
	Poison,
	None,
}

[Serializable]
public class Damage
{
	public int amount;
	public DamageType type;
	public Character sender;
	public bool isBonusDamage;
	public string statusEffectName = "";

	public Damage(int amount, DamageType type, Character sender, bool isBonusDamage = false)
	{
		this.amount = amount;
		this.type = type;
		this.sender = sender;
		this.isBonusDamage = isBonusDamage;
	}

	public Damage(int amount, DamageType type, Character sender, string statusEffectName)
	{
		this.amount = amount;
		this.type = type;
		this.sender = sender;
		this.statusEffectName = statusEffectName;
	}

	public Damage(int amount, DamageType type, Character sender, string statusEffectName, bool isBonusDamage)
	{
		this.amount = amount;
		this.type = type;
		this.sender = sender;
		this.statusEffectName = statusEffectName;
		this.isBonusDamage = isBonusDamage;
	}
}
