using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CastTileMatchProjectileTileMatchEffect : TileMatchEffect
{

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Vector2 projectileSpawnOffset = new Vector2(0, 0);
    
    public override void ExecuteEffect(int amount, Enemy enemy, Action onComplete)
    {

        if (enemy == null)
        {
            enemy = BattleManager.instance.GetEnemy();
        }
        
        if (enemy == null || enemy.IsDead() || enemy.IsDying())
        {
            onComplete?.Invoke();
            return;
        }
        Player.Instance?.animator.SetTrigger("Attack");
        Vector3 spawnPos = Player.Instance.transform.position + (Vector3)projectileSpawnOffset;
        Vector2 direction = (enemy.transform.position - spawnPos).normalized;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        if (projectile.TryGetComponent(out TileMatchProjectile proj))
        {
            proj.Init(-1, direction);
            proj.tileAmount = amount;
            proj.onDestroyed = onComplete;
        }

        return;
    }
}
