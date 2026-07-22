using Utils;

public interface IOnPlayerPreApplyStatusEffectItem
{
	void OnPlayerPreApplyStatusEffect(string effectName, IntRef stackCount);
}
