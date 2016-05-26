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
	public GameObject PlayerPrefab;

	private GamePhase mPhase;
	public GamePhase Phase { get { return mPhase; } }
	public List<EnemyPiece> mEnemyList;
	private PlayerPiece mPlayerPiece;
	public PlayerPiece Player { get { return mPlayerPiece; } }

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
		mEnemyList = new List<EnemyPiece>();

		GenerateNPlaceEnemies();
		PlacePlayer();

		// TODO: Draw One More Card.

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

	#region Phase Functions
	private void EnterPlayerPhase()
	{

	}

	private void ExitPlayerPhase()
	{
		
	}

	private void ExecutePlayerPhase()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SwitchPhase(GamePhase.EnemyPhase);
		}
	}
	
	private void EnterEnemyPhase()
	{
		
	}

	private void ExitEnemyPhase()
	{
		
	}

	private void ExeucteEnemyPhase()
	{
		for (int i = 0; i < mEnemyList.Count; i++)
		{
			mEnemyList[i].ExecuteTurn();
		}

		// End of enumeration.
		SwitchPhase(GamePhase.PlayerPhase);
	}

	private void SwitchPhase(GamePhase _toPhase)
	{
		switch (_toPhase)
		{
		case GamePhase.PlayerPhase:
			ExitEnemyPhase();
//			Debug.Log("Switching from Enemy to Player");
			mPhase = _toPhase;
			EnterPlayerPhase();
			break;
		case GamePhase.EnemyPhase:
			ExitPlayerPhase();
//			Debug.Log("Switching from Enemy to Player");
			mPhase = _toPhase;
			EnterEnemyPhase();
			break;
		}
	}
	#endregion

	private void GenerateNPlaceEnemies()
	{
		// TODO: TEMP IMPLEMENTATION.

		int numEnemiesSpawned = 0;
		while (numEnemiesSpawned < 5)
		{
			int posX = Random.Range(1, DungeonManager.Instance.SizeX - 2);
			int posY = Random.Range(1, DungeonManager.Instance.SizeY - 2);

			if (DungeonManager.Instance.IsCellEmpty(posX, posY))
			{
				SpawnEnemy(posX, posY, EnemyUnit.BlackBishop);
				numEnemiesSpawned++;
			}
		}
	}

	private void PlacePlayer()
	{
		// TODO: Load player health from previous level if any.

		mPlayerPiece = ((GameObject)Instantiate(
			PlayerPrefab,
			DungeonManager.Instance.GridPosToWorldPos(DungeonManager.Instance.SpawnPosX, DungeonManager.Instance.SpawnPosY),
			Quaternion.identity)).GetComponent<PlayerPiece>();
		mPlayerPiece.SetPosition(DungeonManager.Instance.SpawnPosX, DungeonManager.Instance.SpawnPosY);
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
			mEnemyList.Add(curPiece);
	}
}
