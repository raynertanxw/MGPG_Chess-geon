using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GamePhase { PlayerPhase, EnemyPhase };

public class GameManager : MonoBehaviour
{
	private static GameManager sInstance = null;
	public static GameManager Instance { get { return sInstance; } }

	private void OnDestroy()
	{
		if (this == sInstance)
			sInstance = null;
	}



	private enum EnemyUnit
	{
		BlackPawn, BlackRook, BlackBishop, BlackKnight, BlackKing
	};
	public GameObject[] EnemyUnitPrefabs;

	private GamePhase mPhase;
	public GamePhase Phase { get { return mPhase; } }
	public LinkedList<EnemyPiece> mEnemyList;

	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			Setup();
		}
		else if (sInstance != this)
		{
			Destroy(this.gameObject);
			return;
		}
	}

	private void Setup()
	{
		// Variable setups
		mEnemyList = new LinkedList<EnemyPiece>();

		// Generate all the enemies and place them.
		GenerateNPlaceEnemies();

		// TODO: Place the player piece.

		// Draw One More Card.

		mPhase = GamePhase.PlayerPhase;
	}
	
	private void Update()
	{
		switch (mPhase)
		{
		case GamePhase.PlayerPhase:
			ExecutePlayerPhase();
			break;
		case GamePhase.EnemyPhase:
			ExeucteEnemyPhase();
			break;
		}
	}

	private void ExecutePlayerPhase()
	{

	}

	private void ExeucteEnemyPhase()
	{

	}

	private void GenerateNPlaceEnemies()
	{
		// TODO: TEMP IMPLEMENTATION.

		int numEnemiesSpawned = 0;
		while (numEnemiesSpawned < 10)
		{
			int posX = Random.Range(1, DungeonManager.Instance.SizeX - 2);
			int posY = Random.Range(1, DungeonManager.Instance.SizeY - 2);

			if (DungeonManager.Instance.IsCellEmpty(posX, posY))
			{
				SpawnEnemy(posX, posY, EnemyUnit.BlackKnight);
				numEnemiesSpawned++;
			}
		}
	}

	private void SpawnEnemy(int _posX, int _posY, EnemyUnit enemyType)
	{
		EnemyPiece curPiece = null;

		switch (enemyType)
		{
		case EnemyUnit.BlackPawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Black);
			break;
		case EnemyUnit.BlackRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Black);
			break;
		case EnemyUnit.BlackBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Black);
			break;
		case EnemyUnit.BlackKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Black);
			break;
		case EnemyUnit.BlackKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Black);
			break;
		default:
			Debug.LogError("No enemyType handling found");
			break;
		}

		if (curPiece != null)
			mEnemyList.AddFirst(curPiece);
	}
}
