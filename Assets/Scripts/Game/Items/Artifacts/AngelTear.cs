using UnityEngine;
using Utils;

public class AngelTear : Item, IOnPlayerPreHealItem
{
	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private float healBonusMultiplication = 1f;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerPreHeal(IntRef heal)
	{
		if (heal.Value <= 0 || owner.IsDying() || owner.IsDead()) return;

		int bonusHeal = (int)(heal.Value * healBonusMultiplication);

		if (bonusHeal > 0)
		{
			//Debug.Log($"Original heal was {heal.Value}");
			heal.Value += bonusHeal;
			//Debug.Log($"It was buffed for {healBonusMultiplication * 100}% it become {heal.Value}");

			ActivateAnimation();
		}
	}
}
