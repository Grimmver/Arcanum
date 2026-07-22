using UnityEngine;
using System;

public class IceArmor : TileElementsUpdatingItem, IOnTileMatchItem
{
	[Header("Passive Trigger")]
	[SerializeField] public string appliedStatusEffect;
    [SerializeField] public float baseStack = 1f;
    [SerializeField] public float multPerTile = 2f;
    [SerializeField] public int multCap = 2;
    [SerializeField] public int defaultComboSize = 3;

	public override void Equip(Player player)
	{
		base.Equip(player);
	}

	public override void Unequip()
	{
		base.Unequip();
	}

	public void OnTileMatched(Tile tile, int count)
	{
		if (owner == null || owner.IsDying() || tile.elementType != ElementType.Ice || appliedStatusEffect == null)
		{
			Inventory.Instance.NotifyTileItemFinished();
			return;
		}

		owner.ApplyStatusEffect(appliedStatusEffect, CalculateStack(count), Player.Instance);

		Inventory.Instance.NotifyTileItemFinished();
	}

    public int CalculateStack(int comboSize)
    {

        if (comboSize < defaultComboSize)
        {
            return 0;
        }

        int powers = Math.Min(multCap, comboSize - defaultComboSize), extras = Math.Max(0, comboSize - (defaultComboSize + multCap));
        return (int)(baseStack * (Mathf.Pow(multPerTile, powers) + extras));
    }

}
