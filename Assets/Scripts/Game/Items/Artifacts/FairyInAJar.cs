using UnityEngine;

public class FairyInAJar : Item, IOnTileMatchItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(3)] private int minComboCountToHeal = 4;
	[SerializeField][Min(0)] private float healMultiplication = 1f;

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
		int heal = (int)(count * healMultiplication);

		if (owner == null || owner.IsDying() || owner.IsDead() || count < minComboCountToHeal || heal <= 0)
		{
			Inventory.Instance.NotifyTileItemFinished();
			return;
		}

		owner.Heal(heal);
		//Debug.Log($"Healed up to {heal}");

		Inventory.Instance.NotifyTileItemFinished();

		ActivateAnimation();
	}
}
