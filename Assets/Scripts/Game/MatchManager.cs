using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class WeightedTilePrefab
{
	public GameObject tilePrefab;
	public int tileWeight;
}

[System.Serializable]
public class Fraction
{
	public int numerator;
	public int denominator;
}

[System.Serializable]
public class tileToEffectHolder
{
	public GameObject tile;
	public TileMatchEffectHolder effectHolder;
}

public class MatchManager : MonoBehaviour
{
	public static MatchManager Instance { get; private set; }

	[SerializeField] private List<WeightedTilePrefab> elements;  // References to tile prefabs
	[SerializeField] private int gridSize = 6;  // Grid dimensions
	[SerializeField] private int minStreak = 3; // Minimum streak for a match
	[SerializeField] private int minSquare = 2; // Minimum square size for a match
	[SerializeField] private float cellSize = 0.01f;  // Size of each grid cell
	[SerializeField] private Vector3 gridPosition = new Vector3(0, 0, 0);  // Position of the grid
	[SerializeField] private float spawnYOffset = 2f;  // Offset of the grid from the center of the screen

	[SerializeField] private float randomDelayRange = 0.2f;
	[SerializeField] private float swipeThreshold = 0.5f;

	[SerializeField] public BattleManager battleManager;

	[SerializeField] private Fraction regenChance; // Chance to regen new tiles if they WOULD/WOULD NOT create combos.
	[SerializeField] private bool isLucky = false; // Determines whether combos or comboless tiles are regened.
	[SerializeField] private int maxRegenCnt = 100; // Determines the point at which regens stop no matter what.
	[SerializeField] private int luckPunishment = 100; // Temporarily decreases luck by the punishment when the player gets lucky.
	[SerializeField] private int tempLuckDecrease = 0; // Luck down until the end of the current turn.
    public int tempLuckIncrease = 50;
	public int tempLuckDecreaseRate = 10;
    public bool ResumeLuck = false;
	[HideInInspector] public int tempLuckDuration = 0; 

    [SerializeField] public List<tileToEffectHolder> tileToEffectHolderList = new List<tileToEffectHolder>();

	public Dictionary<GameObject, TileMatchEffectHolder> tileToEffectHolderDict = new Dictionary<GameObject, TileMatchEffectHolder>();

	[SerializeField] private RectTransform gridOverlay;

	private GameObject[,] grid;
	private int cntAnimating = 0;
	public int CntAnimating => cntAnimating;
	private bool isReady = false;
	private int elementsInPlace = 0;

	private Vector2 touchStart;
	private bool isSwiping = false;
	private GameObject selectedElement = null;

	private bool canInteract = true;

	static int[] dx = { 1, 0, -1, 0 };
	static int[] dy = { 0, 1, 0, -1 };

	private int totalWeight;
	private List<int> randomToTileId;

	public int debugOverlayCount = 0;
	public event Action OnMatchingFinished;

