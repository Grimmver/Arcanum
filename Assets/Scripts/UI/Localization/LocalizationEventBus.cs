using System;
using System.Collections.Generic;

namespace UI
{
	public static class LocalizationEventBus
	{
		public const string LANGUAGE_PREF = "Language";
		public const string DEFAULT_LANGUAGE = ENGLISH;
		public const string ENGLISH = "EN";
		public const string RUSSIAN = "RU";

		public static event Action<string> OnLanguageChanged;

		private static readonly List<Action<string>> listeners = new();

		public static string CurrentLanguage => PlayerPrefsEventBus.GetString(LANGUAGE_PREF, DEFAULT_LANGUAGE);

		public static void ChangeLanguage(string newLanguage)
		{
			if (string.IsNullOrEmpty(newLanguage)) return;

			PlayerPrefsEventBus.SetString(LANGUAGE_PREF, newLanguage);

			OnLanguageChanged?.Invoke(newLanguage);

			//Debug.Log($"[Localization] Language changed to: {newLanguage}");
		}

		public static void Subscribe(Action<string> callback)
		{
			if (!listeners.Contains(callback))
			{
				listeners.Add(callback);
				OnLanguageChanged += callback;
			}
		}

		public static void Unsubscribe(Action<string> callback)
		{
			if (listeners.Contains(callback))
			{
				listeners.Remove(callback);
				OnLanguageChanged -= callback;
			}
		}
	}
}
