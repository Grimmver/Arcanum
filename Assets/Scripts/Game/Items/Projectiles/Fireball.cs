using UnityEngine;

public class Fireball : Projectile
{

    protected override void HandleHit(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(new Damage((int)damage, DamageType.Fire, Player.Instance));
            Explode();
        }
    }
}
