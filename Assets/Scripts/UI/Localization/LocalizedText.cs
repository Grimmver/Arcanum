using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class LocalizedText : MonoBehaviour
	{
		[SerializeField] private List<LocalizedTextEntry> localizedEntries = new();

		private TMP_Text text;

		private void Awake()
		{
			text = GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			LocalizationEventBus.Subscribe(OnLanguageChanged);
			OnLanguageChanged(LocalizationEventBus.CurrentLanguage);
		}

		private void OnDisable()
		{
			LocalizationEventBus.Unsubscribe(OnLanguageChanged);
		}

		private void OnLanguageChanged(string language)
		{
			foreach (LocalizedTextEntry entry in localizedEntries)
			{
				if (entry.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
				{
					text.text = entry.Text;
					return;
				}
			}

			LocalizedTextEntry fallback = localizedEntries.FirstOrDefault();
			if (fallback != null)
			{
				text.text = fallback.Text;
			}
		}
	}
}
