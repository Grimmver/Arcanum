using DG.Tweening;
using System.Collections.Generic;
using UI;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class Item : MonoBehaviour
{
	[Header("General Item")]
	[SerializeField] private LocalizedTextBlock itemName;
	public string ItemName => itemName.Get();

	[SerializeField] private int cost;
	public int Cost => cost;

	[SerializeField] private LocalizedTextBlock description;
	public string Description => description.Get();

	[SerializeField] private ItemRarity rarity;
	public ItemRarity Rarity => rarity;

	[SerializeField] private List<Item> incompatibleItems = new();
	public IReadOnlyList<Item> IncompatibleItems => incompatibleItems;

	public ItemSlot CurrentSlot;

	[SerializeField] protected Player owner;

	public Sprite Sprite
	{
		get
		{
			if (TryGetComponent(out SpriteRenderer spriteRenderer)) return spriteRenderer.sprite;

			return GetComponentInChildren<SpriteRenderer>().sprite;
		}

		set
		{
			if (TryGetComponent(out SpriteRenderer spriteRenderer))
			{
				spriteRenderer.sprite = value;
			}
			else
			{
				var childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
				if (childSpriteRenderer != null)
				{
					childSpriteRenderer.sprite = value;
				}
			}
		}
	}

	private bool isOverShopSellZone;


	[Header("Dragging Animation")]
	[SerializeField] private float rotationAmount = 1000f;
	[SerializeField] private float rotationSmoothness = 10f;

	private Vector3 offset;
	private Vector3 originalPosition;
	private Vector3 originalLocalPosition;
	private bool isDragging = false;

	private Rigidbody2D rb;
	private float targetZRotation = 0f;
	private Vector3 lastMouseWorldPos;

	private ItemSlot lastSlot;
	private ItemUI itemUI;

	private float mouseDownTime;
	private Vector3 mouseDownPosition;
	private const float clickThresholdTime = 0.2f;
	private const float clickThresholdDistance = 0.1f;
	private bool wasUIOpenOnClick;
	private bool hasDraggedEnoughToCancelClick;
	private Vector3 originalLocalScale;
	private Tween activeTween;

	public abstract void Equip(Player player);
	public abstract void Unequip();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0;
		rb.freezeRotation = true;

		itemUI = GetComponentInChildren<ItemUI>();
		originalLocalScale = transform.localScale;
	}

	private void OnDisable()
	{
		Unequip();
	}

	private void OnMouseDown()
	{
		if (SettingsCanvas.Instance?.IsOpen == true || ShopCanvas.Instance?.IsShopPopupOpen == true /*|| ShopCanvas.Instance.IsOpen*/) return;

		originalPosition = transform.position;
		originalLocalPosition = transform.localPosition;

		lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		offset = transform.position - new Vector3(lastMouseWorldPos.x, lastMouseWorldPos.y, 0);
		isDragging = true;

		mouseDownTime = Time.time;
		mouseDownPosition = lastMouseWorldPos;

		lastSlot = CurrentSlot;

		wasUIOpenOnClick = itemUI != null && itemUI.IsOpen;
		hasDraggedEnoughToCancelClick = false;

		if (wasUIOpenOnClick)
			itemUI.Open(false);
	}

	private void OnMouseDrag()
	{
		if (!isDragging) return;

		Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float dragDistance = Vector3.Distance(currentMousePos, mouseDownPosition);
		if (dragDistance > clickThresholdDistance)
			hasDraggedEnoughToCancelClick = true;

		Vector3 targetPos = new Vector3(currentMousePos.x, currentMousePos.y, 0) + offset;
		if (Time.timeScale == 0)
			transform.position = targetPos;
		else
			rb.MovePosition(targetPos);

		float deltaX = currentMousePos.x - lastMouseWorldPos.x;
		targetZRotation = Mathf.Clamp(deltaX * rotationAmount, -rotationAmount, rotationAmount);
		lastMouseWorldPos = currentMousePos;

		float newZ = Mathf.LerpAngle(transform.eulerAngles.z, targetZRotation, Time.deltaTime * rotationSmoothness);
		transform.rotation = Quaternion.Euler(0, 0, newZ);
	}

	private void OnMouseUp()
	{
		if (SettingsCanvas.Instance?.IsOpen == true || ShopCanvas.Instance?.IsShopPopupOpen == true) return;

		isDragging = false;
		targetZRotation = 0f;
		transform.rotation = Quaternion.identity;

		Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float heldDuration = Time.time - mouseDownTime;
		float movedDistance = Vector3.Distance(currentMousePos, mouseDownPosition);

		if (heldDuration <= clickThresholdTime && !hasDraggedEnoughToCancelClick)
		{
			itemUI?.Open(!wasUIOpenOnClick); // CLICK
			return;
		}

		itemUI?.Open(false);

		if (isOverShopSellZone)
		{
			ShopSell.Instance.TrySell(this);
			return;
		}

		// DRAG logic to return or assign to slot
		Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
		foreach (var col in colliders)
		{
			if (col.TryGetComponent(out ItemSlot targetSlot))
			{
				if (targetSlot == lastSlot)
				{
					targetSlot.AssignItem(this);
					return;
				}

				if (targetSlot.EquippedItem != null)
				{
					SwapItems(targetSlot);
					return;
				}

				if (targetSlot.CanAcceptItem(this))
				{
					targetSlot.AssignItem(this);
					return;
				}
			}
		}

		transform.position = originalPosition;
		lastSlot?.AssignItem(this);
	}

	private void SwapItems(ItemSlot targetSlot)
	{
		Item existing = targetSlot.EquippedItem;

		ItemSlot sourceSlot = CurrentSlot;

		targetSlot.RemoveItem();
		targetSlot.AssignItem(this);

		if (sourceSlot != null)
		{
			sourceSlot.RemoveItem();
			sourceSlot.AssignItem(existing);
		}
		else
		{
			existing.CurrentSlot = null;
			existing.transform.SetParent(null);
		}
	}

	public void CancelDragging()
	{
		if (!isDragging) return;

		isDragging = false;
		targetZRotation = 0f;
		transform.rotation = Quaternion.identity;

		if (CurrentSlot != null)
		{
			CurrentSlot.AssignItem(this);
		}
		else
		{
			transform.position = originalPosition;
			transform.localPosition = originalLocalPosition;
		}

		itemUI?.Open(false);
	}

	public virtual void ActivateAnimation()
	{
		if (activeTween != null && activeTween.IsActive())
			activeTween.Kill();

		transform.localScale = originalLocalScale;

		transform.rotation = Quaternion.identity;

		Vector3 originalScale = transform.localScale;
		Vector3 targetScale = originalScale * 1.7f;

		activeTween = transform.DOScale(targetScale, 0.1f)
			.SetEase(Ease.InOutSine)
			.SetLoops(2, LoopType.Yoyo)
			.SetLink(gameObject)
			.SetUpdate(true);
	}

	public void Open(bool action)
	{
		itemUI.Open(action);
	}

	public void SetOverShopSellZone(bool value) => isOverShopSellZone = value;
}
