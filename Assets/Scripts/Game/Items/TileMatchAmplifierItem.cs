using UnityEngine;

public class TileMatchAmplifierItem : Item, IOnTileMatchAmplifierItem
{
	[Header("Tile Match Amplifier")]
	[SerializeField] private int matchBonus = 1;
	[SerializeField] private ElementType targetType = ElementType.Lighting;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		TileEventBus.OnPreTileMatchedModify += ModifyMatchCount;
	}

	public override void Unequip()
	{
		TileEventBus.OnPreTileMatchedModify -= ModifyMatchCount;
	}

	private int ModifyMatchCount(Tile tile, int baseCount) => baseCount + GetMatchBonus(tile);

	public virtual int GetMatchBonus(Tile tile)
	{
		if (tile == null || tile.elementType != targetType || matchBonus <= 0) return 0;

		// Debug.Log(tile);

		ActivateAnimation();

		return matchBonus;
	}
}
