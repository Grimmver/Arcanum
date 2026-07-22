using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatusEffectDisplaySlot : MonoBehaviour
{
    public Image icon;
    public Image frame;
    public TMP_Text stackText;
    private StatusEffect effect;

    private void Awake()
    {

        if (frame == null)
        {
            frame = transform.Find("StatusEffectFrame")?.GetComponent<Image>();
        }
        if (icon == null)
        {
            icon = transform.Find("StatusEffectImage")?.GetComponent<Image>();
        }
        if (stackText == null)
        {
            stackText = transform.Find("StatusEffectText")?.GetComponent<TMP_Text>();
        }
    }

    public void SetEffect(StatusEffect newEffect)
    {

        effect = newEffect;
        effect.SetIcon(icon);
        effect.SetSlot(this);
        UpdateDisplay();

        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        if (this == null || gameObject == null)
        {
            return;
        }
        effect = null;
        gameObject.SetActive(false);
        if (icon != null)
        {
            icon.sprite = null;
        }
        if (stackText != null)
        {
            stackText.text = "";
        }
    }

    public void UpdateDisplay()
    {
        if (effect != null)
        {
            Sprite effectSprite = effect.GetSprite();
            if (effectSprite != null)
            {
                icon.sprite = effectSprite;
            }

            stackText.text = effect.GetCurrentStack().ToString();
            
            // frame.color = effect.GetFrameColor();
            
            Sprite frameSprite = effect.GetFrameSprite();
            if (frameSprite != null)
            {
                frame.sprite = frameSprite;
            }
        }
    }

    public StatusEffect GetEffect() => effect;
}
