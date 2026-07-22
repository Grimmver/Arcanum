using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Canvas))]
	public abstract class UICanvas<T> : MonoBehaviour where T : UICanvas<T>
	{
		public static T Instance { get; private set; }

		[Header("General")]
		[SerializeField] protected bool openOnAwake = false;
		[SerializeField] protected Image background;

		private bool isOpen;
		public bool IsOpen => isOpen;

		[HideInInspector] public Canvas canvas;

		private bool isInitialized;

		protected virtual void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = (T)this;
			canvas = GetComponent<Canvas>();

			StartCoroutine(DelayedInit());
		}

		public virtual void Open(bool action)
		{
			canvas.enabled = action;
			isOpen = action;

			if (background != null)
			{
				background.gameObject.SetActive(action);
				background.enabled = action;
			}
		}

		private IEnumerator DelayedInit()
		{
			yield return null;
			Open(openOnAwake);
		}
	}
}
