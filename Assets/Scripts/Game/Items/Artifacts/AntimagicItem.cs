using UnityEngine;

public class AntimagicItem : TileElementsUpdatingItem, IOnEnemyPreTakeDamageItem
{

    [Header("Passive Trigger")]
    [SerializeField][Min(0)] private float bonusDamageMultiplication = 1f;

    public override void Equip(Player player)
    {
        base.Equip(player);
        if (owner == null) return;
    }

    public override void Unequip()
    {
        base.Unequip();
    }
    
    public void OnEnemyPreTakeDamage(Enemy enemy, Damage damage)
    {
        if (enemy == null || enemy.IsDying() || enemy.IsDead() || damage.amount <= 0) return;

        if (damage.type != DamageType.Physical) return;

        int bonusDamage = (int)(damage.amount * bonusDamageMultiplication);

        if (bonusDamage <= 0) return;

        damage.amount += bonusDamage;
        //Debug.Log($"Damage was buffed for {bonusDamage}, so now it is {damage.amount}");

        ActivateAnimation();
    }
}
