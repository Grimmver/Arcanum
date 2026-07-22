using UnityEngine;

public class SaviourWine : Item, IOnPlayerEndTurnItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int healAmount = 5;

	public override void Equip(Player player)
	{
		owner = player;

		if (owner == null)
		{
			return;
		}

	}

	public override void Unequip()
	{

		if (owner != null)
		{
			owner = null;
		}

	}

	public void OnPlayerEndTurn()
	{
		owner.Heal(healAmount);
	}
}
