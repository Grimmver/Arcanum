using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AddStatusEffectTileMatchEffect : TileMatchEffect
{

    [SerializeField] public string appliedStatusEffect;

    [SerializeField] public float baseStack = 1f;
    [SerializeField] public float multPerTile = 2f;
    [SerializeField] public int multCap = 2;
    [SerializeField] public int defaultComboSize = 3;

    public int CalculateStack(int comboSize)
    {

        if (comboSize < defaultComboSize)
        {
            return 0;
        }

        int powers = Math.Min(multCap, comboSize - defaultComboSize), extras = Math.Max(0, comboSize - (defaultComboSize + multCap));
        return (int)(baseStack * (Mathf.Pow(multPerTile, powers) + extras));
    }

    public override void ExecuteEffect(int amount, Enemy enemy, Action onComplete)
    {

        if (appliedStatusEffect == null)
        {
            return;
        }

        Player.Instance.ApplyStatusEffect(appliedStatusEffect, CalculateStack(amount), Player.Instance);

        onComplete?.Invoke();

        return;
    }
}
