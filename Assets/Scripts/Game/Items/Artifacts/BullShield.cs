using UnityEngine;

public class BullShield : Item, IOnPlayerEndTurnItem
{
    [Header("Passive Trigger")]
    [SerializeField][Min(0)] private float bonusDamageMultiplication = 0.5f;

    public override void Equip(Player player)
    {
        owner = player;
        if (owner == null) return;
    }

    public override void Unequip()
    {
        owner = null;
    }

    public void OnPlayerEndTurn()
    {

        Enemy enemy = BattleManager.instance.GetEnemy();

        if (enemy == null || enemy.IsDying() || enemy.IsDead()) return;

        int bonusDamage = (int)(owner.Armor * bonusDamageMultiplication);

        if (bonusDamage <= 0) return;

        owner.DealDamage(enemy, new Damage(bonusDamage, DamageType.Physical, owner, true));
        
        ActivateAnimation();
    }
}
