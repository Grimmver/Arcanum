using UnityEngine;

public class EarthArmor : TileElementsUpdatingItem, IOnTileMatchItem
{
	[Header("Stats")]
	[SerializeField] private int bonusArmor = 40;
	[SerializeField] private float resistanceCoefficient = 0.25f;
	[SerializeField] private DamageType resistanceType = DamageType.Physical;

	[Header("Passive Trigger")]
	[SerializeField][Min(0)] private int bonusArmorPerShieldMatch = 1;

	[Header("Sprite Changer")]
	[SerializeField] private Sprite earthArmorSprite;
	private Sprite originalArmorSprite;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		owner.AddArmor(bonusArmor);

		// MatchManager.Instance?.RefreshTiles();

		if (Player.Instance != null)
		{
			originalArmorSprite = Player.Instance.ArmorIcon.sprite;
			Player.Instance.ArmorIcon.sprite = earthArmorSprite;
		}

		if (owner.resistenceStatsDict.ContainsKey(resistanceType))
		{
			owner.resistenceStatsDict[resistanceType] -= resistanceCoefficient;
		}
		else
		{
			owner.resistenceStatsDict[resistanceType] = 1 - resistanceCoefficient;
		}

		base.Equip(player);
	}

	public override void Unequip()
	{

		if (Player.Instance != null && originalArmorSprite != null)
		{
			Player.Instance.ArmorIcon.sprite = originalArmorSprite;
		}

		if (owner != null)
		{
			owner.resistenceStatsDict[resistanceType] += resistanceCoefficient;
		}

		base.Unequip();
	}

	public void OnTileMatched(Tile tile, int count)
	{
		if (tile.elementType != ElementType.Earth || owner == null || owner.IsDying())
		{
			Inventory.Instance.NotifyTileItemFinished();
			return;
		}

		int shieldAmount = bonusArmorPerShieldMatch * count;

		if (shieldAmount > 0)
		{
			owner.AddArmor(shieldAmount);
			owner.StatusEffectManager.RemoveNegativeStatusEffect();

			ActivateAnimation();
			//Debug.Log($"Earth Armor added: +{shieldAmount} Armor, because Earth count is {count} multiplied by {bonusArmorPerShieldMatch}");
		}

		Inventory.Instance.NotifyTileItemFinished();
	}
}
