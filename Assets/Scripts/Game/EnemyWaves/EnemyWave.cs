using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : ScriptableObject
{
	[SerializeField]
	[Tooltip("If this is <= 0 then it returns total strength of enemies")]
	protected int strength;
	public int Strength => strength > 0 ? strength : GetTotalEnemyStrength();

	[SerializeField] protected List<Enemy> enemies;
	public List<Enemy> Enemies => enemies;

	private int GetTotalEnemyStrength()
	{
		int result = 0;
		foreach (Enemy enemy in enemies) result += enemy.Strength;
		return result;
	}
}
