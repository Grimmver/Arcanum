using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CharacterFloatingTextHandler : MonoBehaviour
{
	[SerializeField] private GameObject floatingTextPrefab;
	[SerializeField] private Vector3 offsetMin = new Vector3(-0.5f, -0.7f, 0f);
	[SerializeField] private Vector3 offsetMax = new Vector3(0.5f, -0.3f, 0f);

	private bool lastDamageDirectionRight = false;
	private Character character;
	private Canvas worldCanvas;

	private void Awake()
	{
		character = GetComponentInParent<Character>();
		worldCanvas = GetComponent<Canvas>();

		if (character == null)
		{
			Debug.LogError("CharacterFloatingTextHandler must be attached to a GameObject with a Character component in parents.");
			enabled = false;
			return;
		}

		SetupCanvas();

		character.OnTakeDamage += HandleDamage;
		character.OnDefendDamage += HandleBlockDamage;
		character.OnAddArmor += HandleAddArmor;
		character.OnHeal += HandleHeal;
	}

	private void OnDestroy()
	{
		if (character == null) return;

		character.OnTakeDamage -= HandleDamage;
		character.OnDefendDamage -= HandleBlockDamage;
		character.OnAddArmor -= HandleAddArmor;
		character.OnHeal -= HandleHeal;
	}

	private void SetupCanvas()
	{
		worldCanvas.renderMode = RenderMode.WorldSpace;
		worldCanvas.overrideSorting = true;
		worldCanvas.sortingOrder = 100;
	}

	private void SpawnText(string value, Color color, IFloatingTextAnimationStrategy strategy)
	{
		if (floatingTextPrefab == null) return;

		Vector3 randomizedOffset = new Vector3(
			Random.Range(offsetMin.x, offsetMax.x),
			Random.Range(offsetMin.y, offsetMax.y),
			Random.Range(offsetMin.z, offsetMax.z)
		);

		Vector3 spawnPos = transform.position + randomizedOffset;
		GameObject go = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity, worldCanvas.transform);

		var floatingText = go.GetComponent<FloatingText>();
		if (floatingText != null)
		{
			floatingText.Initialize(value, color, strategy);
		}
	}

	private void HandleDamage(Damage damage)
	{
		lastDamageDirectionRight = !lastDamageDirectionRight;
		float direction = lastDamageDirectionRight ? 1f : -1f;

		SpawnText($"{damage.amount}", Color.red, new DamageTextAnimation(direction));
	}

	private void HandleBlockDamage(Damage damage) => SpawnText($"{damage.amount}", Color.whiteSmoke, new BlockTextAnimation());

	private void HandleAddArmor(int amount) => SpawnText($"{amount}", Color.whiteSmoke, new HealTextAnimation());

	private void HandleHeal(int amount) => SpawnText($"{amount}", Color.green, new HealTextAnimation());
}
