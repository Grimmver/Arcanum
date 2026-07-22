using UnityEngine;

public class HealingStaff : Item, IOnTileMatchItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int minRequiredComboCount = 4;
	[SerializeField] private int healAmount = 2;

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
		if (count < minRequiredComboCount) return;

		if (healAmount > 0)
		{
			owner.Heal(healAmount);
			//Debug.Log($"Combo count is {count}, required is {minRequiredComboCount}, so Owner was healed to {healAmount}");
			ActivateAnimation();
		}

		Inventory.Instance.NotifyTileItemFinished();
	}
}
