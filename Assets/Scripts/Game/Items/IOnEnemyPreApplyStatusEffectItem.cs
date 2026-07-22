using Utils;

public interface IOnEnemyPreApplyStatusEffectItem
{
	void OnEnemyPreApplyStatusEffect(Enemy enemy, string effectName, IntRef stackCount);
}
