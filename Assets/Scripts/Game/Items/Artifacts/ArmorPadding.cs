using UnityEngine;

public class ArmorPadding : Item, IOnPlayerPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int minRequiredArmor = 50;
	[SerializeField] private int armorToScale = 10;
	[SerializeField] private int armorToAdd = 50;
	[SerializeField] private float coefficientK = 22.5f;
	[SerializeField] private float powerN = 1f;
	[SerializeField] private float resistanceCap = 0.75f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
		owner.AddArmor(armorToAdd);
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerPreTakeDamage(Damage damage)
	{
		if (owner == null || owner.IsDying() || owner.IsDead() || damage.amount <= 0) return;

		if (owner.Armor < minRequiredArmor) return;

		int scaleCount = (owner.Armor - minRequiredArmor) / armorToScale;

		if (scaleCount <= 0) return;

		float resistance = resistanceCap - (resistanceCap / (1 + Mathf.Pow((scaleCount / coefficientK), powerN)));

		// Debug.Log($"Damage was going to be{damage.amount}");
		damage.amount = Mathf.CeilToInt(damage.amount * (1f - resistance));
		// Debug.Log($"After reductio it is {damage.amount}");

		ActivateAnimation();
	}
}
