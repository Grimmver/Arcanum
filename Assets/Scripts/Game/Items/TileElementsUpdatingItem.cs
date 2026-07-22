using System.Collections.Generic;
using UnityEngine;

public class TileElementsUpdatingItem : Item
{
	[Header("Elements Updating Settings")]
	[SerializeField] private List<WeightedTilePrefab> tilesToSubtract;
	[SerializeField] private List<WeightedTilePrefab> tilesToAdd;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		foreach (WeightedTilePrefab weightedTilePrefab in tilesToSubtract)
		{
			MatchManager.Instance.RemoveElement(weightedTilePrefab.tilePrefab, weightedTilePrefab.tileWeight);
		}
		foreach (WeightedTilePrefab weightedTilePrefab in tilesToAdd)
		{
			MatchManager.Instance.AddElement(weightedTilePrefab.tilePrefab, weightedTilePrefab.tileWeight);
		}
		MatchManager.Instance.RefreshTiles();
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			foreach (WeightedTilePrefab weightedTilePrefab in tilesToAdd)
			{
				MatchManager.Instance.RemoveElement(weightedTilePrefab.tilePrefab, weightedTilePrefab.tileWeight);
			}
			foreach (WeightedTilePrefab weightedTilePrefab in tilesToSubtract)
			{
				MatchManager.Instance.AddElement(weightedTilePrefab.tilePrefab, weightedTilePrefab.tileWeight);
			}
		}
		MatchManager.Instance.RefreshTiles();

		owner = null;
	}
}
