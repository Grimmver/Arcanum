using System.Collections.Generic;
using UnityEngine;

public class LuckModifyingItem : Item
{
	[Header("Elements Updating Settings")]
	[SerializeField] public int luckAmount = 0;

	public override void Equip(Player player)
	{
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
