using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AddArmorTileMatchEffect : TileMatchEffect
{

    [SerializeField] public float baseArmor = 1f;
    [SerializeField] public float multPerTile = 2f;
    [SerializeField] public int multCap = 2;
    [SerializeField] public int defaultComboSize = 3;

    public int CalculateArmor(int comboSize)
    {

        if (comboSize < defaultComboSize)
        {
            return 0;
        }

        int powers = Math.Min(multCap, comboSize - defaultComboSize), extras = Math.Max(0, comboSize - (defaultComboSize + multCap));
        // Debug.Log(comboSize + ": " + powers + " " + extras + " -> " + (baseArmor * (Mathf.Pow(multPerTile, powers) + extras)));
        return (int)(baseArmor * (Mathf.Pow(multPerTile, powers) + extras));
        // return (int)((comboSize - 1) * comboArmor);
    }

    public override void ExecuteEffect(int amount, Enemy enemy, Action onComplete)
    {
        Player.Instance.AddArmor(CalculateArmor(amount));

        onComplete?.Invoke();

        return;
    }
}
