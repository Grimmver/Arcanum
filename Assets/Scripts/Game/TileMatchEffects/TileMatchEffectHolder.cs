using UnityEngine;
using System.Collections.Generic;

public class TileMatchEffectHolder : MonoBehaviour
{

    public List <TileMatchEffect> tileMatchEffects;

    public void PrecalcList()
    {
        tileMatchEffects = new List<TileMatchEffect>(GetComponents<TileMatchEffect>());
    }

}
