using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class MoneyUI : MonoBehaviour
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected TMP_Text amountText;

        protected abstract string PrefKey { get; }

        protected virtual void Start()
        {
            PlayerPrefsEventBus.OnIntChanged += OnValueChanged;
            UpdateUI();
        }

        protected virtual void OnDestroy()
        {
            PlayerPrefsEventBus.OnIntChanged -= OnValueChanged;
        }

        private void OnValueChanged(string key, int value)
        {
            if (key != PrefKey) return;
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            int value = PlayerPrefsEventBus.GetInt(PrefKey);
            amountText.text = value.ToString();
            Animate();
        }

        protected virtual void Animate()
        {
            // Optional animation override
        }
    }
}
