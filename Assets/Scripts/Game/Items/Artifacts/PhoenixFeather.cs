using UnityEngine;

public class PhoenixFeather : Item, IOnTileMatchItem, IOnEnemyPreTakeDamageItem
{

	[SerializeField] public string effectName;
	[SerializeField] public string decoratorPrefabName;
	[SerializeField] public string relativePrefabName;

    [Header("Passive Trigger")]
    [SerializeField] private int damagePerCombo = 10;
    [SerializeField] private DamageType damageType = DamageType.Fire;
    [SerializeField] ElementType elementType = ElementType.Fire;
    [SerializeField] private float bonusDamageMultiplication = 1.0f;

	public override void Equip(Player player)
	{
		
        owner = player;
		if (owner == null)
        {
            return;
        }

		RelevantStatusEffectManager.Instance.AddDecoratorAfter(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName), RelevantStatusEffectManager.Instance.GetPrefab(relativePrefabName));
	}

	public override void Unequip()
	{
		if (owner != null)
		{
			RelevantStatusEffectManager.Instance.RemoveDecorator(effectName, RelevantStatusEffectManager.Instance.GetPrefab(decoratorPrefabName));
		}
        owner = null;
	}

    public void OnTileMatched(Tile tile, int count)
	{

        if (tile.elementType != elementType || owner == null || owner.IsDying() || owner.IsDead())
		{
			Inventory.Instance.NotifyTileItemFinished();
			return;
		}

		owner.TakePureDamage(new Damage(damagePerCombo, damageType, owner, "Fire", true));

		Inventory.Instance.NotifyTileItemFinished();

		ActivateAnimation();
	}

    public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
    {
        if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0 || damage.type != damageType) return;

        int bonusDamage = (int)(damage.amount * bonusDamageMultiplication);

        if (bonusDamage <= 0) return;

        damage.amount += bonusDamage;
        //Debug.Log($"Damage was buffed for {bonusDamage}, so now it is {damage.amount}");

        ActivateAnimation();
    }

}
