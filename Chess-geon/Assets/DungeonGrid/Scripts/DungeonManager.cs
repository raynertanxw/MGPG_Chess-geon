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

	private DungeonBlock[,] dungeonBlockGrid = null;
    public DungeonBlock[,] DungeonBlocks { get { return dungeonBlockGrid; } }
	private GameObject[,] dungeonBlockGameObjectGrid = null;
	private SpriteRenderer[,] dungeonBlockSpriteRens = null;

	//AStar
	GridManager rookGrid = null;
	GridManager bishopGrid = null;
	GridManager knightGrid = null;
	GridManager kingGrid = null;
	LinkedList<Node> testPath = null;

	void Awake()
	{
		if (sInstance != null)
			return;
		else
			sInstance = this;

		Generate();

		rookGrid = new GridManager(this, GridType.Rook);
		bishopGrid = new GridManager(this, GridType.Bishop);
		knightGrid = new GridManager(this, GridType.Knight);
		kingGrid = new GridManager(this, GridType.King);
	}

	void Start()
	{
		// ASTAR TEST!!!!!!
//		testPath = AStarManager.FindPath(rookGrid.nodes[1, 1], rookGrid.nodes[17, 27], rookGrid);
//		testPath = AStarManager.FindPath(bishopGrid.nodes[1, 1], bishopGrid.nodes[17, 27], bishopGrid);
		testPath = AStarManager.FindPath(knightGrid.nodes[1, 1], knightGrid.nodes[17, 27], knightGrid);
//		testPath = AStarManager.FindPath(kingGrid.nodes[1, 1], kingGrid.nodes[17, 27], kingGrid);
		// END OF ASTAR TEST!!!!!
	}

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

		RoomPattern[] patterns = Resources.FindObjectsOfTypeAll<RoomPattern>();
		#if UNITY_EDITOR
		if (patterns.Length == 0)
			Debug.LogError("There are no Room Patterns found.");
		#endif

		// Actually floor terrain, generated in batches of 5x5 grids.
		for (int divY = 0; divY < numYDiv; divY++)
		{
			for (int divX = 0; divX < numXDiv; divX++)
			{
				int anchorX = divXSize * divX + 1;
				int anchorY = divYSize * divY + 1;

				RoomPattern curPattern = patterns[Random.Range(0, patterns.Length)];

				for (int y = 0; y < divYSize; y++)
				{
					for (int x = 0; x < divXSize; x++)
					{
						int indexX = anchorX + x;
						int indexY = anchorY + y;
						DungeonBlock curBlock;

						curBlock = new DungeonBlock(curPattern.BlockTerrainType[y * curPattern.RoomSizeY + x], indexX, indexY);
						dungeonBlockGrid[indexX, indexY] = curBlock;
					}
				}
			}
		}
	}

	public static bool IsWhiteTile(int x, int y)
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
				}
			}
		}
	}

	private Vector3 GridPosToWorldPos(int _x, int _y)
	{
		return dungeonBlockGameObjectGrid[_x, _y].transform.position;
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;
		
		DebugDrawGrid(this.transform.position, SizeY, SizeX, blockSize, Color.cyan);

		for (int y = 0; y < SizeY; y++)
		{
			for (int x = 0; x < SizeX; x++)
			{
				if (rookGrid.nodes[x, y].State == BlockState.Obstacle)
				{
					DebugDrawSquare_AnchorCenter(GridPosToWorldPos(x, y), blockSize, Color.red);
				}
			}
		}

		if (testPath == null)
			return;
		for (LinkedListNode<Node> j = testPath.First; j.Next != null; j = j.Next)
		{
			Node node = (Node) j.Value;
			Node next = (Node) j.Next.Value;
			Debug.DrawLine(GridPosToWorldPos(node.PosX, node.PosY),
				GridPosToWorldPos(next.PosX, next.PosY),
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
}
