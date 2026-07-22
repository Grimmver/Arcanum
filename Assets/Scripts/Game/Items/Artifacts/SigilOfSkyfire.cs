using UnityEngine;

public class SigilOfSkyfire : TileElementsUpdatingItem
{

	[SerializeField] public string effectName;
	[SerializeField] public string decoratorPrefabName;
	[SerializeField] public string relativePrefabName;

	public override void Equip(Player player)
	{

		base.Equip(player);
		
		if (owner == null)
        {
            return;
        }

		RelevantStatusEffectManager.Instance.AddDecoratorAfter(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName), RelevantStatusEffectManager.Instance.GetPrefab(relativePrefabName));
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			RelevantStatusEffectManager.Instance.RemoveDecorator(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName));
		}
		
		base.Unequip();
	}

}
