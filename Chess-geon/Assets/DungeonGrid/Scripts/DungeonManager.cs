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
    public DungeonBlock[,] DungeonBlocks { get { return dungeonBlockGrid; } }
	private GameObject[,] dungeonBlockGameObjectGrid = null;
	private SpriteRenderer[,] dungeonBlockSpriteRens = null;

	//AStar
	GridManager rookGrid = null;
	GridManager bishopGrid = null;
	LinkedList<Node> testPath = null;
	public GameObject testMarker;

	void Awake()
	{
		if (sInstance != null)
			return;
		else
			sInstance = this;

		Generate();

		rookGrid = new GridManager(this, GridType.Rook);
		bishopGrid = new GridManager(this, GridType.Bishop);
	}

	void Start()
	{
		// ASTAR TEST!!!!!!
//		testPath = AStarManager.FindPath(rookGrid.nodes[1, 1], rookGrid.nodes[16, 27], rookGrid);
		testPath = AStarManager.FindPath(bishopGrid.nodes[1, 1], bishopGrid.nodes[17, 27], bishopGrid);
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

	private Vector3 GridPosToWorldPos(int _x, int _y)
	{
		return dungeonBlockGameObjectGrid[_x, _y].transform.position;
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;
		
		DebugDrawGrid(this.transform.position, SizeY, sizeX, blockSize, Color.cyan);

		for (int y = 0; y < SizeY; y++)
		{
			for (int x = 0; x < SizeX; x++)
			{
				if (rookGrid.nodes[x, y].State == BlockState.Wall)
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
