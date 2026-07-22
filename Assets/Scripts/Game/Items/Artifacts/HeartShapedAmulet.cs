using UnityEngine;

public class HeartShapedAmulet : Item, IOnWaveSpawnItem
{
	[Header("Stats")]
	[SerializeField] private int bonusHealth = 50;

	[Header("Passive Trigger")]
	[SerializeField][Min(1)] private int linearHealthScaling = 1;
	[SerializeField][Min(1)] private int battlesToScale = 1;
    [HideInInspector] public int currentBonusHealth;

	public override void Equip(Player player)
	{
		owner = player;
		if (owner == null) return;

		owner.MaxHealth += bonusHealth;
		owner.Health += bonusHealth;
        
    }

	public override void Unequip()
	{
		if (owner == null) return;

		owner.MaxHealth -= bonusHealth + currentBonusHealth;

		owner = null;
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		if (owner == null || owner.IsDying() || owner.IsDead()) return;

		if (EnemyWaveManager.Instance == null) return;

		int battleNumber = EnemyWaveManager.Instance.TurnCounter;

		if (battleNumber > 0 && battleNumber % battlesToScale == 0)
		{
			currentBonusHealth += linearHealthScaling;
			owner.MaxHealth += linearHealthScaling;
			owner.Health += linearHealthScaling;
			//Debug.Log($"Health was scaled, now it is {currentBonusHealth}, linearly scaling for {linearHealthScaling} in each {battlesToScale} battles, now it is battle number {battleNumber}");

			ActivateAnimation();
		}
	}
}
