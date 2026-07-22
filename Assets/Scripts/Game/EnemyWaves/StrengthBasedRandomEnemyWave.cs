using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Strength Based Random Enemy Wave", fileName = "RandomWave")]
public class StrengthBasedRandomEnemyWave : EnemyWave
{
	[SerializeField] private int requiredStrength = 10;
	public int RequiredStrength => requiredStrength;

	public List<Enemy> GetRandomizedEnemies()
	{
		List<Enemy> result = new List<Enemy>();
		List<Enemy> shuffled = new List<Enemy>(Enemies);
		int total = 0;

		for (int i = 0; i < shuffled.Count; i++)
		{
			int rnd = Random.Range(i, shuffled.Count);
			(shuffled[i], shuffled[rnd]) = (shuffled[rnd], shuffled[i]);
		}

		foreach (var enemy in shuffled)
		{
			result.Add(enemy);
			total += enemy.Strength;
			if (total >= requiredStrength) break;
		}

		return result;
	}
}
