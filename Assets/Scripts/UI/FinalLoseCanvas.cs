using TMPro;
using UnityEngine;

namespace UI
{
	public class FinalLoseCanvas : UICanvas<FinalLoseCanvas>
	{
		[SerializeField] private TMP_Text loseText;
		[SerializeField] private TMP_Text OkText;

		public override void Open(bool action)
		{
			base.Open(action);
		}

		public void OnCloseButtonClick() => Open(false);
	}
}
