using UnityEngine;

public class TricksterBag : TileElementsUpdatingItem
{

    [Header("Elements Updating Settings")]
	[SerializeField] public int luckAmount = 30;

	public override void Equip(Player player)
	{

        base.Equip(player);

		owner = player;
		if (owner == null) return;

		if (luckAmount > 0)
		{
			MatchManager.Instance.IncreaseLuck(luckAmount);
		}
		else if (luckAmount < 0)
		{
			MatchManager.Instance.DecreaseLuck(luckAmount);
		}
	}

	public override void Unequip()
	{

        base.Unequip();

		if (owner != null)
		{
			if (luckAmount > 0)
			{
				MatchManager.Instance.DecreaseLuck(luckAmount);
			}
			else if (luckAmount < 0)
			{
				MatchManager.Instance.IncreaseLuck(luckAmount);
			}
		}

		owner = null;
	}

}
