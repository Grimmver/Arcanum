using UnityEngine;
using System;

public class SelectiveTargetProjectile : Projectile
{
    [SerializeField] private string targetComponentTypeName;

    public Type targetType;

    protected void Start()
    {
        if (!string.IsNullOrEmpty(targetComponentTypeName))
        {
            targetType = Type.GetType(targetComponentTypeName);
            if (targetType == null)
            {
                Debug.LogWarning($"SelectiveTargetProjectile: Could not find type '{targetComponentTypeName}'. Check spelling and namespace.");
            }
        }
        else
        {
            Debug.LogWarning("SelectiveTargetProjectile: Target component type name is empty.");
        }
    }

    protected override void HandleHit(Collider2D collision)
    {
        if (targetType == null)
        {
            return;
        }

        Component targetComp = collision.GetComponent(targetType);
        if (targetComp != null)
        {
            if (targetComp is Character character)
            {
                character.TakeDamage(new Damage((int)damage, DamageType.Fire, Player.Instance));
                Explode();
            }
            else
            {
                Debug.LogWarning($"SelectiveTargetProjectile: Target component '{targetType.Name}' does not implement IDamageable.");
            }
        }
    }
}
