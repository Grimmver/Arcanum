using UnityEngine;
using System.Collections.Generic;

public static class QuickResume
{
    private const string KEY = "RESUME_DATA_JSON";

    /* -------- Save current run into PlayerPrefs ---------- */
    public static void WriteCurrentState()
    {
        var data = new ResumeData
        {
            wave = EnemyWaveManager.Instance.TurnCounter -1,
            playerHealth = EnemyWaveManager.Instance.PlayerHealth,
            playerArmor = EnemyWaveManager.Instance.PlayerArmor,
            crystal = EnemyWaveManager.Instance.crystal,
            wavestrength = EnemyWaveManager.Instance.lastUsedStrength,
            tomeofknowledgelevel = 0,
            heartamuletbonus = 0,
            metalbonushealth = 0,
            metaltakenarmor = 0,
            plasticarmorstack = 3,
            maxHealthRemoved = 0,
            Restored = true
        };

        foreach (ItemSlot slot in Inventory.Instance.ItemSlots)
            if (slot.EquippedItem != null) 
            { 
                data.inventoryItemNames.Add(slot.EquippedItem.ItemName);
                if (slot.EquippedItem is HeartShapedAmulet amulet)
                    data.heartamuletbonus = amulet.currentBonusHealth;
                if (slot.EquippedItem is TomeOfKnowledge tome)
                    data.heartamuletbonus = tome.comboCounter;
                if (slot.EquippedItem is LivingMetal metal)
                {
                    data.metalbonushealth = metal.totalHealthBuff;
                    data.metaltakenarmor = metal.totalArmorLoss;
                }
                if (slot.EquippedItem is PlasticArmor plasticarmor)
                {
                    data.plasticarmorstack = plasticarmor.currentDamageReduction;
                }
                if (slot.EquippedItem is DarkRing darkRing)
                {
                    data.maxHealthRemoved = darkRing.maxHealthRemoved;
                }
                //slot.EquippedItem.Unequip();
                //slot.RemoveItem();
            }

        // 3. Serialize & store
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();                    // make sure it’s on disk
    }

    /* -------- Try to read & consume the save ---------- */
    public static bool TryRestore()
    {
        if (!PlayerPrefs.HasKey(KEY)) return false;

        var json = PlayerPrefs.GetString(KEY);
        var data = JsonUtility.FromJson<ResumeData>(json);

        // 1. Restore primitive fields
        EnemyWaveManager.Instance.TurnCounter = data.wave;
        //EnemyWaveManager.Instance.PlayerHealth = data.playerHealth;
        EnemyWaveManager.Instance.PlayerArmor = data.playerArmor;
        EnemyWaveManager.Instance.crystal = data.crystal;
        EnemyWaveManager.Instance.lastUsedStrength = data.wavestrength-1; 
        PlayerPrefsEventBus.SetInt("Crystal", data.crystal);
        Debug.Log("Crystalls NoW are set to " + PlayerPrefsEventBus.GetInt("Crystal", 0));

        // 2. Re-fill inventory
        Inventory inv = Inventory.Instance;
        inv.CancelAllDraggingItems();          // tidy empty slots first

        foreach (string itemName in data.inventoryItemNames)
        {
            // Your shop already has prefab references – pick one that matches
            Item prefab = Shop.Instance.shopItemPrefabs.Find(p => p.ItemName == itemName);
            if (prefab == null) continue;           
            Item newItem = Object.Instantiate(prefab);
            if (newItem is HeartShapedAmulet newamulet)
                newamulet.currentBonusHealth = data.heartamuletbonus;
            if (newItem is TomeOfKnowledge newtome)
                newtome.comboCounter = data.heartamuletbonus;
            if (newItem is LivingMetal newmetal)
            {
                newmetal.totalHealthBuff = data.metalbonushealth;
                newmetal.totalArmorLoss = data.metaltakenarmor;
            }
            if (newItem is PlasticArmor newplasticarmor)
            {
                newplasticarmor.currentDamageReduction = data.plasticarmorstack;
            }
            if (newItem is DarkRing darkRing)
            {
                darkRing.maxHealthRemoved = data.maxHealthRemoved;
            }
            inv.TryAddItem(newItem);
            newItem.transform.localScale = prefab.transform.localScale;
            newItem.transform.localPosition = prefab.transform.localPosition;
            Shop.Instance.UpdateItemSoldList(newItem); // update sold items list in shop
        }
        //Player.Instance.MaxHealth = data.playerHealth; 
        Player.Instance.Armor = data.playerArmor;
        if (data.Restored)
        {
            MatchManager.Instance.IncreaseLuck(MatchManager.Instance.tempLuckIncrease); // restore health if it was saved
            MatchManager.Instance.ResumeLuck = true;
            MatchManager.Instance.tempLuckDuration = MatchManager.Instance.tempLuckIncrease/ MatchManager.Instance.tempLuckDecreaseRate + 1;
        }
        // 3. Clear the resume data so the next “fresh start” is really fresh
        PlayerPrefs.DeleteKey(KEY);
        return true;
    }
}
