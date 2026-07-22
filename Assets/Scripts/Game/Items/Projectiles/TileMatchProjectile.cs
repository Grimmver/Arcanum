using UnityEngine;
using System;
using System.Collections.Generic;

public class TileMatchProjectile : Projectile
{
    
    [SerializeField] public List <TileMatchEffect> tileMatchEffects = new List <TileMatchEffect>();
    public int tileAmount = 0;  

    protected void Awake()
    {
        tileMatchEffects = new List<TileMatchEffect>(GetComponents<TileMatchEffect>());
    }

    protected override void HandleHit(Collider2D collision)
    {
        
        Component targetComp = collision.GetComponent(typeof(Enemy));
        if (targetComp != null)
        {
            if (targetComp is Enemy enemy)
            {
                if (tileMatchEffects == null || tileMatchEffects.Count == 0)
                {
                    Explode();
                    return;
                }

                int effectId = 0;

                void ExecuteNextEffect()
                {
                    if (effectId >= tileMatchEffects.Count)
                    {
                        Explode();
                        return;
                    }

                    tileMatchEffects[effectId].ExecuteEffect(tileAmount, enemy, () =>
                    {
                        effectId++;
                        ExecuteNextEffect();
                    });
                }

                ExecuteNextEffect();
            }
        }
    }
}
