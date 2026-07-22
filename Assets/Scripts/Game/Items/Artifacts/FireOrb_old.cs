// using UnityEngine;

// public class FireOrb : Item, IOnTileMatchItem
// {
// 	[Header("Stat Bonus")]
// 	public int bonusMaxHealth = 20;

// 	[Header("Passive Trigger")]
// 	[SerializeField] private int requiredFireGemCount = 3;
// 	[SerializeField] private float fireballDamage = 30f;
// 	[SerializeField] private Vector2 fireballSpawnOffset = new Vector2(0, 0);
// 	[SerializeField] private GameObject fireballPrefab;

// 	private int fireGemMatchCounter = 0;

// 	public override void Equip(Player player)
// 	{
// 		owner = player;
// 		if (owner == null) return;

// 		owner.MaxHealth += bonusMaxHealth;
// 		owner.Health += bonusMaxHealth;
// 	}

// 	public override void Unequip()
// 	{
// 		if (owner != null)
// 		{
// 			owner.MaxHealth -= bonusMaxHealth;
// 		}

// 		owner = null;
// 		fireGemMatchCounter = 0;
// 	}

// 	public void OnTileMatched(Tile tile, int count = 1)
// 	{
// 		if (tile.elementType != ElementType.Fire)
// 		{
// 			Inventory.Instance.NotifyTileItemFinished();
// 			return;
// 		}

// 		fireGemMatchCounter += count;
// 		if (fireGemMatchCounter >= requiredFireGemCount)
// 		{
// 			OnFireGemsMatch(fireGemMatchCounter);
// 			fireGemMatchCounter = 0;
// 		}
// 	}

// 	private void OnFireGemsMatch(int count)
// 	{
// 		Enemy enemy = FindAnyObjectByType<Enemy>();
// 		if (owner == null || enemy == null || enemy.IsDying())
// 		{
// 			Inventory.Instance.NotifyTileItemFinished();
// 			return;
// 		}

// 		Vector3 spawnPos = owner.transform.position + (Vector3)fireballSpawnOffset;
// 		Vector2 direction = (enemy.transform.position - spawnPos).normalized;

// 		SpawnProjectile(fireballPrefab, spawnPos, direction, fireballDamage);
// 	}

// 	private void SpawnProjectile(GameObject prefab, Vector3 spawnPos, Vector2 direction, float damage)
// 	{
// 		GameObject fireball = Instantiate(prefab, spawnPos, Quaternion.identity);
// 		if (fireball.TryGetComponent(out SelectiveTargetProjectile fb))
// 		{
// 			fb.Init(damage, direction);
// 			fb.onDestroyed = () =>
// 			{
// 				Inventory.Instance.NotifyTileItemFinished();
// 			};
// 			AudioManager.Instance?.PlaySound("Fireball");
// 		}
// 		else
// 		{
// 			Destroy(fireball);
// 			Inventory.Instance.NotifyTileItemFinished();
// 		}
// 	}
// }
