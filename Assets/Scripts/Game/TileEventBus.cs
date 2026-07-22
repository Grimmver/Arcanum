using System;

public static class TileEventBus
{
	public static event Func<Tile, int, int> OnPreTileMatchedModify;
	public static event Action<Tile, int> OnTileMatched;
	public static event Action<Tile> OnTileDestroyed;
	public static event Action<Tile> OnTileDropped;
	public static event Action<Tile> OnTileSpawned;
	public static event Action OnTileMatchingProcessed;

	public static int ModifyMatchCount(Tile tile, int baseCount)
	{
		int modified = baseCount;

		if (OnPreTileMatchedModify != null)
		{
			foreach (Func<Tile, int, int> modifier in OnPreTileMatchedModify.GetInvocationList())
			{
				modified = modifier.Invoke(tile, modified);
			}
		}

		return modified;
	}

	public static void TileMatched(Tile tile, int matchCount)
	{
		OnTileMatched?.Invoke(tile, matchCount);
	}

	public static void TileDestroyed(Tile tile) => OnTileDestroyed?.Invoke(tile);
	public static void TileDropped(Tile tile) => OnTileDropped?.Invoke(tile);
	public static void TileSpawned(Tile tile) => OnTileSpawned?.Invoke(tile);
	public static void TileMatchingProcessed() => OnTileMatchingProcessed?.Invoke();
}
