using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class HealTileMatchEffect : TileMatchEffect
{

    [SerializeField] public float baseHeal = 1f;
    [SerializeField] public float multPerTile = 2f;
    [SerializeField] public int multCap = 2;
    [SerializeField] public int defaultComboSize = 3;

    public int CalculateHeal(int comboSize)
    {

        if (comboSize < defaultComboSize)
        {
            return 0;
        }

        int powers = Math.Min(multCap, comboSize - defaultComboSize), extras = Math.Max(0, comboSize - (defaultComboSize + multCap));
        return (int)(baseHeal * (Mathf.Pow(multPerTile, powers) + extras));
    }

    public override void ExecuteEffect(int amount, Enemy enemy, Action onComplete)
    {
        
        Player.Instance.Heal(CalculateHeal(amount));

        onComplete?.Invoke();

        return;
    }
}