	// private RuntimeInspector runtimeInspector;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		foreach (var kv in tileToEffectHolderList)
		{
			tileToEffectHolderDict[kv.tile] = kv.effectHolder;
		}

	}

	void Start()
	{
		PrecalcRandomWeights();
		InitializeGrid();
	}


	public void PrecalcRandomWeights()
	{
		totalWeight = elements.Sum(e => Mathf.Max(0, e.tileWeight));
		randomToTileId = new List<int>();

		if (totalWeight <= 0)
		{
			return;
		}

		for (int tileId = 0; tileId < elements.Count; tileId++)
		{
			int count = Mathf.Max(0, elements[tileId].tileWeight);
			for (int i = 0; i < count; i++)
			{
				randomToTileId.Add(tileId);
			}
		}
	}


	void InitializeGrid()
	{
		grid = new GameObject[gridSize, gridSize];
		elementsInPlace = 0;
		isReady = false;
		FillGrid();
		StartCoroutine(InitializeGridWithAnimation());

	}

	void FillGrid()
	{

		PrecalcRandomWeights();

		RestoreLuck();

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				SpawnRandomTile(x, y);
			}
		}

		bool isGood = false;

		while (!isGood)
		{

			isGood = true;

			while (CheckSoftlock())
			{
				isGood = false;
				for (int x = 0; x < gridSize; x++)
				{
					for (int y = 0; y < gridSize; y++)
					{
						if (!grid[x, y].GetComponent<Tile>().isMatched)
						{
							continue;
						}
						Destroy(grid[x, y]);
						grid[x, y] = null;
						SpawnRandomTile(x, y);
					}
				}
			}

			while (CheckForMatchesOnField())
			{
				isGood = false;
				for (int x = 0; x < gridSize; x++)
				{
					for (int y = 0; y < gridSize; y++)
					{
						if (!grid[x, y].GetComponent<Tile>().isMatched)
						{
							continue;
						}
						Destroy(grid[x, y]);
						grid[x, y] = null;
						SpawnRandomTile(x, y);
					}
				}
			}
		}
	}

	IEnumerator InitializeGridWithAnimation()
	{
		UpdateCntAnimating(1);
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				grid[x, y].GetComponent<Renderer>().enabled = false;
			}
		}
		for (int x = 0; x < gridSize; x++)
		{
			StartCoroutine(AnimateColumn(x));
		}
		UpdateCntAnimating(-1);
		yield return null;
	}

	IEnumerator AnimateColumn(int x)
	{
		for (int y = 0; y < gridSize; y++)
		{
			float randomDelay = Random.Range(0f, randomDelayRange);
			yield return new WaitForSeconds(randomDelay);
			grid[x, y].GetComponent<Renderer>().enabled = true;
			StartCoroutine(grid[x, y].GetComponent<Tile>().AnimateElementGrow());
		}
		yield return null;
	}

	void SpawnRandomTile(int x, int y)
	{
		if (grid[x, y] != null || totalWeight <= 0 || randomToTileId.Count == 0)
		{
			//Debug.LogWarning($"SpawnRandomTile skipped at ({x}, {y}) due to empty tile list.");
			return;
		}

		if (grid[x, y] != null)
		{
			return;
		}

		Vector3 startPos = transform.position + gridPosition;
		startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
		startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;

		Vector3 targetPos = startPos + new Vector3(x * cellSize, y * cellSize, -5f);

		var randomResult = Random.Range(0, totalWeight);
		var type = randomToTileId[randomResult];
		GameObject element = Instantiate(elements[type].tilePrefab, targetPos, Quaternion.identity);
		element.transform.SetParent(transform);

		element.AddComponent<BoxCollider2D>();

		Tile tile = element.GetComponent<Tile>();
		if (tile == null)
		{
			tile = element.AddComponent<Tile>();
		}

		tile.x = x;
		tile.y = y;
		tile.elementType = elements[type].tilePrefab.GetComponent<Tile>().elementType;
		tile.matchManager = this;
		tile.tileMatchEffectHolder = tileToEffectHolderDict[elements[type].tilePrefab];

		grid[x, y] = element;
	}

	void AddClickHandler(GameObject element)
	{
		if (element.GetComponent<Collider2D>() == null)
		{
			element.AddComponent<BoxCollider2D>();
		}

		ElementClickHandler clickHandler = element.AddComponent<ElementClickHandler>();
		clickHandler.gridManager = this;
	}

	public void OnElementClicked(GameObject element)
	{
		if (!isReady || cntAnimating > 0)
			return;

		Tile tile = element.GetComponent<Tile>();
		if (tile == null)
			return;

		StartCoroutine(HandleElementDestruction(tile.x, tile.y));
	}

	IEnumerator HandleElementDestruction(int x, int y)
	{
		UpdateCntAnimating(1);

		if (grid[x, y] != null)
		{
			Destroy(grid[x, y]);
			grid[x, y] = null;
		}

		for (int currentY = y + 1; currentY < gridSize; currentY++)
		{
			if (grid[x, currentY] != null)
			{
				GameObject element = grid[x, currentY];
				Tile tile = element.GetComponent<Tile>();

				grid[x, currentY - 1] = element;
				grid[x, currentY] = null;

				tile.y = currentY - 1;

				Vector3 startPos = transform.position + gridPosition;
				startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
				startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
				Vector3 targetPos = startPos + new Vector3(x * cellSize, (currentY - 1) * cellSize, -5f);

				element.GetComponent<Tile>().animator.AnimateElementDrop(targetPos);
			}
		}

		yield return StartCoroutine(DropElementsAndSpawnNew());

		UpdateCntAnimating(-1);
	}

	IEnumerator DisplayNewElementsAtTop(int x)
	{

		int emptySpaces = 0;
		for (int y = 0; y < gridSize; y++)
		{
			if (!grid[x, y].GetComponent<Renderer>().enabled)
			{
				emptySpaces++;
			}
		}

		for (int i = 0; i < emptySpaces; i++)
		{
			int targetY = -1;
			for (int y = gridSize - 1; y >= 0; y--)
			{
				if (!grid[x, y].GetComponent<Renderer>().enabled)
				{
					targetY = y;
					break;
				}
			}

			if (targetY == -1)
				break;

			// Debug.Log(x + " " + targetY);
			grid[x, targetY].GetComponent<Renderer>().enabled = true;
			StartCoroutine(grid[x, targetY].GetComponent<Tile>().AnimateElementGrow());

			yield return new WaitForSeconds(0.05f);
		}
	}

	public bool IsReady() => isReady;

	void OnDrawGizmos()
	{
		Vector3 startPos = transform.position + gridPosition;
		startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
		startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;

		Gizmos.color = Color.yellow;

		for (int x = 0; x <= gridSize; x++)
		{
			Vector3 lineStart = startPos + new Vector3(x * cellSize - cellSize / 2f, -cellSize / 2f, 0);
			Vector3 lineEnd = startPos + new Vector3(x * cellSize - cellSize / 2f, (gridSize - 1) * cellSize + cellSize / 2f, 0);
			Gizmos.DrawLine(lineStart, lineEnd);
		}

		for (int y = 0; y <= gridSize; y++)
		{
			Vector3 lineStart = startPos + new Vector3(-cellSize / 2f, y * cellSize - cellSize / 2f, 0);
			Vector3 lineEnd = startPos + new Vector3((gridSize - 1) * cellSize + cellSize / 2f, y * cellSize - cellSize / 2f, 0);
			Gizmos.DrawLine(lineStart, lineEnd);
		}

		Gizmos.color = Color.red;
		Vector3 centerPos = transform.position + gridPosition;
		Gizmos.DrawWireSphere(centerPos, cellSize * 0.1f);

		Gizmos.color = Color.green;
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				Vector3 cellCenter = startPos + new Vector3(x * cellSize, y * cellSize, 0);
				Gizmos.DrawWireCube(cellCenter, Vector3.one * cellSize * 0.1f);
			}
		}
	}

	void Update()
	{
		HandleSwipeInput();
	}

	void HandleSwipeInput()
	{
		if (!canInteract || debugOverlayCount > 0 || UI.ShopCanvas.Instance.IsOpen || UI.SettingsCanvas.Instance.IsOpen || UI.RewardsCanvas.Instance.IsOpen || cntAnimating > 0)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

			if (hit.collider != null)
			{
				selectedElement = hit.collider.gameObject;
				touchStart = mousePosition;
				isSwiping = true;
			}
		}
		else if (Input.GetMouseButtonUp(0) && isSwiping && selectedElement != null)
		{

			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 swipeDelta = mousePosition - touchStart;

			if (swipeDelta.magnitude > swipeThreshold)
			{
				if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
				{
					if (swipeDelta.x > 0)
					{
						TrySwapElements(selectedElement, 1, 0);
					}
					else
					{
						TrySwapElements(selectedElement, -1, 0);
					}
				}
				else
				{
					if (swipeDelta.y > 0)
					{
						TrySwapElements(selectedElement, 0, 1);
					}
					else
					{
						TrySwapElements(selectedElement, 0, -1);
					}
				}
			}

			isSwiping = false;
			selectedElement = null;
		}

		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
				RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

				if (hit.collider != null)
				{
					selectedElement = hit.collider.gameObject;
					touchStart = touchPosition;
					isSwiping = true;
				}
			}
			else if (touch.phase == TouchPhase.Ended && isSwiping && selectedElement != null)
			{
				Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
				Vector2 swipeDelta = touchPosition - touchStart;

				if (swipeDelta.magnitude > swipeThreshold)
				{
					if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
					{
						if (swipeDelta.x > 0)
						{
							TrySwapElements(selectedElement, 1, 0);
						}
						else
						{
							TrySwapElements(selectedElement, -1, 0);
						}
					}
					else
					{
						if (swipeDelta.y > 0)
						{
							TrySwapElements(selectedElement, 0, 1);
						}
						else
						{
							TrySwapElements(selectedElement, 0, -1);
						}
					}
				}

				isSwiping = false;
				selectedElement = null;
			}
		}
	}

	void TrySwapElements(GameObject element, int deltaX, int deltaY)
	{
		if (!isReady || cntAnimating > 0)
			return;

		Tile tile = element.GetComponent<Tile>();
		if (tile == null)
			return;

		int newX = tile.x + deltaX;
		int newY = tile.y + deltaY;

		if (newX < 0 || newX >= gridSize || newY < 0 || newY >= gridSize)
			return;

		if (grid[newX, newY] == null)
			return;

		StartCoroutine(SwapElements(tile.x, tile.y, newX, newY));
	}

	IEnumerator SwapElements(int x1, int y1, int x2, int y2)
	{

		AudioManager.Instance?.PlaySound("Swipe sound");

		UpdateCntAnimating(1);

		BattleManager.EventBus.PlayerSwapped();

		GameObject element1 = grid[x1, y1];
		GameObject element2 = grid[x2, y2];

		Tile tile1 = element1.GetComponent<Tile>();
		Tile tile2 = element2.GetComponent<Tile>();

		Vector3 startPos = transform.position + gridPosition;
		startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
		startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;

		Vector3 pos1 = startPos + new Vector3(x1 * cellSize, y1 * cellSize, -5f);
		Vector3 pos2 = startPos + new Vector3(x2 * cellSize, y2 * cellSize, -5f);

		float swapDuration = 0.2f;
		float elapsedTime = 0f;

		Vector3 startPos1 = element1.transform.position;
		Vector3 startPos2 = element2.transform.position;

		tile1.animator.SwapAnimation(pos2, swapDuration);
		tile2.animator.SwapAnimation(pos1, swapDuration);

		while (elapsedTime < swapDuration)
		{
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		grid[x1, y1] = element2;
		grid[x2, y2] = element1;

		tile1.x = x2;
		tile1.y = y2;
		tile2.x = x1;
		tile2.y = y1;

		bool hasMatch = CheckForMatchesOnField();

		if (!hasMatch)
		{
			yield return new WaitForSeconds(0.2f);

			elapsedTime = 0f;
			startPos1 = element1.transform.position;
			startPos2 = element2.transform.position;

			tile1.animator.SwapAnimation(pos1, swapDuration);
			tile2.animator.SwapAnimation(pos2, swapDuration);

			while (elapsedTime < swapDuration)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			grid[x1, y1] = element1;
			grid[x2, y2] = element2;

			tile1.x = x1;
			tile1.y = y1;
			tile2.x = x2;
			tile2.y = y2;
			BattleManager.EventBus.PlayerSwapTurnFinished();
			yield return null;
		}
		else
		{
			yield return StartCoroutine(HandleMatchesOnField());

			BattleManager.EventBus.PlayerSwapTurnFinished();
		}

		UpdateCntAnimating(-1);
	}

	Tile[,] GetTiles()
	{
		Tile[,] tiles = new Tile[gridSize, gridSize];
		if (grid == null)
		{
			for (int x = 0; x < gridSize; x++)
			{
				for (int y = 0; y < gridSize; y++)
				{
					tiles[x, y] = null;
				}
			}
			return tiles;
		}
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (grid[x, y] == null)
				{
					tiles[x, y] = null;
					continue;
				}
				Tile tile = grid[x, y].GetComponent<Tile>();
				tiles[x, y] = tile;
			}
		}
		return tiles;
	}

	bool CheckForMatchesOnField()
	{
		return CheckForMatches(GetTiles());
	}

	bool CheckForMatches(Tile[,] tiles)
	{
		bool hasMatch = false;

		for (int y = 0; y < gridSize; y++)
		{
			for (int x = 0; x <= gridSize - minStreak; x++)
			{
				if (tiles[x, y] == null)
				{
					continue;
				}

				Tile headTile = tiles[x, y];
				if (headTile.isAnimating)
				{
					continue;
				}

				int comboSize = 1;

				for (int xx = x + 1; xx < gridSize; xx++)
				{
					if (tiles[xx, y] == null)
					{
						break;
					}
					Tile otherTile = tiles[xx, y];
					if (otherTile.isAnimating || !headTile.CanMatchWithTile(otherTile))
					{
						break;
					}
					comboSize++;
					if (!headTile.CanLeadMatches())
					{
						headTile = otherTile;
					}
				}

				if (comboSize < minStreak)
				{
					continue;
				}
				hasMatch = true;
				for (int offset = 0; offset < comboSize; offset++)
				{
					tiles[x + offset, y].isMatched = true;
				}
			}
		}

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y <= gridSize - minStreak; y++)
			{
				if (tiles[x, y] == null)
				{
					continue;
				}

				Tile headTile = tiles[x, y];
				if (headTile.isAnimating)
				{
					continue;
				}

				int comboSize = 1;

				for (int yy = y + 1; yy < gridSize; yy++)
				{
					if (tiles[x, yy] == null)
					{
						break;
					}
					Tile otherTile = tiles[x, yy];
					if (otherTile.isAnimating || !headTile.CanMatchWithTile(otherTile))
					{
						break;
					}
					comboSize++;
					if (!headTile.CanLeadMatches())
					{
						headTile = otherTile;
					}
				}
				if (comboSize < minStreak)
				{
					continue;
				}
				hasMatch = true;
				for (int offset = 0; offset < comboSize; offset++)
				{
					tiles[x, y + offset].isMatched = true;
				}
			}
		}

		// Check for square matches
		for (int x = 0; x <= gridSize - minSquare; x++)
		{
			for (int y = 0; y <= gridSize - minSquare; y++)
			{

				Tile headTile = null;
				bool isSquareFeasible = true;

				for (int xx = x; xx < x + minSquare; xx++)
				{
					for (int yy = y; yy < y + minSquare; yy++)
					{
						if (tiles[xx, yy] == null)
						{
							isSquareFeasible = false;
							break;
						}
						Tile currentTile = tiles[xx, yy];
						if (currentTile.isAnimating)
						{
							isSquareFeasible = false;
							break;
						}
						if (currentTile.CanLeadMatches())
						{
							headTile = currentTile;
						}
					}
					if (!isSquareFeasible)
					{
						break;
					}
				}
				if (!isSquareFeasible || headTile == null)
				{
					continue;
				}

				for (int xx = x; xx < x + minSquare; xx++)
				{
					for (int yy = y; yy < y + minSquare; yy++)
					{
						Tile otherTile = tiles[xx, yy];
						if (!headTile.CanMatchWithTile(otherTile))
						{
							isSquareFeasible = false;
							break;
						}
					}
					if (!isSquareFeasible)
					{
						break;
					}
				}

				if (!isSquareFeasible)
				{
					continue;
				}

				hasMatch = true;

				for (int xx = x; xx < x + minSquare; xx++)
				{
					for (int yy = y; yy < y + minSquare; yy++)
					{
						tiles[xx, yy].isMatched = true;
					}
				}
			}
		}

		return hasMatch;
	}

	private IEnumerator HandleMatchesOnField()
	{

		List<List<Tile>> connectedTileGroups = FindConnectedGroups(GetTiles());
		List<List<GameObject>> connectedGroups = new List<List<GameObject>>();

		foreach (List<Tile> tileGroup in connectedTileGroups)
		{
			List<GameObject> group = new List<GameObject>();
			foreach (Tile tile in tileGroup)
			{
				group.Add(grid[tile.x, tile.y]);
			}
			connectedGroups.Add(group);
		}

		foreach (List<GameObject> group in connectedGroups)
		{
			float duration = 0.3f;
			float elapsedTime = 0f;

			Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
			foreach (GameObject element in group)
			{
				if (element != null)
					originalScales[element] = element.transform.localScale;
			}

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / duration;

				foreach (GameObject element in group)
				{
					if (element != null)
					{
						element.transform.localScale = Vector3.Lerp(originalScales[element], Vector3.zero, t);
					}
				}

				yield return null;
			}

			bool IsBusy = true;
			battleManager.IsProcessingMatch = true;

			Tile leader = null;
			foreach (var go in group)
			{
				if (go != null && go.TryGetComponent(out Tile tile))
				{
					leader = tile;
					break;
				}
			}

			if (leader != null)
			{
				leader.HandleMatch(group, () =>
				{
					IsBusy = false;
				});
			}
			else
			{
				IsBusy = false;
				battleManager.IsProcessingMatch = false;
			}

			float timeout = 5f;
			float waitTime = 0f;

			while (IsBusy && waitTime < timeout)
			{
				waitTime += Time.deltaTime;
				yield return null;
			}

			IsBusy = false;

			yield return new WaitUntil(() => !battleManager.IsProcessingMatch);
			yield return new WaitForSeconds(0.1f);
		}

		foreach (List<GameObject> group in connectedGroups)
		{
			foreach (GameObject element in group)
			{
				if (element != null && element.TryGetComponent(out Tile tile))
				{
					grid[tile.x, tile.y] = null;
					Destroy(element);
				}
			}
		}

		yield return StartCoroutine(DropElementsAndSpawnNew());
	}

	List<List<Tile>> FindConnectedGroups(Tile[,] tiles)
	{
		List<Tile> matchedElements = new List<Tile>();

		// Group connected matched elements
		List<List<Tile>> connectedGroups = new List<List<Tile>>();
		HashSet<Tile> processedElements = new HashSet<Tile>();

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (tiles[x, y] != null)
				{
					Tile tile = tiles[x, y];
					if (!tile.isMatched || processedElements.Contains(tiles[x, y]) || !tile.CanLeadMatches())
					{
						continue;
					}
					HashSet<Tile> currentGroup = new HashSet<Tile>();
					FindConnectedElements(tile, x, y, currentGroup, processedElements, tiles);
					connectedGroups.Add(currentGroup.ToList());
				}
			}
		}

		return connectedGroups;
	}

	public void RefreshTiles()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (grid[x, y] != null && grid[x, y].TryGetComponent(out Tile tile))
				{
					tile.StopTileCoroutines();
				}
			}
		}
		StopAllCoroutines();

		isReady = false;
		elementsInPlace = 0;
		cntAnimating = 0;
		BattleManager.instance.FinishProcessingMatch();

		StartCoroutine(DropAndRespawnAllTiles());
	}

	private IEnumerator DropAndRespawnAllTiles()
	{
		UpdateCntAnimating(1);
		isReady = false;

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (grid[x, y] != null)
				{
					Destroy(grid[x, y]);
					grid[x, y] = null;
				}
			}
		}

		yield return new WaitForSeconds(0.2f);

		FillGrid();
		yield return StartCoroutine(InitializeGridWithAnimation());

		if (BattleManager.instance.turnPhase != BattleManager.TurnPhase.PlayerTurn)
		{
			BattleManager.instance.SetTurn(BattleManager.TurnPhase.PlayerTurn);
			BattleManager.EventBus.PlayerTurnStart();
		}

		UpdateCntAnimating(-1);
	}

	private void FindConnectedElements(Tile headTile, int x, int y, HashSet<Tile> currentGroup, HashSet<Tile> processedElements, Tile[,] tiles)
	{

		if (tiles[x, y] == null || currentGroup.Contains(tiles[x, y]))
		{
			// Debug.Log("Skipping1 " + x + " " + y);
			return;
		}

		Tile currentTile = tiles[x, y];

		// Debug.Log("New tile! " + x + " " + y);
		processedElements.Add(currentTile);
		currentGroup.Add(currentTile);

		for (int directionId = 0; directionId < 4; directionId++)
		{
			int xx = x + dx[directionId], yy = y + dy[directionId];
			if (!IsWithinGrid(xx, yy))
			{
				continue;
			}
			Tile otherTile = tiles[xx, yy];
			if (otherTile.isMatched && headTile.CanMatchWithTile(otherTile))

			{
				FindConnectedElements(headTile, xx, yy, currentGroup, processedElements, tiles);
			}
		}
	}

	IEnumerator DropElementsAndSpawnNew()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (grid[x, y] == null)
				{
					for (int aboveY = y + 1; aboveY < gridSize; aboveY++)
					{
						if (grid[x, aboveY] != null)
						{
							grid[x, y] = grid[x, aboveY];
							grid[x, aboveY] = null;

							Tile tile = grid[x, y].GetComponent<Tile>();
							tile.y = y;

							Vector3 startPos = transform.position + gridPosition;
							startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
							startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
							Vector3 targetPos = startPos + new Vector3(x * cellSize, y * cellSize, -5f);

							grid[x, y].GetComponent<Tile>().animator.AnimateElementDrop(targetPos);

							break;
						}
					}
				}
			}
		}

		yield return new WaitForSeconds(0.5f);

		bool[,] isNew = new bool[gridSize, gridSize];
		bool[,] isRegenable = new bool[gridSize, gridSize];

		bool isSkill = CheckForMatchesOnField();
		// UnmarkMatched(); // Probably not needed?

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				isRegenable[x, y] = isNew[x, y] = (grid[x, y] == null);
				SpawnRandomTile(x, y);
			}
		}

		bool isRegenning = true;

		int regenIterationCount = -1;

		while (isRegenning)
		{

			regenIterationCount++;

			for (int x = 0; x < gridSize; x++)
			{
				for (int y = 0; y < gridSize; y++)
				{
					SpawnRandomTile(x, y);
				}
			}

			if (regenIterationCount == maxRegenCnt)
			{
				break;
			}

			isRegenning = false;
			CheckForMatchesOnField();

			for (int x = 0; x < gridSize; x++)
			{
				for (int y = 0; y < gridSize; y++)
				{
					if (!isRegenable[x, y])
					{
						continue;
					}
					if (grid[x, y].GetComponent<Tile>().isMatched)
					{
						if (isLucky)
						{
							continue;
						}
						int randomResult = Random.Range(0, regenChance.denominator);
						if (randomResult < regenChance.numerator)
						{
							// Debug.Log("With combo regen: " + x + " " + y);
							Destroy(grid[x, y]);
							grid[x, y] = null;
							isRegenning = true;
						}
						else
						{
							isRegenable[x, y] = false;
						}
					}
					else
					{
						if (!isLucky)
						{
							continue;
						}
						int randomResult = Random.Range(0, regenChance.denominator);
						if (randomResult < regenChance.numerator)
						{
							// Debug.Log("Without combo regen: " + x + " " + y);
							Destroy(grid[x, y]);
							grid[x, y] = null;
							isRegenning = true;
						}
						else
						{
							isRegenable[x, y] = false;
						}
					}
				}
			}

			UnmarkMatched();
		}

		// Debug.Log("Total iterations:" + regenIterationCount);

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (!isNew[x, y])
				{
					continue;
				}
				grid[x, y].GetComponent<Renderer>().enabled = false;
			}
		}

		for (int x = 0; x < gridSize; x++)
		{
			yield return StartCoroutine(DisplayNewElementsAtTop(x));
		}

		yield return new WaitForSeconds(0.5f);

		if (CheckForMatchesOnField())
		{

			if (!isSkill)
			{
				tempLuckDecrease += luckPunishment;
				DecreaseLuck(luckPunishment);
			}

			yield return StartCoroutine(HandleMatchesOnField());
		}
		else
		{
			RestoreLuck();
		}
	}

	void UnmarkMatched()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (grid[x, y] == null)
				{
					continue;
				}
				grid[x, y].GetComponent<Tile>().isMatched = false;
			}
		}
	}

	public void AddReady()
	{
		elementsInPlace++;

		if (elementsInPlace >= gridSize * gridSize)
		{
			isReady = true;
		}
	}

	public bool IsWithinGrid(int x, int y)
	{
		return (x >= 0 && y >= 0 && x < gridSize && y < gridSize);
	}

	public void SetCanInteract(bool newCanInteract)
	{
		canInteract = newCanInteract;
		
		UpdateOverlay();

		if (canInteract && CheckSoftlock())
		{
			StartCoroutine(ResetField());
		}
	}

	public IEnumerator ResetField()
	{
		UpdateCntAnimating(1);
		while (cntAnimating > 1)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		yield return ClearField();
		while (cntAnimating > 1)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		FillGrid();
		yield return InitializeGridWithAnimation();
		UpdateCntAnimating(-1);
	}

	public IEnumerator ClearField()
	{
		UpdateCntAnimating(1);
		for (int xySum = 0; xySum <= (gridSize << 1) - 2; xySum++)
		{
			StartCoroutine(ClearDiagonal(xySum, 0, -1));
			yield return new WaitForSeconds(0.1f);
		}
		UpdateCntAnimating(-1);
	}

	public IEnumerator ClearDiagonal(int diagonalId, int xSign, int ySign)
	{
		for (int x = 0; x < gridSize; x++)
		{
			int currSum = x;
			if (xSign == -1)
			{
				currSum = gridSize - x - 1;
			}

			int y = diagonalId - currSum;
			if (ySign == -1)
			{
				y = gridSize - y - 1;
			}
			if (IsWithinGrid(x, y))
			{
				StartCoroutine(ClearTile(x, y));
			}
		}
		yield break;
	}

	public IEnumerator ClearTile(int x, int y)
	{

		if (grid[x, y] == null)
		{
			yield break;
		}

		float duration = 0.3f;
		float elapsedTime = 0f;

		Vector3 originalScale = grid[x, y].transform.localScale;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;

			if (grid[x, y] != null)
			{
				grid[x, y].transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
			}

			yield return null;
		}

		grid[x, y] = null;
	}

	public bool CheckSoftlock()
	{
		// Horizontal swaps.
		for (int x = 0; x + 1 < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (IsProgressiveSwap(x, y, x + 1, y))
				{
					return false;
				}
			}
		}
		// Vertical swaps.
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y + 1 < gridSize; y++)
			{
				if (IsProgressiveSwap(x, y, x, y + 1))
				{
					return false;
				}
			}
		}

		// Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Softlock detected!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

		return true;
	}

	public bool IsProgressiveSwap(int x1, int y1, int x2, int y2)
	{
		Tile[,] tiles = GetTiles();
		Tile tile1 = tiles[x1, y1], tile2 = tiles[x2, y2];

		if (tile1 == null || tile2 == null)
		{
			return true;
		}

		tile1.x = x2;
		tile1.y = y2;
		tile2.x = x1;
		tile2.y = y1;

		tiles[x1, y1] = tile2;
		tiles[x2, y2] = tile1;

		bool isGood = CheckForMatches(tiles);

		UnmarkMatched();

		tile1.x = x1;
		tile1.y = y1;
		tile2.x = x2;
		tile2.y = y2;

		if (isGood)
		{
			// Debug.Log("Found valid move: " + x1 + " " + y1 + " <---> " + x2 + " " + y2);
		}

		return isGood;
	}

	private void OnValidate()
	{
		PrecalcRandomWeights();
	}

	public void AddElement(GameObject tilePrefab, int tileWeight)
	{

		if (tileWeight <= 0)
		{
			return;
		}

		for (int elementId = 0; elementId < elements.Count; elementId++)
		{
			if (elements[elementId].tilePrefab == tilePrefab)
			{
				elements[elementId].tileWeight += tileWeight;
				PrecalcRandomWeights();
				return;
			}
		}

		WeightedTilePrefab weightedTilePrefab = new WeightedTilePrefab();
		weightedTilePrefab.tilePrefab = tilePrefab;
		weightedTilePrefab.tileWeight = tileWeight;
		elements.Add(weightedTilePrefab);

		PrecalcRandomWeights();
	}

	public void RemoveElement(GameObject tilePrefab, int tileWeight)
	{

		if (tileWeight <= 0)
		{
			return;
		}

		for (int elementId = 0; elementId < elements.Count; elementId++)
		{
			if (elements[elementId].tilePrefab == tilePrefab)
			{
				elements[elementId].tileWeight -= tileWeight;
				// if (elements[elementId].tileWeight <= 0)
				// {
				// 	elements[elementId] = elements[elements.Count - 1];
				// 	elements.RemoveAt(elements.Count - 1);
				// }
				PrecalcRandomWeights();
				return;
			}
		}

		WeightedTilePrefab newElement = new WeightedTilePrefab();
		newElement.tilePrefab = tilePrefab;
		newElement.tileWeight = -tileWeight;
		elements.Add(newElement);

		PrecalcRandomWeights();
	}

	public List<WeightedTilePrefab> GetElements()
	{
		return elements;
	}

	private void UpdateOverlay()
	{

		if (gridOverlay == null)
		{
			return;
		}

		bool isVisible = (!canInteract && cntAnimating == 0);

		gridOverlay.gameObject.SetActive(isVisible);

		if (isVisible == false)
		{
			return;
		}

		float gridWorldSize = gridSize * cellSize;
		Vector3 worldCenter = transform.position + gridPosition;

		RectTransform rt = gridOverlay;
		
		rt.position = worldCenter;
		
		rt.sizeDelta = new Vector2(gridWorldSize, gridWorldSize);
		
		rt.localScale = Vector3.one;
	}
	
	private void UpdateCntAnimating(int delta)
	{
		cntAnimating += delta;
		UpdateOverlay();
	}

	public void DecreaseLuck(int amount=1)
	{
		if (isLucky)
		{
			regenChance.numerator -= amount;
			if (regenChance.numerator < 0)
			{
				regenChance.numerator = -regenChance.numerator;
				isLucky = false;
			}
		}
		else
		{
			regenChance.numerator += amount;
		}
	}

	public void IncreaseLuck(int amount=1)
	{
		if (!isLucky)
		{
			regenChance.numerator -= amount;
			if (regenChance.numerator < 0)
			{
				regenChance.numerator = -regenChance.numerator;
				isLucky = true;
			}
		}
		else
		{
			regenChance.numerator += amount;
		}
	}

	public void RestoreLuck()
	{
		IncreaseLuck(tempLuckDecrease);
		tempLuckDecrease = 0;
	}

}

public class ElementClickHandler : MonoBehaviour
{
	public MatchManager gridManager;

	void OnMouseDown()
	{
		if (gridManager != null)
		{
			gridManager.OnElementClicked(gameObject);
		}
	}
}
