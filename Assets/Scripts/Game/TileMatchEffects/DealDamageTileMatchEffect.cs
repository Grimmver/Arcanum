using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class DealDamageTileMatchEffect : TileMatchEffect
{

    [SerializeField] public DamageType damageType;

    [SerializeField] public float baseDamage = 1f;
    [SerializeField] public float multPerTile = 2f;
    [SerializeField] public int multCap = 2;
    [SerializeField] public int defaultComboSize = 3;

    public int CalculateDamage(int comboSize)
    {

        if (comboSize < defaultComboSize)
        {
            return 0;
        }

        int powers = Math.Min(multCap, comboSize - defaultComboSize), extras = Math.Max(0, comboSize - (defaultComboSize + multCap));
        // Debug.Log(comboSize + ": " + powers + " " + extras + " -> " + (baseDamage * (Mathf.Pow(multPerTile, powers) + extras)));
        return (int)(baseDamage * (Mathf.Pow(multPerTile, powers) + extras));
    }

    public override void ExecuteEffect(int amount, Enemy enemy, Action onComplete)
    {

        if (enemy == null)
        {
            onComplete?.Invoke();
            return;
        }

        Player.Instance.DealDamage(enemy, new Damage(CalculateDamage(amount), damageType, Player.Instance));

        onComplete?.Invoke();

        return;
    }
}
