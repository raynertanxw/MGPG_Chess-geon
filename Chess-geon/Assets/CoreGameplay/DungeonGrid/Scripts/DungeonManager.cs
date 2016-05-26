using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private int divXSize = 5;
	private int divYSize = 5;
	[Header("Render Options")]
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
	public Sprite blackTileSprite, whiteTileSprite, wallTileSprite;

	[Header("Gameplay Options")]
	public float patternsBias = 10.0f;
	public float structuredPatternsBias = 5.0f;
	public float emptyPatternBias = 50.0f;

	public RoomPattern[] patterns;
	public RoomPattern[] structuredPatterns;
	public RoomPattern EmptyRoom;

	private DungeonBlock[,] dungeonBlockGrid = null;
    public DungeonBlock[,] DungeonBlocks { get { return dungeonBlockGrid; } }
	private GameObject[,] dungeonBlockGameObjectGrid = null;
	private SpriteRenderer[,] dungeonBlockSpriteRens = null;

	// Game Logic
	private int mnExitPosX, mnExitPosY;
	public int ExitPosX { get { return mnExitPosX; } }
	public int ExitPosY { get { return mnExitPosY; } }
	private int mnSpawnPosX, mnSpawnPosY;
	public int SpawnPosX { get { return mnSpawnPosX; } }
	public int SpawnPosY { get { return mnSpawnPosY; } }

	//AStar
	private GridManager[] mGrids = null;
	public GridManager[] Grids { get { return mGrids; } }

	void Awake()
	{
		if (sInstance != null)
			return;
		else
			sInstance = this;

		Generate();

		mGrids = new GridManager[5];

		mGrids[0] = new GridManager(this, GridType.Pawn);
		mGrids[1] = new GridManager(this, GridType.Rook);
		mGrids[2] = new GridManager(this, GridType.Bishop);
		mGrids[3] = new GridManager(this, GridType.Knight);
		mGrids[4] = new GridManager(this, GridType.King);
	}

	#region Dungeon Generation

	[ContextMenu("Generate")]
	private void Generate()
	{
		dungeonBlockGrid = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(0).gameObject);
		}
		dungeonBlockGameObjectGrid = null;
		dungeonBlockSpriteRens = null;

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

		dungeonBlockGrid = new DungeonBlock[SizeX, SizeY];
		int numXDiv = (SizeX - 2) / divXSize;
		int numYDiv = (SizeY - 2) / divYSize;

		// Fill up edges with walls.
		for (int edgeX = 0; edgeX < SizeX; edgeX++)
		{
			DungeonBlock curTopRowBlock = new DungeonBlock(TerrainType.Wall, edgeX, 0);
			dungeonBlockGrid[edgeX, 0] = curTopRowBlock;

			for (int edgeY = numYDiv * divYSize + 1; edgeY < SizeY; edgeY++)
			{
				DungeonBlock curBtmRowBlock = new DungeonBlock(TerrainType.Wall, edgeX, edgeY);
				dungeonBlockGrid[edgeX, edgeY] = curBtmRowBlock;
			}
		}
		for (int edgeY = 1; edgeY < SizeY - 1; edgeY++)
		{
			DungeonBlock curLeftColBlock = new DungeonBlock(TerrainType.Wall, 0, edgeY);
			dungeonBlockGrid[0, edgeY] = curLeftColBlock;

			for (int edgeX = numXDiv * divXSize + 1; edgeX < SizeX; edgeX++)
			{
				DungeonBlock curRightColBlock = new DungeonBlock(TerrainType.Wall, edgeX, edgeY);
				dungeonBlockGrid[edgeX, edgeY] = curRightColBlock;
			}
		}

