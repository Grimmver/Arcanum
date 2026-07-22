using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public abstract class TileMatchEffect : MonoBehaviour
{
    public abstract void ExecuteEffect(int amount, Enemy enemy, Action onComplete);
}
