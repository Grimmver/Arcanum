using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	[Serializable]
	public class LocalizedString
	{
		public string Language;
		[TextArea(3, 7)] public string Text;

		public static string CreateLocalized(List<LocalizedString> list)
		{
			string currentLang = LocalizationEventBus.CurrentLanguage;

			foreach (LocalizedString localized in list)
			{
				if (localized.Language.Equals(currentLang, StringComparison.OrdinalIgnoreCase))
				{
					return localized.Text;
				}
			}

			foreach (LocalizedString localized in list)
			{
				if (localized.Language.Equals(LocalizationEventBus.DEFAULT_LANGUAGE, StringComparison.OrdinalIgnoreCase))
				{
					return localized.Text;
				}
			}

			return list.Count > 0 ? list[0].Text : string.Empty;
		}
	}
}
