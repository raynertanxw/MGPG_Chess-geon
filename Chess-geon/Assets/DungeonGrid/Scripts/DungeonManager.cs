using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
	private static DungeonManager sInstance = null;
	public static DungeonManager Instance { get { return sInstance; } }

	void OnDestroy()
	{
		// Only set sInstance to null if the actual instance is destroyed.
		if (sInstance == this)
			sInstance = null;
	}

	[SerializeField] private int sizeX = 32;
	[SerializeField] private int sizeY = 32;
	public int SizeX { get { return sizeX; } }
	public int SizeY { get { return sizeY; } }
	[SerializeField] private int scaleMultiplier = 1;
	public int ScaleMultiplier { get { return scaleMultiplier; } }
	private float blockSize;
	public float BlockSize { get { return blockSize; } }
	private float halfBlockSize;
	public GameObject dungeonBlockPrefab;
	public Sprite blackTileSprite, whiteTileSprite, selectableTileSprite, wallTileSprite;

	private DungeonBlock[,] dungeonBlockGrid = null;
	private GameObject[,] dungeonBlockGameObjectGrid = null;
	private SpriteRenderer[,] dungeonBlockSpriteRens = null;

	void Awake()
	{
		if (sInstance != null)
			return;
		else
			sInstance = this;

		blockSize = blackTileSprite.rect.xMax / 100.0f * scaleMultiplier;
		halfBlockSize = blockSize / 2.0f;

		CreateDungeonBlocks();
		CreateDungeonGameObjects();
	}

	private void CreateDungeonBlocks()
	{
		if (dungeonBlockGrid != null)
		{
			Debug.LogWarning("dungeonBlockGrid is already created.");
			return;
		}

		dungeonBlockGrid = new DungeonBlock[sizeX, sizeY];
		for (int y = 0; y < sizeY; y++)
		{
			for (int x = 0; x < sizeX; x++)
			{
				DungeonBlock curBlock;

				// Edge Cases
				if (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1)
				{
					curBlock = new DungeonBlock(BlockState.Wall, x, y);
					dungeonBlockGrid[x, y] = curBlock;
					continue;
				}

				if (Random.Range(0.0f, 1.0f) < 0.1f)
				{
					curBlock = new DungeonBlock(BlockState.Wall, x , y);
					dungeonBlockGrid[x, y] = curBlock;
					continue;
				}

				curBlock = new DungeonBlock(BlockState.Empty, x, y);
				dungeonBlockGrid[x, y] = curBlock;
			}
		}
	}

	private bool IsWhiteTile(int x, int y)
	{
		// White is even-even, odd-odd. Black is even-odd, odd-even.
		if (x % 2 == y % 2)
			return true;
		else
			return false;
	}

	private void CreateDungeonGameObjects()
	{
		if (dungeonBlockGameObjectGrid != null || dungeonBlockSpriteRens != null)
		{
			Debug.LogWarning("DungeonBlockGameObjectGrid or dungeonBlockSpriteRens is already created.");
			return;
		}

		dungeonBlockGameObjectGrid	= new GameObject[sizeX, sizeY];
		dungeonBlockSpriteRens		= new SpriteRenderer[sizeX, sizeY];
		for (int y = 0; y < sizeY; y++)
		{
			for (int x = 0; x < sizeX; x++)
			{
				Vector3 curBlockPos = transform.position;
				curBlockPos.x += halfBlockSize + (x * blockSize);
				curBlockPos.y += halfBlockSize + (y * blockSize);
				GameObject curBlock = (GameObject) Instantiate(dungeonBlockPrefab, curBlockPos, Quaternion.identity);
				curBlock.transform.localScale = Vector3.one * scaleMultiplier;
				dungeonBlockGameObjectGrid[x, y] = curBlock;
				curBlock.transform.SetParent(this.transform);

				dungeonBlockSpriteRens[x, y] = curBlock.GetComponent<SpriteRenderer>();
				switch (dungeonBlockGrid[x, y].State)
				{
				case BlockState.Wall:
					dungeonBlockSpriteRens[x, y].sprite = wallTileSprite;
					break;
				case BlockState.Selectable:
					dungeonBlockSpriteRens[x, y].sprite = selectableTileSprite;
					break;
				default:
					if (IsWhiteTile(x, y))	// White Tile
						dungeonBlockSpriteRens[x, y].sprite = whiteTileSprite;
					else	// Black Tile
						dungeonBlockSpriteRens[x, y].sprite = blackTileSprite;
					break;
				}
			}
		}
	}
}
