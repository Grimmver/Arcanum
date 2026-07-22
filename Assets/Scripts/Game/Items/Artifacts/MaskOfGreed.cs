using UnityEngine;

public class MaskOfGreed : Item, IOnEnemyTakeExcessiveDamageItem
{
	[Header("Passive Trigger")]
	[SerializeField][Range(0f, 1f)] private float crystalDropChance = 1.0f;
	[SerializeField][Min(0f)] private float crystalMultiplier = 2f;
	[SerializeField][Range(0f, 1f)] private float coinDropChance = 0.1f;
	[SerializeField][Min(0f)] private float coinMultiplier = 0.25f;

	private const string CRYSTAL_PREF = "Crystal";
	private const string COIN_PREF = "Coin";

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyTakeExcessiveDamage(Enemy enemy, Damage damage)
	{
		int excessiveDamage = damage.amount;
		bool activated = false;

		if (Random.value < crystalDropChance)
		{
			int bonusCrystals = Mathf.CeilToInt(excessiveDamage * crystalMultiplier);
			PlayerPrefsEventBus.SetInt(CRYSTAL_PREF, PlayerPrefsEventBus.GetInt(CRYSTAL_PREF, 100) + bonusCrystals);
			//Debug.Log($"{bonusCrystals} Crystals added");

			activated = true;
		}

		if (Random.value < coinDropChance)
		{
			int bonusCoins = Mathf.CeilToInt(excessiveDamage * coinMultiplier);
			PlayerPrefsEventBus.SetInt(COIN_PREF, PlayerPrefsEventBus.GetInt(COIN_PREF) + bonusCoins);
			//Debug.Log($"{bonusCoins} Coins added");

			activated = true;
		}

		if (activated)
		{
			ActivateAnimation();
		}
	}
}
