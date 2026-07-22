using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class StatusEffect : MonoBehaviour
{

    private Character effectOwner;
    private Queue <Character> stackQueue = new Queue <Character>();
    private Image icon;
    private StatusEffectDisplaySlot slot;

    [Header("Effect Info")]
    public Sprite sprite;
    [SerializeField] private Color frameColor = Color.white;
    [SerializeField] private Sprite frameSprite;
    [SerializeField] String effectType;
    [SerializeField] private bool isNegative = true;

    public virtual void Init(Character owner, int stack, Character sender)
    {
        transform.SetParent(owner.transform);
        effectOwner = owner;
        if (stack <= 0)
        {
            return;
        }
        ApplyAdditionalStack(stack, sender);
    }

    public virtual void ApplyAdditionalStack(int amount, Character sender)
    {
        if (amount <= 0) return;
        for (int stackId = 0; stackId < amount; stackId ++)
        {
            stackQueue.Enqueue(sender);
        }

        if (slot != null)
        {
            slot.UpdateDisplay();
        }
    }

    public virtual int GetCurrentStack()
    {
        return stackQueue.Count;
    }

    public virtual Character GetOldestStack()
    {
        return stackQueue.Count > 0 ? stackQueue.Peek() : null;
    }

    public virtual void DecrementStack(int amount=1)
    {
        while (amount > 0)
        {
            amount --;
            if (GetCurrentStack() == 0)
            {
                break;
            }
            stackQueue.Dequeue();
        }

        if (slot != null)
        {
            slot.UpdateDisplay();
        }
    }

    public virtual void ProcessStatusEffect(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public virtual float GetIncomingDamageModifier()
    {
        return 0f;
    }

    public virtual float GetIncomingTypedDamageModifier(DamageType damageType)
    {
        return 0f;
    }

    public virtual float GetOutgoingDamageModifier()
    {
        return 0f;
    }

    public virtual bool IsStunning()
    {
        return false;
    }

    public virtual bool IsTricking()
    {
        return false;
    }

    public virtual Image GetIcon()
    {
        return icon;
    }

    public virtual StatusEffectDisplaySlot GetSlot()
    {
        return slot;
    }

    public virtual Sprite GetSprite()
    {
        return sprite;
    }

    public virtual Sprite GetFrameSprite()
    {
        return frameSprite;
    }

    public virtual Color GetFrameColor()
    {
        return frameColor;
    }

    public virtual void SetIcon(Image icon)
    {
        this.icon = icon;
    }

    public virtual void SetSlot(StatusEffectDisplaySlot slot)
    {
        this.slot = slot;
    }

    public virtual Character GetOwner()
    {
        return effectOwner;
    }

    public String GetEffectType()
    {
        return effectType;
    }

    public virtual String GetBaseEffectType()
    {
        return effectType;
    }

    public virtual void ParseDamage(Damage damage)
    {
        return;
    }

    public virtual bool IsNegative()
    {
        return isNegative;
    }

    public virtual List <Character> GetStackList()
    {
        return stackQueue.ToList();
    }

}