//		RoomPattern[] patterns = Resources.FindObjectsOfTypeAll<RoomPattern>();
//		#if UNITY_EDITOR
//		Debug.Log(patterns.Length);
//		if (patterns.Length == 0)
//			Debug.LogError("There are no Room Patterns found.");
//		#endif

		// Actual floor terrain, generated in batches of 5x5 grids.
		for (int divY = 0; divY < numYDiv; divY++)
		{
			for (int divX = 0; divX < numXDiv; divX++)
			{
				int anchorX = divXSize * divX + 1;
				int anchorY = divYSize * divY + 1;

				RoomPattern curPattern = GetRandPattern();

				for (int y = 0; y < divYSize; y++)
				{
					for (int x = 0; x < divXSize; x++)
					{
						int indexX = anchorX + x;
						int indexY = anchorY + y;
						DungeonBlock curBlock;

						TerrainType terrainType = curPattern.BlockTerrainType[y * curPattern.SizeY + x];
						if (terrainType == TerrainType.Wall)
							if ((Random.value - curPattern.MatchPercentage) > 0.0f)
								terrainType = TerrainType.Tile;

						curBlock = new DungeonBlock(terrainType, indexX, indexY);
						dungeonBlockGrid[indexX, indexY] = curBlock;
					}
				}
			}
		}

		// Set the EndTile.
		mnExitPosX = divXSize * (numXDiv - 1) + 1;
		mnExitPosY = divYSize * (numYDiv - 1) + 1;
		mnExitPosX += divXSize / 2;
		mnExitPosY += divYSize / 2;
		dungeonBlockGrid[ExitPosX, ExitPosY] = new DungeonBlock(TerrainType.Stairs, ExitPosX, ExitPosY);
		SetSurroundingTilesToEmpty(ExitPosX, ExitPosY);

		// Set the PlayerSpawnPosition
		mnSpawnPosX = divXSize / 2 + 1;
		mnSpawnPosY = divYSize / 2 + 1;
		dungeonBlockGrid[SpawnPosX, SpawnPosY] = new DungeonBlock(TerrainType.Spawn, SpawnPosX, SpawnPosY);
		SetSurroundingTilesToEmpty(SpawnPosX, SpawnPosY);
	}

	private RoomPattern GetRandPattern()
	{
		float totalBias = patternsBias + structuredPatternsBias + emptyPatternBias;
		float rangeCheck = patternsBias;
		float randNum = Random.Range(0.0f, totalBias);
		if (randNum < rangeCheck)
		{
			return patterns[Random.Range(0, patterns.Length)];
		}
		rangeCheck += structuredPatternsBias;
		if (randNum < rangeCheck)
		{
			return structuredPatterns[Random.Range(0, structuredPatterns.Length)];
		}
		return EmptyRoom;
	}

	private void SetSurroundingTilesToEmpty(int _posX, int _posY)
	{
		if (_posX < 1 || _posX > SizeX - 2 || _posY < 1 || _posY > SizeY - 2)
		{
			Debug.LogError("Tile (" + _posX + ", " + _posY + ") is too near the edge for surrounding tiles to be set to empty.");
			return;
		}

		// Btm-Left
		dungeonBlockGrid[_posX - 1, _posY - 1].SetBlockState(BlockState.Empty);
		// Left
		dungeonBlockGrid[_posX - 1, _posY].SetBlockState(BlockState.Empty);
		// Top-Left
		dungeonBlockGrid[_posX - 1, _posY + 1].SetBlockState(BlockState.Empty);
		// Top
		dungeonBlockGrid[_posX, _posY + 1].SetBlockState(BlockState.Empty);
		// Top-Right
		dungeonBlockGrid[_posX + 1, _posY + 1].SetBlockState(BlockState.Empty);
		// Right
		dungeonBlockGrid[_posX + 1, _posY].SetBlockState(BlockState.Empty);
		// Btm-Right
		dungeonBlockGrid[_posX + 1, _posY - 1].SetBlockState(BlockState.Empty);
		// Btm
		dungeonBlockGrid[_posX, _posY - 1].SetBlockState(BlockState.Empty);
	}

	private void CreateDungeonGameObjects()
	{
		if (dungeonBlockGameObjectGrid != null || dungeonBlockSpriteRens != null)
		{
			Debug.LogWarning("DungeonBlockGameObjectGrid or dungeonBlockSpriteRens is already created.");
			return;
		}

		dungeonBlockGameObjectGrid	= new GameObject[SizeX, SizeY];
		dungeonBlockSpriteRens		= new SpriteRenderer[SizeX, SizeY];
		for (int y = 0; y < SizeY; y++)
		{
			for (int x = 0; x < SizeX; x++)
			{
				Vector3 curBlockPos = transform.position;
				curBlockPos.x += halfBlockSize + (x * blockSize);
				curBlockPos.y += halfBlockSize + (y * blockSize);
				GameObject curBlock = (GameObject) Instantiate(dungeonBlockPrefab, curBlockPos, Quaternion.identity);
				curBlock.transform.localScale = Vector3.one * scaleMultiplier;
				dungeonBlockGameObjectGrid[x, y] = curBlock;
				curBlock.transform.SetParent(this.transform);

				dungeonBlockSpriteRens[x, y] = curBlock.GetComponent<SpriteRenderer>();
				switch (dungeonBlockGrid[x, y].Terrain)
				{
				case TerrainType.Wall:
					dungeonBlockSpriteRens[x, y].sprite = wallTileSprite;
					break;
				case TerrainType.Tile:
					if (IsWhiteTile(x, y))	// White Tile
						dungeonBlockSpriteRens[x, y].sprite = whiteTileSprite;
					else	// Black Tile
						dungeonBlockSpriteRens[x, y].sprite = blackTileSprite;
					break;
				case TerrainType.Stairs:
					dungeonBlockSpriteRens[x, y].sprite = whiteTileSprite;
					dungeonBlockSpriteRens[x, y].color = Color.green;
					break;
				case TerrainType.Spawn:
					dungeonBlockSpriteRens[x, y].sprite = whiteTileSprite;
					dungeonBlockSpriteRens[x, y].color = Color.yellow;
					break;
				}
			}
		}
	}

	#endregion

	public Vector3 GridPosToWorldPos(int _x, int _y)
	{
		return dungeonBlockGameObjectGrid[_x, _y].transform.position;
	}

	public bool IsWhiteTile(int x, int y)
	{
		// White is even-even, odd-odd. Black is even-odd, odd-even.
		if (x % 2 == y % 2)
			return true;
		else
			return false;
	}

	public void PlaceEnemy(int _posX, int _posY)
	{
		if (dungeonBlockGrid[_posX, _posY].State != BlockState.Empty)
		{
			Debug.LogError("Unable to place Enemy. (" + _posX + ", " + _posY + ") is not empty.");
			return;
		}

		dungeonBlockGrid[_posX, _posY].SetBlockState(BlockState.EnemyPiece);
	}

	public void RemoveEnemy(int _posX, int _posY)
	{
		if (dungeonBlockGrid[_posX, _posY].State != BlockState.EnemyPiece)
		{
			Debug.LogError("Unable to remove Enemy. (" + _posX + ", " + _posY + ") does not have an enemy piece on it.");
			return;
		}

		dungeonBlockGrid[_posX, _posY].SetBlockState(BlockState.Empty);
	}

	public void MoveEnemy(int _fromX, int _fromY, int _toX, int _toY)
	{
		PlaceEnemy(_toX, _toY);
		RemoveEnemy(_fromX, _fromY);
	}

	public bool IsValidCell(int _posX, int _posY)
	{
		if (_posX < 0 || _posY < 0 || _posX >= SizeX || _posY >= SizeY)
		{
			return false;
		}

		return true;
	}

	public bool IsCellEmpty(int _posX, int _posY)
	{
		if (IsValidCell(_posX, _posY) == false)
			return false;

		if (dungeonBlockGrid[_posX, _posY].State == BlockState.Empty)
			return true;
		else
			return false;
	}

	public bool IsPlayerPos(int _posX, int _posY)
	{
		if (_posX != GameManager.Instance.Player.PosX)
			return false;

		if (_posY != GameManager.Instance.Player.PosY)
			return false;

		return true;
	}

	#region Debug Tools

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;
		
		DebugDrawGrid(this.transform.position, SizeY, SizeX, blockSize, Color.cyan);

		for (int y = 0; y < SizeY; y++)
		{
			for (int x = 0; x < SizeX; x++)
			{
				if (dungeonBlockGrid[x, y].State != BlockState.Empty)
				{
					DebugDrawSquare_AnchorCenter(GridPosToWorldPos(x, y), blockSize, Color.red);
				}
			}
		}
	}

	public static void DrawPath(LinkedList<Node> _path)
	{
		if (_path == null)
			return;
		for (LinkedListNode<Node> j = _path.First; j.Next != null; j = j.Next)
		{
			Node node = (Node) j.Value;
			Node next = (Node) j.Next.Value;
			Debug.DrawLine(Instance.GridPosToWorldPos(node.PosX, node.PosY),
				Instance.GridPosToWorldPos(next.PosX, next.PosY),
				Color.magenta);
		}
	}

	private void DebugDrawSquare_AnchorCenter(Vector3 origin, float cellsize, Color color)
	{
		float halfSize = cellsize / 2.0f;
		// v0, v1, v2, v3 (btm-left, btm-right, top-right, top-left).
		Vector3 v0 = new Vector3(origin.x - halfSize, origin.y - halfSize, origin.z);
		Vector3 v1 = new Vector3(origin.x + halfSize, origin.y - halfSize, origin.z);
		Vector3 v2 = new Vector3(origin.x + halfSize, origin.y + halfSize, origin.z);
		Vector3 v3 = new Vector3(origin.x - halfSize, origin.y + halfSize, origin.z);

		Debug.DrawLine(v0, v1, color);
		Debug.DrawLine(v1, v2, color);
		Debug.DrawLine(v2, v3, color);
		Debug.DrawLine(v3, v0, color);

	}

	private void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
	{
		float width = (numCols * cellSize);
		float height = (numRows * cellSize);

		// Draw the horizontal grid lines
		for (int i = 0; i < numRows + 1; i++)
		{
			Vector3 startPos = origin + i * cellSize * Vector3.up;
			Vector3 endPos = startPos + width * Vector3.right;
			Debug.DrawLine(startPos, endPos, color);
		}

		// Draw the vertical grid lines
		for (int i = 0; i < numCols + 1; i++)
		{
			Vector3 startPos = origin + i * cellSize * Vector3.right;
			Vector3 endPos = startPos + height * Vector3.up;
			Debug.DrawLine(startPos, endPos, color);
		}
	}

	#endregion
}
