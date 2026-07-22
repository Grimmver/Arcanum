using UnityEngine;

public class MageArtifact : Item, IOnTileMatchItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int bonusFireDamage = 20;
	[SerializeField] private int bonusIceDamage = 10;
	[SerializeField] private int bonusLightningDamage = 15;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnTileMatched(Tile tile, int count)
	{
		if (Player.Instance == null || count <= 0) return;

		bool activated = false;

		if (tile.elementType == ElementType.Fire)
		{
			EnemyWaveManager.Instance?.CurrentWaveTakeDamage(new Damage(bonusIceDamage, DamageType.Ice, owner, true));
			activated = true;
		}

		if (tile.elementType == ElementType.Ice)
		{
			EnemyWaveManager.Instance?.CurrentWaveTakeDamage(new Damage(bonusLightningDamage, DamageType.Lighting, owner, true));
			activated = true;
		}

		if (tile.elementType == ElementType.Lighting)
		{
			EnemyWaveManager.Instance?.CurrentWaveTakeDamage(new Damage(bonusFireDamage, DamageType.Fire, owner, true));
			activated = true;
		}

		if (activated)
		{
			ActivateAnimation();
		}

		Inventory.Instance.NotifyTileItemFinished();
	}
}
