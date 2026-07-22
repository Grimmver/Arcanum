using UnityEngine;

public class FireOrb : TileElementsUpdatingItem
{

	[SerializeField] public GameObject tilePrefab;
	[SerializeField] public GameObject projectilePrefab;
	[SerializeField] private Vector2 addedOffset = new Vector2(0, 0);
	public GameObject oldProjectilePrefab;

	[SerializeField] public string effectName;
	[SerializeField] public string decoratorPrefabName;
	[SerializeField] public string relativePrefabName;

	public override void Equip(Player player)
	{
		base.Equip(player);
		if (owner == null) return;

		TileMatchEffectHolder tileMatchEffectHolder = MatchManager.Instance.tileToEffectHolderDict[tilePrefab];

		CastTileMatchProjectileTileMatchEffect effect = tileMatchEffectHolder.GetComponent<CastTileMatchProjectileTileMatchEffect>();
		oldProjectilePrefab = effect.projectilePrefab;
		effect.projectilePrefab = projectilePrefab;
		effect.projectileSpawnOffset += addedOffset;

		RelevantStatusEffectManager.Instance.AddDecoratorAfter(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName), RelevantStatusEffectManager.Instance.GetPrefab(relativePrefabName));
	}

	public override void Unequip()
	{
		if (owner != null)
		{

			TileMatchEffectHolder tileMatchEffectHolder = MatchManager.Instance.tileToEffectHolderDict[tilePrefab];

			CastTileMatchProjectileTileMatchEffect effect = tileMatchEffectHolder.GetComponent<CastTileMatchProjectileTileMatchEffect>();
			effect.projectilePrefab = oldProjectilePrefab;
			effect.projectileSpawnOffset -= addedOffset;

			RelevantStatusEffectManager.Instance.RemoveDecorator(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName));
		}
		base.Unequip();	
	}

}
