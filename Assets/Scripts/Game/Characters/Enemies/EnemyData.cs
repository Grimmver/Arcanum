using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemy", fileName = "Enemy")]
public class EnemyData : ScriptableObject
{
	[Header("Spawn Settings")]
	[SerializeField][Min(0)] protected int strength = 1;
	public int Strength => strength;

	[SerializeField][Min(0)] protected float multiplierPerLeftStrength = 0.1f;
	public float MultiplierPerLeftStrength => multiplierPerLeftStrength;

	[SerializeField][Range(0, 100f)][Tooltip("Percentage of spawning if the strength is high enough")] protected float spawnChance = 10f;
	public float SpawnChance => spawnChance;

	[Header("Stats Settings")]
	public string enemyName;
	public string Name => enemyName;

	[SerializeField] protected int health = 100;
	public int Health => health;

	[SerializeField] protected int maxHealth = 100;
	public int MaxHealth => maxHealth;

	[SerializeField] protected int armor = 0;
	public int Armor => armor;

	[SerializeField] protected int minAttackDamage = 5;
	public int MinAttackDamage => minAttackDamage;

	[SerializeField] protected int maxAttackDamage = 10;
	public int MaxAttackDamage => maxAttackDamage;

	[SerializeField] protected Sprite sprite;
	public Sprite Sprite => sprite;

	[SerializeField] protected string attackSound;
	public string AttackSound => attackSound;
}
