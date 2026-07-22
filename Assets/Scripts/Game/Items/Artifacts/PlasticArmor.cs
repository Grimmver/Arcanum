using UnityEngine;

public class PlasticArmor : Item, IOnPlayerPreTakeDamageItem, IOnWaveSpawnItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int linearScalingDamageReduction = 1;
	[SerializeField] private int battlesToScale = 10;
	[SerializeField][Min(0)] public int currentDamageReduction = 3;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerPreTakeDamage(Damage damage)
	{
		if (owner == null || owner.IsDying() || owner.IsDead() || damage.amount <= 0) return;

		if (currentDamageReduction <= 0) return;

		//Debug.Log($"Original damage was {damage.amount}");
		damage.amount = Mathf.Max(0, damage.amount - currentDamageReduction);
		//Debug.Log($"Now it was reduced by {currentDamageReduction}, so it is {damage.amount}");

		ActivateAnimation();
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null || owner.IsDying() || owner.IsDead()) return;

		if (EnemyWaveManager.Instance == null) return;

		int battleNumber = EnemyWaveManager.Instance.TurnCounter;

		if (battleNumber > 0 && battleNumber % battlesToScale == 0)
		{
			currentDamageReduction += linearScalingDamageReduction;
			//Debug.Log($"Damage reduction was scaled, now it is {currentDamageReduction}, it started from {constantDamageReduction}, " +
			//	$"linearly scaling for {linearScalingDamageReduction} in each {battlesToScale} battles, now it is battle number {battleNumber}");

			ActivateAnimation();
		}
	}
}
