using UnityEngine;

public class TomeOfKnowledge : Item, IOnTileMatchItem, IOnEnemyPreTakeDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(3)] private int minRequiredMatch = 3;
	[SerializeField][Min(1)] private int damage = 1;
	[SerializeField] public int comboCounter = 0;
	[SerializeField] private float comboCoefficient = 1f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnTileMatched(Tile tile, int count)
	{
		if (count < minRequiredMatch || tile == null || !tile.IsPrimarilyOffensive())
		{
			Inventory.Instance?.NotifyTileItemFinished();
			return;
		}

		comboCounter ++;
		
		ActivateAnimation();

		Inventory.Instance?.NotifyTileItemFinished();
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (damage.amount <= 0 || enemy == null || enemy.IsDying() || enemy.IsDead() || damage.statusEffectName != ""  || damage.isBonusDamage) return;

		int bonusDamage = (int)(comboCounter * comboCoefficient);

		if (bonusDamage <= 0) return;

		// Debug.Log("Buffing by " + bonusDamage);
		damage.amount += bonusDamage;

		ActivateAnimation();

	}
}
