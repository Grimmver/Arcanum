using UnityEngine;
using System.Collections.Generic;

public class ChaosWand : Item, IOnWaveSpawnItem, IOnPlayerStartTurnItem
{
	[Header("Passive Trigger")]
	[SerializeField] private int comboSize = 3;
    [SerializeField] List <GameObject> projectilePrefabs = new List <GameObject>();

    public override void Equip(Player player)
	{
		owner = player;
	}

	public override void Unequip()
	{
		owner = null;
	}

	public void OnWaveSpawn(Enemy[] wave)
	{
		CastProjectile();
	}

    public void OnPlayerStartTurn()
	{
        CastProjectile();
	}

    private void CastProjectile()
    {
        if (owner == null || projectilePrefabs == null || projectilePrefabs.Count == 0) return;

        Enemy enemy = BattleManager.instance.GetEnemy();
        
        if (enemy == null)
        {
            return;
        }

        int prefabId = Random.Range(0, projectilePrefabs.Count);
        GameObject projectilePrefab = projectilePrefabs[prefabId];

        if (projectilePrefab == null)
        {
            return;
        }

        Player.Instance?.animator.SetTrigger("Attack");
        Vector3 spawnPos = Player.Instance.transform.position;
        Vector2 direction = (enemy.transform.position - spawnPos).normalized;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        if (projectile.TryGetComponent(out TileMatchProjectile proj))
        {
            proj.Init(-1, direction);
            proj.tileAmount = comboSize;
            // proj.onDestroyed = null;
        }

		ActivateAnimation();
    }

}
