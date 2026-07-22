using System;
using UnityEngine;

namespace UI
{
	[Serializable]
	public class LocalizedTextEntry
	{
		public string Language;
		[TextArea(3, 7)] public string Text;
	}
}
