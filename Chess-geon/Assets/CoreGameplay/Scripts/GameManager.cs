using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaburuTools.Action;
using DaburuTools;

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

	private BehaviourTree mPlayerPhaseBehaviourTree;
	private BehaviourTree mEnemyPhaseBehaviourTree;
	private GamePhase mPhase;
	public GamePhase Phase { get { return mPhase; } }
	private bool mPlayerToEndPhase;
	private bool mbIsGameOver;
	public bool IsGameOver { get { return mbIsGameOver; } }

	private List<EnemyPiece> mEnemyList;
	public List<EnemyPiece> EnemyList { get { return mEnemyList; } }
	private PlayerPiece mPlayerPiece;
	public PlayerPiece Player { get { return mPlayerPiece; } }

	private ControlAreaManager mCtrlArea;

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
		mPlayerToEndPhase = false;
		InitializePlayerPhaseBehaviourTree();
		InitializeEnemyPhaseBehaviourTree();

		mCtrlArea = GameObject.Find("ControlAreaCanvas").GetComponent<ControlAreaManager>();

		GenerateNPlaceEnemies();
		PlacePlayer();

		mPhase = GamePhase.PlayerPhase;
		mbIsGameOver = false;
	}

	void Start()
	{
		// Draw 3 cards.
		DelayAction firstDraw = new DelayAction(1.8f);
		firstDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		DelayAction secondDraw = new DelayAction(1.8f + 0.3f);
		secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		DelayAction thirdDraw = new DelayAction(1.8f + 0.3f + 0.3f);
		secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		ActionHandler.RunAction(firstDraw, secondDraw, thirdDraw);
	}

	private void InitializePlayerPhaseBehaviourTree()
	{
		BTAction BTAct_CheckEndTurn = new BTAction(
			() =>
			{
				if (mPlayerToEndPhase)
				{
					SwitchPhase(GamePhase.EnemyPhase);
					return BTStatus.Running;
				}
				return BTStatus.Success;
			}
		);
		BTAction BTAct_MoveCard = new BTAction(
			() =>
			{
				if (ControlAreaManager.CardIsBeingDragged)
				{
					return BTStatus.Running;
				}

				return BTStatus.Success;
			}
		);
		BTAction BTAct_ExecuteCard = new BTAction(
			() =>
			{
				if (ControlAreaManager.IsPanelOpen)
				{
					mCtrlArea.SetControlBlockerEnabled(true);
					return BTStatus.Running;
				}
				// Check player turn status.
				if (Player.TurnStatus == PlayerTurnStatus.Running)
				{
					mCtrlArea.SetControlBlockerEnabled(true);
					return BTStatus.Running;
				}

				if (EventAnimationController.Instance.IsAnimating == false)
					ControlAreaManager.Instance.SetControlBlockerEnabled(false);

				return BTStatus.Success;
			}
		);
		BTSequence BT_Root = new BTSequence(BTAct_CheckEndTurn, BTAct_MoveCard, BTAct_ExecuteCard);

		mPlayerPhaseBehaviourTree = new BehaviourTree(BT_Root);
	}

	// Vairables only for EnemyPhase. Prefixed with EP.
	private int EPcurEnemyIndex = 0;
	private bool EPdoneStartAnimation = false;
	private void InitializeEnemyPhaseBehaviourTree()
	{
		// Returns fail when there are enemies. Success if no enemies found.
		BTAction BTAct_NoEnemyCheck = new BTAction(
			() =>
			{
				// If there are not even one enemy. Nothing to do here.
				if (EnemyList.Count < 1)
				{
					EndEnemyPhase();
					return BTStatus.Success;
				}

				return BTStatus.Failure;
			}
		);

		BTAction BTAct_EnemyPhaseStartAnimations = new BTAction(
			() =>
			{
				if (EPdoneStartAnimation == false)
				{
					EventAnimationController.Instance.ExecutePhaseAnimation(GamePhase.EnemyPhase);
					EPdoneStartAnimation = true;
					return BTStatus.Running;
				}
				else if (EventAnimationController.Instance.IsAnimating == false)
				{
					EPdoneStartAnimation = false;
					ControlAreaManager.Instance.SetControlBlockerEnabled(true);
					return BTStatus.Success;
				}

				return BTStatus.Running;
			}
		);

		BTAction BTAct_MoveEnemyPieces = new BTAction(
			() =>
			{
				EnemyPiece curEnemy = null;

				if (EPcurEnemyIndex < EnemyList.Count)
					curEnemy = EnemyList[EPcurEnemyIndex];

				switch (curEnemy.TurnStatus)
				{
				case EnemyTurnStatus.Unprocessed:
					curEnemy.ExecuteTurn();
					break;
				case EnemyTurnStatus.Running:
					return BTStatus.Running;	// Do nothing, just let it run.
				default:	// Both processed and waiting, move on to the next EnemyPiece.
					EPcurEnemyIndex++;
					if (EPcurEnemyIndex >= EnemyList.Count)	// If went through the whole thing once already.
					{
						EPcurEnemyIndex = 0;
						BoardScroller.Instance.FocusCameraToPos(
							DungeonManager.Instance.GridPosToWorldPos(GameManager.Instance.Player.PosX, GameManager.Instance.Player.PosY),
							0.2f,
							Graph.InverseExponential);
						return BTStatus.Success;
					}
					break;
				}

				return BTStatus.Running;
			}
		);
		BTAction BTAct_AttackPlayer = new BTAction(
			() =>
			{
				EnemyPiece curEnemy = null;

				if (EPcurEnemyIndex < EnemyList.Count)
					curEnemy = EnemyList[EPcurEnemyIndex];

				switch (curEnemy.TurnStatus)
				{
				case EnemyTurnStatus.Waiting:
					curEnemy.ExecuteTurn();
					break;
				case EnemyTurnStatus.Running:
					return BTStatus.Running;	// Do nothing, just let it run.
				default:	// Move on to the next one. Only search for those that are Waiting status.
					EPcurEnemyIndex++;
					if (EPcurEnemyIndex >= EnemyList.Count)	// If went finish second pass.
					{
						EndEnemyPhase();
						return BTStatus.Success;
					}
					break;
				}

				return BTStatus.Running;
			}
		);
		// BT_Sequence only runs when there are enemy pieces.
		// Refer to BT_NullCheckSelector below.
		BTSequence BT_EnemyMovementSequence = new BTSequence(BTAct_MoveEnemyPieces, BTAct_AttackPlayer);
		// Root is Selector that checks for enemies.
		// If no enemies, immediately stops (BTAct_NoEnemyCheck returns success).
		BTSelector BT_NullCheckSelector = new BTSelector(BTAct_NoEnemyCheck, BT_EnemyMovementSequence);
		BTSequence BT_Root = new BTSequence(BTAct_EnemyPhaseStartAnimations, BT_NullCheckSelector);

		mEnemyPhaseBehaviourTree = new BehaviourTree(BT_Root);
	}
	
	private void Update()
	{
		if (IsGameOver)
		{
			return;
		}

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
		// Reset variable to make sure player can end phase again.
		mPlayerToEndPhase = false;
		// mCtrlArea.SetControlBlockerEnabled(false) is now done by the phase animation below.
		EventAnimationController.Instance.ExecutePhaseAnimation(GamePhase.PlayerPhase);

		DelayAction firstDraw = new DelayAction(1.8f);
		firstDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		DelayAction secondDraw = new DelayAction(1.8f + 0.5f);
		secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		ActionHandler.RunAction(firstDraw, secondDraw);
	}
	
	private void ExitPlayerPhase()
	{
		mCtrlArea.SetControlBlockerEnabled(true);
	}

	private void ExecutePlayerPhase()
	{
		// TODO: Draw card

		mPlayerPhaseBehaviourTree.Tick();
	}

	public void EndPlayerPhase()
	{
		if (mPlayerToEndPhase)
		{
			Debug.LogWarning("mPlayerToEndPhase is already set to true");
		}

		mPlayerToEndPhase = true;
	}
	
	private void EnterEnemyPhase()
	{
		
	}

	private void ExitEnemyPhase()
	{
		
	}

	private void ExeucteEnemyPhase()
	{
		mEnemyPhaseBehaviourTree.Tick();
	}

	private void EndEnemyPhase()
	{
		EPcurEnemyIndex = 0;
		for (int i = 0; i < EnemyList.Count; i++)
		{
			EnemyList[i].ResetTurnStatus();
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
				EnemyList.Add(EnemyPiece.Spawn(posX, posY, EnemyUnit.BlackKing));
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
		mPlayerPiece.Initialise(DungeonManager.Instance.SpawnPosX, DungeonManager.Instance.SpawnPosY);
	}

	public void PlayerDied()
	{
		mbIsGameOver = true;
		EventAnimationController.Instance.ShowGameOver();
	}

	public void ReachedFloorExit()
	{
		// TODO: Implement next level.
	}
}
