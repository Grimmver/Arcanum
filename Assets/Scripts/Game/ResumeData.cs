using UnityEngine;
using System;
using System.Collections.Generic;

/// Serializable class � Unity can turn it into / from JSON quickly.
[Serializable]
public class ResumeData
{
    public int wave;
    public int wavestrength;
    public int playerHealth;
    public int playerArmor;
    public int crystal;
    public int tomeofknowledgelevel;
    public int heartamuletbonus;
    public int metalbonushealth;
    public int metaltakenarmor;
    public int plasticarmorstack;
    public int maxHealthRemoved;
    public bool Restored = false;
    public List<string> inventoryItemNames = new();   // ItemName strings
}
