using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
	private List<StatusEffect> activeEffects = new List<StatusEffect>();
	public List<StatusEffect> ActiveEffects => activeEffects;
	private Character owner;
	private bool isProcessing = false;

	[SerializeField] private StatusEffectDisplaySlot[] displaySlots;

	public void Init(Character owner)
	{
		this.owner = owner;
		displaySlots = owner.GetComponentsInChildren<StatusEffectDisplaySlot>(true);
		UpdateUI();
		// Debug.Log("Initiated under " + owner + " with " + displaySlots + " (" + displaySlots.Length + ")");
	}

	public void ApplyEffect(string effectName, int stack, Character sender)
	{

		if (stack <= 0)
		{
			return;
		}

		var existing = activeEffects.Find(e => e.GetBaseEffectType() == effectName);

		if (existing != null)
		{
			existing.ApplyAdditionalStack(stack, sender);
		}
		else if (activeEffects.Count < displaySlots.Length)
		{
			GameObject newEffectGO = RelevantStatusEffectManager.Instance.FindRelevantStatusEffectPrefab(effectName);
			newEffectGO.SetActive(false);

			StatusEffect newEffect = newEffectGO.GetComponent<StatusEffect>();
			newEffect.Init(owner, stack, sender);
			activeEffects.Add(newEffect);

			displaySlots[activeEffects.Count - 1].SetEffect(newEffect);
		}
		else
		{
			Debug.Log("Could not add effect due to insufficient slot space.");
		}

		UpdateUI();
	}


	public void ProcessStatusEffects(Action onComplete)
	{

		isProcessing = true;

		if (owner == null || owner.IsDying() || activeEffects == null || activeEffects.Count == 0)
		{
			isProcessing = false;
			onComplete?.Invoke();
			return;
		}

		int effectId = 0;

		void ExecuteNextEffect()
		{

			if (owner == null || owner.IsDying())
			{
				isProcessing = false;
				onComplete?.Invoke();
				return;
			}

			if (effectId >= activeEffects.Count)
			{
				RemoveInactiveStatusEffects(() =>
				{
					isProcessing = false;
					onComplete?.Invoke();
				}
				);
				return;
			}

			activeEffects[effectId].ProcessStatusEffect(() =>
			{
				effectId++;
				ExecuteNextEffect();
			});
		}

		ExecuteNextEffect();
	}

	public void RemoveInactiveStatusEffects(Action onComplete)
	{

		List<StatusEffect> stillActiveEffects = new List<StatusEffect>();

		for (int effectId = 0; effectId < activeEffects.Count; effectId++)
		{
			if (activeEffects[effectId] != null && activeEffects[effectId].GetCurrentStack() != 0)
			{
				stillActiveEffects.Add(activeEffects[effectId]);
			}
			else
			{
				Destroy(activeEffects[effectId].gameObject);
			}
		}

		activeEffects = stillActiveEffects;

		foreach (var slot in displaySlots)
		{
			slot.ClearSlot();
		}
		for (int effectId = 0; effectId < activeEffects.Count; effectId++)
		{
			displaySlots[effectId].SetEffect(activeEffects[effectId]);
		}

		UpdateUI();

		onComplete?.Invoke();
	}

	public void UpdateUI()
	{
		// Debug.Log("Updating UI...");
		for (int slotId = 0; slotId < displaySlots.Length; slotId++)
		{
			var effect = displaySlots[slotId].GetEffect();
			// Debug.Log("Slot " + slotId + ": " + effect);
			if (effect != null)
			{
				displaySlots[slotId].UpdateDisplay();
			}
			else
			{
				displaySlots[slotId].ClearSlot();
			}
		}
	}

	public bool IsStunned()
	{

		foreach (StatusEffect effect in activeEffects)
		{
			if (effect.IsStunning())
			{
				// Debug.Log(owner + " is stunned!");
				if (!isProcessing)
				{
					RemoveInactiveStatusEffects(null);
				}
				UpdateUI();
				return true;
			}
		}

		return false;
	}

	public bool IsTricked()
	{

		foreach (StatusEffect effect in activeEffects)
		{
			if (effect.IsTricking())
			{
				if (!isProcessing)
				{
					RemoveInactiveStatusEffects(null);
				}
				UpdateUI();
				return true;
			}
		}

		return false;
	}

	public float GetIncomingDamageModifier()
	{

		float resultingModifier = 1f;

		foreach (StatusEffect effect in activeEffects)
		{
			resultingModifier += effect.GetIncomingDamageModifier();
		}

		resultingModifier = Mathf.Max(0, resultingModifier);

		return resultingModifier;
	}

	public float GetIncomingTypedDamageModifier(DamageType damageType)
	{

		float resultingModifier = 1f;

		foreach (StatusEffect effect in activeEffects)
		{
			resultingModifier += effect.GetIncomingTypedDamageModifier(damageType);
		}

		resultingModifier = Mathf.Max(0, resultingModifier);

		return resultingModifier;
	}

	public float GetOutgoingDamageModifier()
	{

		float resultingModifier = 1f;

		foreach (StatusEffect effect in activeEffects)
		{
			resultingModifier += effect.GetOutgoingDamageModifier();
		}

		resultingModifier = Mathf.Max(0, resultingModifier);

		return resultingModifier;
	}

	public void ClearStatusEffects()
	{
		foreach (StatusEffect effect in activeEffects)
		{
			while (effect.GetCurrentStack() > 0) // This MIGHT come in handy in case some effects randomly prevent decrementing. Though should probably be replaceed at that point.
			{
				effect.DecrementStack(effect.GetCurrentStack());
			}
		}
		RemoveInactiveStatusEffects(null);
	}

	public void ParseDamage(Damage sentDamage)
	{
		foreach (StatusEffect effect in activeEffects)
		{
			effect.ParseDamage(sentDamage);
		}
		if (!isProcessing)
		{
			RemoveInactiveStatusEffects(null);
		}
	}

	public void RemoveNegativeStatusEffect()
	{

		List<StatusEffect> negativeEffects = new List<StatusEffect>();

		for (int effectId = 0; effectId < activeEffects.Count; effectId++)
		{
			if (activeEffects[effectId].IsNegative())
			{
				negativeEffects.Add(activeEffects[effectId]);
			}
		}

		if (negativeEffects.Count == 0)
		{
			return;
		}

		int randomIndex = UnityEngine.Random.Range(0, negativeEffects.Count);
		negativeEffects[randomIndex].DecrementStack();

		if (!isProcessing)
		{
			RemoveInactiveStatusEffects(null);
		}

		return;
	}
}
