using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public enum ElementType
{
	Fire,
	Ice,
	Lighting,
	Shield,
	Blood,
	Earth,
	Poison,
	Trick,
	Arrow,
	Heal,
	Sword,
}

public class Tile : MonoBehaviour
{
	public int x;
	public int y;
	// [SerializeField] public DamageType damageType;
	public ElementType elementType;
	public bool isMatched = false;
	public bool isAnimating = false;
	public Vector3 originalScale = Vector3.one;

	[SerializeField] private float gravity = 9.8f;
	[SerializeField] private bool canLeadMatches = true;
	[SerializeField] private TileAnimator _animator;
	public TileAnimator animator => _animator;

	[SerializeField] protected LocalizedTextBlock tileName;
	public string Name => tileName.Get();

	[SerializeField] protected LocalizedTextBlock description;
	public string Description => description.Get();

	[SerializeField] private bool isPrimarilyOffensive;

	[SerializeField] public TileMatchEffectHolder tileMatchEffectHolder;

	public Sprite Sprite
	{
		get
		{
			if (TryGetComponent(out SpriteRenderer spriteRenderer)) return spriteRenderer.sprite;

			return GetComponentInChildren<SpriteRenderer>().sprite;
		}
	}

	public MatchManager matchManager;

	private bool isAlive = true;

	private void OnDestroy()
	{
		isAlive = false;
		TryCompleteTile();
	}

	private void OnDisable()
	{
		isAlive = false;
		TryCompleteTile();
	}

	private void TryCompleteTile()
	{
		StopAllCoroutines();
		isAnimating = false;
		// ?
		// TileEventBus.TileMatched(this, 0);
		matchManager?.AddReady();
	}

	public void StopTileCoroutines()
	{
		isAnimating = false;
		StopAllCoroutines();

		if (_animator != null)
		{
			_animator.StopAllCoroutines();
		}
	}

	public IEnumerator AnimateElementGrow()
	{
		isAnimating = true;

		float growDuration = 0.3f;
		float elapsedTime = 0f;

		originalScale = transform.localScale;
		Vector3 targetScale = originalScale;

		while (elapsedTime < growDuration)
		{
			if (!isAlive || gameObject == null || transform == null || !gameObject.activeInHierarchy)
			{
				TryCompleteTile();
				yield break;
			}

			elapsedTime += Time.deltaTime;
			float t = elapsedTime / growDuration;
			t = t * t * (3f - 2f * t);
			transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
			yield return null;
		}

		transform.localScale = targetScale;
		isAnimating = false;

		matchManager?.AddReady();
	}

	public bool CanLeadMatches() => canLeadMatches;

	public bool CanMatchWithTile(Tile other) => elementType == other.elementType;

	public void HandleMatch(List<GameObject> tiles, Action onComplete)
	{
		if (!isAlive)
		{
			matchManager?.AddReady();
			TileEventBus.TileMatched(this, TileEventBus.ModifyMatchCount(this, 0));
			onComplete?.Invoke();
			return;
		}

		tileMatchEffectHolder.PrecalcList();
		List<TileMatchEffect> tileMatchEffects = tileMatchEffectHolder.tileMatchEffects;

		int baseCount = tiles.Count;
		int modifiedCount = TileEventBus.ModifyMatchCount(this, baseCount);

		if (tileMatchEffects == null || tileMatchEffects.Count == 0)
		{
			matchManager?.AddReady();
			TileEventBus.TileMatched(this, modifiedCount);
			onComplete?.Invoke();
			return;
		}

		int effectId = 0;

		void ExecuteNextEffect()
		{
			if (effectId >= tileMatchEffects.Count || !isAlive)
			{
				matchManager?.AddReady();
				TileEventBus.TileMatched(this, modifiedCount);
				onComplete?.Invoke();
				return;
			}

			tileMatchEffects[effectId].ExecuteEffect(modifiedCount, null, () =>
			{
				effectId++;
				ExecuteNextEffect();
			});
		}

		ExecuteNextEffect();
	}

	private void TriggerMatchProcessed(int matchCount, Action onComplete)
	{
		TileEventBus.TileMatched(this, matchCount);
		matchManager?.AddReady();
		onComplete?.Invoke();
	}

	public bool IsPrimarilyOffensive()
	{
		return isPrimarilyOffensive;
	}
}
