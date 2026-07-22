using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectDecorator : StatusEffect
{
    [SerializeField] public StatusEffect _baseEffect;

    public void SetBaseEffect(StatusEffect baseEffect)
    {
        _baseEffect = baseEffect;
    }

    public override void Init(Character owner, int stack, Character sender)
    {
        _baseEffect.Init(owner, stack, sender);
        transform.SetParent(_baseEffect.transform);
    }

    public override void ApplyAdditionalStack(int amount, Character sender)
    {
        _baseEffect.ApplyAdditionalStack(amount, sender);
    }

    public override int GetCurrentStack()
    {
        return _baseEffect.GetCurrentStack();
    }

    public override Character GetOldestStack()
    {
        return _baseEffect.GetOldestStack();
    }

    public override void DecrementStack(int amount=1)
    {
        _baseEffect.DecrementStack(amount);
    }

    public override void ProcessStatusEffect(Action onComplete)
    {
        _baseEffect.ProcessStatusEffect(onComplete);
    }

    public override float GetIncomingDamageModifier()
    {
        return _baseEffect.GetIncomingDamageModifier();
    }

    public override float GetIncomingTypedDamageModifier(DamageType damageType)
    {
        return _baseEffect.GetIncomingTypedDamageModifier(damageType);
    }

    public override float GetOutgoingDamageModifier()
    {
        return _baseEffect.GetOutgoingDamageModifier();
    }

    public override bool IsStunning()
    {
        return _baseEffect.IsStunning();
    }

    public override bool IsTricking()
    {
        return _baseEffect.IsTricking();
    }

    public override Image GetIcon()
    {
        return _baseEffect.GetIcon();
    }

    public override StatusEffectDisplaySlot GetSlot()
    {
        return _baseEffect.GetSlot();
    }

    public override Sprite GetSprite()
    {
        return _baseEffect.GetSprite();
    }

    public override Sprite GetFrameSprite()
    {
        return _baseEffect.GetFrameSprite();
    }

    public override Color GetFrameColor()
    {
        return _baseEffect.GetFrameColor();
    }

    public override void SetIcon(Image icon)
    {
        _baseEffect.SetIcon(icon);
    }

    public override void SetSlot(StatusEffectDisplaySlot slot)
    {
        _baseEffect.SetSlot(slot);
    }

    public override Character GetOwner()
    {
        return _baseEffect.GetOwner();
    }

    private void OnDestroy()
    {
        if (_baseEffect != null)
        {
            Destroy(_baseEffect.gameObject);
        }
    }

    public override string GetBaseEffectType()
    {
        return _baseEffect.GetBaseEffectType();
    }

    public override void ParseDamage(Damage damage)
    {
        _baseEffect.ParseDamage(damage);
    }

    public override bool IsNegative()
    {
        return _baseEffect.IsNegative();
    }

    public override List <Character> GetStackList()
    {
        return _baseEffect.GetStackList();
    }
    
}
