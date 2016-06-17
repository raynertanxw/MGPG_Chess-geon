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

	// Vairables only for EnemyPhase. Prefixed with EP.
	private int EPcurEnemyIndex = 0;
	private EnemyTurnStatus EPprocessingMode = EnemyTurnStatus.Unprocessed;
	private void ExeucteEnemyPhase()
	{
		// If there are not even one enemy. Nothing to do here.
		if (mEnemyList.Count < 1)
		{
			EndEnemyPhase();
			return;
		}

		EnemyPiece curEnemy = null;

		if (EPcurEnemyIndex < mEnemyList.Count)
			curEnemy = mEnemyList[EPcurEnemyIndex];

		if (EPprocessingMode == EnemyTurnStatus.Unprocessed)
		{
			switch (curEnemy.TurnStatus)
			{
			case EnemyTurnStatus.Unprocessed:
				curEnemy.ExecuteTurn();
				break;
			case EnemyTurnStatus.Running:
				return;	// Do nothing, just let it run.
			default:	// Both processed and waiting, move on to the next EnemyPiece.
				EPcurEnemyIndex++;
				if (EPcurEnemyIndex >= mEnemyList.Count)	// If went through the whole thing once already.
				{
					EPcurEnemyIndex = 0;
					EPprocessingMode = EnemyTurnStatus.Waiting;
				}
				break;
			}
		}
		else if (EPprocessingMode == EnemyTurnStatus.Waiting)
		{
			switch (curEnemy.TurnStatus)
			{
			case EnemyTurnStatus.Waiting:
				curEnemy.ExecuteTurn();
				break;
			case EnemyTurnStatus.Running:
				return;	// Do nothing, just let it run.
			default:	// Move on to the next one. Only search for those that are Waiting status.
				EPcurEnemyIndex++;
				if (EPcurEnemyIndex >= mEnemyList.Count)	// If went finish second pass.
					EndEnemyPhase();
				break;
			}
		}
		else
		{
			Debug.LogError("Invalid EPprocessingMode");
		}
	}

	private void EndEnemyPhase()
	{
		EPcurEnemyIndex = 0;
		EPprocessingMode = EnemyTurnStatus.Unprocessed;
		for (int i = 0; i < mEnemyList.Count; i++)
		{
			mEnemyList[i].ResetTurnStatus();
		}
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
		while (numEnemiesSpawned < 8)
		{
			int posX = Random.Range(1, DungeonManager.Instance.SizeX - 2);
			int posY = Random.Range(1, DungeonManager.Instance.SizeY - 2);

			if (DungeonManager.Instance.IsCellEmpty(posX, posY))
			{
				mEnemyList.Add(EnemyPiece.Spawn(posX, posY, EnemyUnit.BlackKing));
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
}
