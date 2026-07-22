using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpriteDamageTypePair
{
	public Sprite sprite;
	public DamageType damageType;
}

public class ChaosCrystal : Item, IOnEnemyPreTakeDamageItem, IOnWaveSpawnItem, IOnPlayerStartTurnItem
{
	[Header("Passive Trigger")]
	[SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private List <SpriteDamageTypePair> spriteDamageTypePairs = new List <SpriteDamageTypePair>();
	[SerializeField] private int currentTypeId;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
	{
		if (owner == null || owner.IsDying() || owner.IsDead() || damage.amount <= 0 || damage.statusEffectName != "") return;

        if (spriteDamageTypePairs == null || spriteDamageTypePairs.Count == 0)
        {
            return;
        }

        if (damage.type != spriteDamageTypePairs[currentTypeId].damageType)
        {
            return;
        }

		damage.amount = (int)(damage.amount * damageMultiplier);

		ActivateAnimation();
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		RerollEffect();
	}

    public void OnPlayerStartTurn()
    {
        RerollEffect();
    }

    public void RerollEffect()
    {

        if (spriteDamageTypePairs == null || spriteDamageTypePairs.Count == 0)
        {
            return;
        }

        currentTypeId = Random.Range(0, spriteDamageTypePairs.Count);

        Sprite = spriteDamageTypePairs[currentTypeId].sprite;

        // Debug.Log($"Set ChaosCrystal type ID to {currentTypeId}.");
    }
}
