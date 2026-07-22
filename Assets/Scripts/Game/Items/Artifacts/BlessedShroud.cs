using UnityEngine;

public class BlessedShroud : Item, IOnPlayerPreTakeDamageItem, IOnWaveSpawnItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int maxAttackCount = 3;
	[SerializeField] private int damageReduction = 8;
	private int currentAttackCount;

    [SerializeField] private Sprite spriteRestored;
    [SerializeField] private Sprite spriteBroken;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		currentAttackCount = 0;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnPlayerPreTakeDamage(Damage damage)
	{
		if (owner == null || owner.IsDying() || owner.IsDead() || damage.amount <= 0) return;

		if (currentAttackCount >= maxAttackCount) return;

        currentAttackCount ++;

        if (currentAttackCount == maxAttackCount)
        {
            Sprite = spriteBroken;
        }

		// Debug.Log($"Original damage was {damage.amount}");
		damage.amount = Mathf.Max(0, damage.amount - damageReduction);
		// Debug.Log($"Now it was reduced by {damageReduction}, so it is {damage.amount}");

		ActivateAnimation();
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null || owner.IsDying() || owner.IsDead()) return;

		if (EnemyWaveManager.Instance == null) return;

		currentAttackCount = 0;
        Sprite = spriteRestored;
	}
}
