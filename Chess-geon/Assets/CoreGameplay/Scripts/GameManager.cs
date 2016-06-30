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
	public GameObject EnemyPrefab;

	private BehaviourTree mPlayerPhaseBehaviourTree;
	private BehaviourTree mEnemyPhaseBehaviourTree;
	private GamePhase mPhase;
	public GamePhase Phase { get { return mPhase; } }
	private bool mPlayerToEndPhase;
	private bool mbIsGameOver;
	public bool IsGameOver { get { return mbIsGameOver; } }
	private bool mbGameStarted;
	private int mnFloorNumber = 0;
	public int FloorNumber { get { return mnFloorNumber; } }
	private int mnScore;
	public int Score { get { return mnScore; } }
	public void AddScore(int _score)
	{
		mnScore += (int) (_score * mfFloorScoreMultiplier);
		ScorePanelManager.Instance.UpdateScore(mnScore);
	}
	private float mfFloorScoreMultiplier;

	private List<EnemyPiece> mEnemyList;
	public List<EnemyPiece> EnemyList { get { return mEnemyList; } }
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
		// Setup Floor number.
		if (PlayerPrefs.HasKey(Constants.kStrFloorNumber))
		{
			mnFloorNumber = PlayerPrefs.GetInt(Constants.kStrFloorNumber);
		}
		else
		{
			mnFloorNumber = 0;
			PlayerPrefs.SetInt(Constants.kStrFloorNumber, mnFloorNumber);
		}
		mfFloorScoreMultiplier = 1.0f + (0.1f * mnFloorNumber);

		// Setup Score (if any).
		if (PlayerPrefs.HasKey(Constants.kStrCurScore))
		{
			mnScore = PlayerPrefs.GetInt(Constants.kStrCurScore);
		}
		else
		{
			mnScore = 0;
			PlayerPrefs.SetInt(Constants.kStrCurScore, mnScore);
		}
		
		// Variable setups
		mEnemyList = new List<EnemyPiece>();
		mPlayerToEndPhase = false;
		InitializePlayerPhaseBehaviourTree();
		InitializeEnemyPhaseBehaviourTree();

		PlacePlayer();
		GenerateNPlaceEnemies();

		mPhase = GamePhase.PlayerPhase;
		mbIsGameOver = false;
		mbGameStarted = false;
	}

	void Start()
	{
		// Draw 3 cards.
		ActionParallel drawParallel = new ActionParallel();
		if (PlayerPrefs.HasKey(Constants.kStrNumCardsInHand))
		{
			int numCards = PlayerPrefs.GetInt(Constants.kStrNumCardsInHand);
			for (int i = 0; i < numCards; i++)
			{
				DelayAction draw = new DelayAction(i * 0.3f);

				CardType type = (CardType)PlayerPrefs.GetInt(Constants.kStrCardType[i]);
				if (type == CardType.Movement)
				{
					GridType moveType = (GridType)PlayerPrefs.GetInt(Constants.kStrCardMoveType[i]);
					draw.OnActionFinish += () => {
						DeckManager.Instance.DrawCard(moveType);
					};
				}
				else
				{
					CardTier tier = (CardTier) PlayerPrefs.GetInt(Constants.kStrCardTier[i]);
					draw.OnActionFinish += () => {
						DeckManager.Instance.DrawSpecificCard(type, tier);
					};
				}

				drawParallel.Add(draw);
			}
		}
		else
		{
			DelayAction firstDraw = new DelayAction(0.1f);
			firstDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
			DelayAction secondDraw = new DelayAction(0.1f + 0.3f);
			secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
			DelayAction thirdDraw = new DelayAction(0.1f + 0.3f + 0.3f);
			secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
			drawParallel = new ActionParallel(firstDraw, secondDraw, thirdDraw);
		}

		Vector3 vec3PlayerPos = DungeonManager.Instance.GridPosToWorldPos(Player.PosX, Player.PosY);
		Vector3 vec3ExitPos = DungeonManager.Instance.GridPosToWorldPos(DungeonManager.Instance.ExitPosX, DungeonManager.Instance.ExitPosY);
		DelayAction pan = new DelayAction(2.0f);
		pan.OnActionStart += () => {
			BoardScroller.Instance.FocusCameraToPos(vec3ExitPos, 1.5f, Graph.SmoothStep);
		};
		pan.OnActionFinish += () => {
			BoardScroller.Instance.FocusCameraToPos(vec3PlayerPos, 0.5f, Graph.SmoothStep);
		};

		DelayAction phaseAnimDelay = new DelayAction(0.6f);
		phaseAnimDelay.OnActionFinish += () => {
			EventAnimationController.Instance.ExecutePhaseAnimation(GamePhase.PlayerPhase);
			mbGameStarted = true;
		};

		ActionSequence startSeq = new ActionSequence(drawParallel, pan, phaseAnimDelay);
		ActionHandler.RunAction(startSeq);
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
					ControlAreaManager.Instance.SetControlBlockerEnabled(true);
					return BTStatus.Running;
				}
				// Check player turn status.
				if (Player.TurnStatus == PlayerTurnStatus.Running)
				{
					ControlAreaManager.Instance.SetControlBlockerEnabled(true);
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
						return BTStatus.Success;
					}
					break;
				}

				return BTStatus.Running;
			}
		);

		// Reduce the player's shield by 1 point after end of every enemy turn.
		BTAction BTAct_ReducePlayersShield = new BTAction(
			() =>
			{
				Player.DeductShieldPoints(1);
				if (Player.TurnStatus == PlayerTurnStatus.Running)
					return BTStatus.Running;
				else
				{
					EndEnemyPhase();
					return BTStatus.Success;
				}
			}
		);

		// BT_Sequence only runs when there are enemy pieces.
		// Refer to BT_NullCheckSelector below.
		BTSequence BT_EnemyMovementSequence = new BTSequence(BTAct_MoveEnemyPieces, BTAct_AttackPlayer);
		// Root is Selector that checks for enemies.
		// If no enemies, immediately stops (BTAct_NoEnemyCheck returns success).
		BTSelector BT_NullCheckSelector = new BTSelector(BTAct_NoEnemyCheck, BT_EnemyMovementSequence);
		BTSequence BT_Root = new BTSequence(BTAct_EnemyPhaseStartAnimations, BT_NullCheckSelector, BTAct_ReducePlayersShield);

		mEnemyPhaseBehaviourTree = new BehaviourTree(BT_Root);
	}
	
	private void Update()
	{
		if (!mbGameStarted)
		{
			return;
		}

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
		// ControlAreaManager.Instance.SetControlBlockerEnabled(false) is now done by the phase animation below.
		EventAnimationController.Instance.ExecutePhaseAnimation(GamePhase.PlayerPhase);

		DelayAction firstDraw = new DelayAction(1.8f);
		firstDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		DelayAction secondDraw = new DelayAction(1.8f + 0.5f);
		secondDraw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		ActionHandler.RunAction(firstDraw, secondDraw);
	}
	
	private void ExitPlayerPhase()
	{
		RepeatPanelControls.ClearRepeats();
		ControlAreaManager.Instance.SetControlBlockerEnabled(true);
	}

	private void ExecutePlayerPhase()
	{
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
		int numBlack, numStone, numSlime, numGlass, numGold, numCursed;

		if (FloorNumber < 5)
		{
			numBlack = 10 - FloorNumber;
			numStone = FloorNumber;
			numSlime = 0;
			numGlass = 0;
			numGold = 0;
			numCursed = 0;
		}
		else if (FloorNumber < 15)
		{
			numBlack = Random.Range(1, 7);
			numStone = Random.Range(1, 11);
			numSlime = 0;
			numGlass = Random.Range(1, 5);
			numGold = Random.Range(0, 3);
			numCursed = 0;
		}
		else if (FloorNumber < 25)
		{
			numBlack = Random.Range(1, 8);
			numStone = Random.Range(1, 11);
			numSlime = Random.Range(1, 4);
			numGlass = Random.Range(1, 5);
			numGold = Random.Range(0, 4);
			numCursed = 0;
		}
		else
		{
			// Scale the range accordingly to each floor level.
			numBlack = Random.Range(FloorNumber - 24, FloorNumber - 17);
			numStone = Random.Range(FloorNumber - 24, FloorNumber - 11);
			numSlime = Random.Range(FloorNumber - 24, FloorNumber - 20);
			numGlass = Random.Range(FloorNumber - 24, FloorNumber - 20);
			numGold = Random.Range(FloorNumber - 25, FloorNumber - 20);
			numCursed = Random.Range(FloorNumber - 25, FloorNumber - 24);
		}

		int[] spawnNums = { numBlack, numStone, numSlime, numGlass, numGold, numCursed };
		for (int iType = 0; iType < spawnNums.Length; iType++)
		{
			int numEnemiesSpawned = 0;

			while (numEnemiesSpawned < spawnNums[iType])
			{
				int posX = Random.Range(1, DungeonManager.Instance.SizeX - 2);
				int posY = Random.Range(1, DungeonManager.Instance.SizeY - 2);

				if (DungeonManager.Instance.IsCellEmpty(posX, posY) &&
					DungeonManager.Instance.IsPlayerPos(posX, posY) == false)
				{
					EnemyPiece.Spawn(posX, posY, (EnemyUnit) Random.Range(5 * iType, 5 * (iType + 1)));
					numEnemiesSpawned++;
				}
			}
		}
	}

	private void PlacePlayer()
	{
		mPlayerPiece = ((GameObject)Instantiate(
			PlayerPrefab,
			DungeonManager.Instance.GridPosToWorldPos(DungeonManager.Instance.SpawnPosX, DungeonManager.Instance.SpawnPosY),
			Quaternion.identity)).GetComponent<PlayerPiece>();

		PlayerData playerData = new PlayerData(3, 0, 0);
		if (PlayerPrefs.HasKey(Constants.kStrPlayerHealth))
			playerData.Health = PlayerPrefs.GetInt(Constants.kStrPlayerHealth);
		if (PlayerPrefs.HasKey(Constants.kStrPlayerHealth))
			playerData.Shield = PlayerPrefs.GetInt(Constants.kStrPlayerShield);
		if (PlayerPrefs.HasKey(Constants.kStrPlayerHealth))
			playerData.Coins = PlayerPrefs.GetInt(Constants.kStrPlayerCoins);
		mPlayerPiece.Initialise(DungeonManager.Instance.SpawnPosX, DungeonManager.Instance.SpawnPosY, playerData);
	}

	public void PlayerDied()
	{
		// Reset Floor Number.
		PlayerPrefs.DeleteKey(Constants.kStrFloorNumber);

		// Set highscore if any and clear current score.
		if (PlayerPrefs.HasKey(Constants.kStrHighscore))
		{
			int oldHighscore = PlayerPrefs.GetInt(Constants.kStrHighscore);
			if (mnScore > oldHighscore)
			{
				PlayerPrefs.SetInt(Constants.kStrHighscore, mnScore);
			}
		}
		else
		{
			PlayerPrefs.SetInt(Constants.kStrHighscore, mnScore);
		}
		PlayerPrefs.DeleteKey(Constants.kStrCurScore);

		// Reset PlayerData.
		PlayerPrefs.DeleteKey(Constants.kStrPlayerHealth);
		PlayerPrefs.DeleteKey(Constants.kStrPlayerShield);
		PlayerPrefs.DeleteKey(Constants.kStrPlayerCoins);

		// Clear Cards.
		for (int i = 0; i < DeckManager.knMaxCardsInHand; i++)
		{
			PlayerPrefs.DeleteKey(Constants.kStrCardType[i]);
			PlayerPrefs.DeleteKey(Constants.kStrCardTier[i]);
			PlayerPrefs.DeleteKey(Constants.kStrCardMoveType[i]);
		}
		PlayerPrefs.DeleteKey(Constants.kStrNumCardsInHand);

		mbIsGameOver = true;
		EventAnimationController.Instance.ShowGameOver();
	}

	public void ReachedFloorExit()
	{
		// Progress Floor.
		mnFloorNumber++;
		PlayerPrefs.SetInt(Constants.kStrFloorNumber, mnFloorNumber);

		// Add clear floor bonus score.
		AddScore(100);

		// Save current score.
		PlayerPrefs.SetInt(Constants.kStrCurScore, mnScore);

		// Save PlayerData.
		PlayerPrefs.SetInt(Constants.kStrPlayerHealth, Player.Health);
		PlayerPrefs.SetInt(Constants.kStrPlayerShield, Player.Shield);
		PlayerPrefs.SetInt(Constants.kStrPlayerCoins, Player.Coins);

		// Save Cards.
		for (int i = 0; i < DeckManager.Instance.GetNumCardsInHand(); i++)
		{
			CardData curCardData = DeckManager.Instance.GetCardDataAt(i);
			PlayerPrefs.SetInt(Constants.kStrCardType[i], (int)curCardData.Type);
			PlayerPrefs.SetInt(Constants.kStrCardTier[i], (int)curCardData.Tier);
			PlayerPrefs.SetInt(Constants.kStrCardMoveType[i], (int)curCardData.MoveType);
		}
		PlayerPrefs.SetInt(Constants.kStrNumCardsInHand, DeckManager.Instance.GetNumCardsInHand());

		mbIsGameOver = true;
		EventAnimationController.Instance.ShowFloorCleared();
	}
}
