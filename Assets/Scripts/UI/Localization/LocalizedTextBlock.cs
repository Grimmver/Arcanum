using System;
using System.Collections.Generic;

namespace UI
{
	[Serializable]
	public class LocalizedTextBlock
	{
		public List<LocalizedTextEntry> languages = new();

		public string Get()
		{
			string currentLang = LocalizationEventBus.CurrentLanguage;

			foreach (LocalizedTextEntry entry in languages)
			{
				if (entry.Language.Equals(currentLang, StringComparison.OrdinalIgnoreCase))
					return entry.Text;
			}

			return languages.Count > 0 ? languages[0].Text : string.Empty;
		}
	}
}
