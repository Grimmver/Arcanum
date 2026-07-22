using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BossCurse : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private LocalizedTextBlock curseName;
	public string Name => curseName.Get();

	[SerializeField] private LocalizedTextBlock description;
	public string Description => description.Get();

	[SerializeField] private int strength;
	public int Strength => strength;

	[SerializeField][Range(0, 1f)] private float scale;

	[SerializeField] private GameObject cursePanelPrefab;
	private Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void Activate()
	{


		Animate();
	}

	private void Animate()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (cursePanelPrefab == null) return;


	}

	private void ShowCursePanel()
	{

	}
}
