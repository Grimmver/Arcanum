using System;
using UnityEngine;

public class TransformingStatusEffectDecorator : StatusEffectDecorator
{

	[SerializeField] public string transformInto;

	[Header("Balance")]
	[SerializeField] public int requiredStacks; // Amount of stacks required to cast new effect.
	[SerializeField] public int vitalityPerExtraStack = 1000; // Amount of HP+Armor to increase required stacks by 1.
	[SerializeField] public int rewardInitial; // Amount of stacks of new effect applied for getting requiredStacks.
	[SerializeField] public int rewardConsequent; // Amount of extra new effect stacks applied per each extra requiredStacks.

	public override void ProcessStatusEffect(Action onComplete)
	{

		int trueRequiredStacks = requiredStacks + (GetOwner().MaxHealth + GetOwner().Armor) / vitalityPerExtraStack;

		if (GetCurrentStack() < trueRequiredStacks)
		{
			_baseEffect.ProcessStatusEffect(() =>
				{
					onComplete?.Invoke();
				}
			);
			return;
		}

		Character originalStackOwner = GetOldestStack();

		int streaks = GetCurrentStack() / trueRequiredStacks, totalStacksUsed = streaks * trueRequiredStacks;

		DecrementStack(totalStacksUsed);

		GetOwner().ApplyStatusEffect(transformInto, rewardInitial + (streaks - 1) * rewardConsequent, originalStackOwner);

		_baseEffect.ProcessStatusEffect(() =>
			{
				onComplete?.Invoke();
			}
		);
	}

}
