using System;
using System.Collections.Generic;
using UnityEngine;

public class SenderHealTickStatusEffectDecorator : StatusEffectDecorator
{

	private Dictionary<Character, int> stackCredit = new Dictionary<Character, int>();

	[Header("Balance")]
	public int stacksRequired = 10;
	public float healPerStacks = 5f;

	public override void ProcessStatusEffect(Action onComplete)
	{

		foreach (KeyValuePair<Character, int> kvp in stackCredit)
		{
			Character sender = kvp.Key;
			int amount = (int)((kvp.Value / stacksRequired) * healPerStacks);
			if (amount > 0)
			{
				sender.Heal(amount);
			}
		}

		_baseEffect.ProcessStatusEffect(() =>
			{
				onComplete?.Invoke();
			}
		);
	}

	public override void DecrementStack(int amount = 1)
	{
		while (amount > 0)
		{
			amount--;
			Character oldestStackSender = _baseEffect.GetOldestStack();
			stackCredit[oldestStackSender]--;
			if (stackCredit[oldestStackSender] == 0)
			{
				stackCredit.Remove(oldestStackSender);
			}
			_baseEffect.DecrementStack();
		}
	}

	public override void Init(Character owner, int stack, Character sender)
	{
		base.Init(owner, stack, sender);
		if (stack <= 0)
		{
			return;
		}
		stackCredit[sender] = stack;
	}

	public override void ApplyAdditionalStack(int amount, Character sender)
	{
		base.ApplyAdditionalStack(amount, sender);
		if (amount <= 0)
		{
			return;
		}
		if (stackCredit.ContainsKey(sender))
		{
			stackCredit[sender] += amount;
		}
		else
		{
			stackCredit[sender] = amount;
		}
	}

}
