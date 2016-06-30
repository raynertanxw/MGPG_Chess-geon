using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DaburuTools;
using DaburuTools.Action;

public enum EnemyUnit
{
	BlackPawn,	BlackRook,	BlackBishop,	BlackKnight,	BlackKing,
	StonePawn,	StoneRook,	StoneBishop,	StoneKnight,	StoneKing,
	SlimePawn,	SlimeRook,	SlimeBishop,	SlimeKnight,	SlimeKing,
	GlassPawn,	GlassRook,	GlassBishop,	GlassKnight,	GlassKing,
	GoldPawn,	GoldRook,	GoldBishop,		GoldKnight,		GoldKing,
	CursedPawn,	CursedRook,	CursedBishop,	CursedKnight,	CursedKing,
	Count
};
public enum EnemyType { Black, Stone, Slime, Glass, Gold, Cursed };
public enum EnemyTurnStatus { Unprocessed, Running, Waiting, Processed };

public class EnemyPiece : MonoBehaviour
{
	#region Object Pooling
	private static List<EnemyPiece> enemyPool = null;
	private static Sprite[] enemySprites = null;
	private static string[] enemySpriteNames = null;
	private static EnemyStratergy[] enemyAlgorithms = null;
	private static EnemyTypeStrategy[] typeAlgorithms = null;

	private void Awake()
	{
		if (enemyPool == null)
		{
			enemyPool = new List<EnemyPiece>();

			enemySprites = Resources.LoadAll<Sprite>("Sprites/chessgeon_pieces");
			enemySpriteNames = new string[enemySprites.Length];

			for(int i = 0; i < enemySpriteNames.Length; i++) {
				enemySpriteNames[i] = enemySprites[i].name;
			}

			enemyAlgorithms = new EnemyStratergy[5];
			enemyAlgorithms[0] = new EnemyStratergyPawn();
			enemyAlgorithms[1] = new EnemyStratergyRook();
			enemyAlgorithms[2] = new EnemyStratergyBishop();
			enemyAlgorithms[3] = new EnemyStratergyKnight();
			enemyAlgorithms[4] = new EnemyStratergyKing();

			typeAlgorithms = new EnemyTypeStrategy[6];
			typeAlgorithms[0] = new EnemyTypeStrategyBlack();
			typeAlgorithms[1] = new EnemyTypeStrategyStone();
			typeAlgorithms[2] = new EnemyTypeStrategySlime();
			typeAlgorithms[3] = new EnemyTypeStrategyGlass();
			typeAlgorithms[4] = new EnemyTypeStrategyGold();
			typeAlgorithms[5] = new EnemyTypeStrategyCursed();
		}

		Setup();
		ReturnToPool();
		enemyPool.Add(this);
	}

	private void OnDestroy()
	{
		enemyPool.Remove(this);
		if (enemyPool.Count == 0)
		{
			enemyPool = null;
			enemySprites = null;
			enemySpriteNames = null;
			enemyAlgorithms = null;
		}
	}

	public static EnemyPiece Spawn(int _posX, int _posY, int _health, GridType _movementType, EnemyType _unitType)
	{
		EnemyPiece curPiece = null;

		for (int i = 0; i < enemyPool.Count; i++)
		{
			if (enemyPool[i].IsAlive == false)
			{
				curPiece = enemyPool[i];
				curPiece.mbIsAlive = true;
				curPiece.mSpriteRen.enabled = true;

				curPiece.mnPosX = _posX;
				curPiece.mnPosY = _posY;
				curPiece.mnHealth = _health;
				curPiece.SetMovementType(_movementType);
				curPiece.SetUnitType(_unitType);
				int spriteIndex = Array.IndexOf(enemySpriteNames, _unitType.ToString() + "_" + _movementType.ToString());
				curPiece.mSpriteRen.sprite = enemySprites[spriteIndex];

				DungeonManager.Instance.PlaceEnemy(curPiece, _posX, _posY);
				curPiece.transform.position = DungeonManager.Instance.GridPosToWorldPos(_posX, _posY);

				break;
			}
		}

		if (curPiece == null)
		{
			GameObject newEnemyPiece = (GameObject) Instantiate(GameManager.Instance.EnemyPrefab);

			curPiece = newEnemyPiece.GetComponent<EnemyPiece>();
			curPiece.mbIsAlive = true;
			curPiece.mSpriteRen.enabled = true;

			curPiece.mnPosX = _posX;
			curPiece.mnPosY = _posY;
			curPiece.mnHealth = _health;
			curPiece.SetMovementType(_movementType);
			curPiece.SetUnitType(_unitType);
			int spriteIndex = Array.IndexOf(enemySpriteNames, _unitType.ToString() + "_" + _movementType.ToString());
			curPiece.mSpriteRen.sprite = enemySprites[spriteIndex];

			DungeonManager.Instance.PlaceEnemy(curPiece, _posX, _posY);
			curPiece.transform.position = DungeonManager.Instance.GridPosToWorldPos(_posX, _posY);
		}

		GameManager.Instance.EnemyList.Add(curPiece);

		return curPiece;
	}

	public static EnemyPiece Spawn(int _posX, int _posY, EnemyUnit enemyType)
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

		case EnemyUnit.StonePawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Stone);
			break;
		case EnemyUnit.StoneRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Stone);
			break;
		case EnemyUnit.StoneBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Stone);
			break;
		case EnemyUnit.StoneKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Stone);
			break;
		case EnemyUnit.StoneKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Stone);
			break;

		case EnemyUnit.SlimePawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Slime);
			break;
		case EnemyUnit.SlimeRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Slime);
			break;
		case EnemyUnit.SlimeBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Slime);
			break;
		case EnemyUnit.SlimeKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Slime);
			break;
		case EnemyUnit.SlimeKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Slime);
			break;

		case EnemyUnit.GlassPawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Glass);
			break;
		case EnemyUnit.GlassRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Glass);
			break;
		case EnemyUnit.GlassBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Glass);
			break;
		case EnemyUnit.GlassKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Glass);
			break;
		case EnemyUnit.GlassKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Glass);
			break;

		case EnemyUnit.GoldPawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Gold);
			break;
		case EnemyUnit.GoldRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Gold);
			break;
		case EnemyUnit.GoldBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Gold);
			break;
		case EnemyUnit.GoldKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Gold);
			break;
		case EnemyUnit.GoldKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Gold);
			break;

		case EnemyUnit.CursedPawn:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Pawn, EnemyType.Cursed);
			break;
		case EnemyUnit.CursedRook:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Rook, EnemyType.Cursed);
			break;
		case EnemyUnit.CursedBishop:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Bishop, EnemyType.Cursed);
			break;
		case EnemyUnit.CursedKnight:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.Knight, EnemyType.Cursed);
			break;
		case EnemyUnit.CursedKing:
			curPiece = EnemyPiece.Spawn(_posX, _posY, 1, GridType.King, EnemyType.Cursed);
			break;
		default:
			Debug.LogError("No enemyType handling found");
			break;
		}

		return curPiece;
	}

	private void ReturnToPool()
	{
		mbIsAlive = false;
		mSpriteRen.enabled = false;
	}
	#endregion

	public SpriteRenderer mSpriteRen;

	private void Setup()
	{
		mSpriteRen = GetComponent<SpriteRenderer>();

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}

	private bool mbIsAlive = false;
	public bool IsAlive { get { return mbIsAlive; } }

	private EnemyTurnStatus mTurnStatus = EnemyTurnStatus.Unprocessed;
	public EnemyTurnStatus TurnStatus { get { return mTurnStatus; } }

	private LinkedList<Node> mPath;

	private GridType mMovementType = GridType.Pawn;
	public GridType MovementType { get { return mMovementType; } }
	public void SetMovementType(GridType _movementType) { mMovementType = _movementType; }
	private EnemyType mUnitType = EnemyType.Black;
	public EnemyType UnitType { get { return mUnitType; } }
	public void SetUnitType(EnemyType _unitType) { mUnitType = _unitType; }

	private int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	private int mnHealth = -1;
	public int Health { get { return mnHealth; } }

	private void MovePosition(int _newX, int _newY)
	{
		// Checking
		if (DungeonManager.Instance.IsCellEmpty(_newX, _newY) == false)
		{
			Debug.LogWarning("Enemy is attempting move to NON-empty cell");
			return;
		}

		DungeonManager.Instance.MoveEnemy(mnPosX, mnPosY, _newX, _newY);
		mnPosX = _newX;
		mnPosY = _newY;
	}

	public void TakeDamage(int _damage)
	{
		EventAnimationController.Instance.SpawnDamageParticles(transform.position);
		ShakeAction2D camShake = new ShakeAction2D(Camera.main.transform, 5, 0.5f, Graph.InverseLinear);
		camShake.SetShakeByDuration(0.2f, 15);
		ActionHandler.RunAction(camShake);

		mnHealth -= _damage;

		// Special Take Damage events.
		typeAlgorithms[(int)UnitType].SpecialTakeDamageEvents();

		// Check if piece died. Handling for dying.
		if (mnHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		// Coins for the player.
		int coinsDropped = -1;
		switch (mUnitType)
		{
		case EnemyType.Black:	coinsDropped = 1; break;
		case EnemyType.Stone:	coinsDropped = 2; break;
		case EnemyType.Slime:	coinsDropped = 1; break;
		case EnemyType.Glass:	coinsDropped = 2; break;
		case EnemyType.Gold:	coinsDropped = 15; break;
		case EnemyType.Cursed:	coinsDropped = 5; break;
		}
		DelayAction delayForCoins = new DelayAction(0.5f);
		delayForCoins.OnActionFinish += () => {
			GameManager.Instance.Player.AddCoins(coinsDropped);
		};
		ActionHandler.RunAction(delayForCoins);

		// Special Die Events
		typeAlgorithms[(int)UnitType].SpecialDieEvents(this);

		// TODO: Diff animations by diff enemy type?
		ScaleToAction scaleDown = new ScaleToAction(transform, Vector3.zero, 0.4f);
		scaleDown.OnActionFinish += () => {
			ReturnToPool();
			transform.localScale = Vector3.one;
			transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
		};
		ActionHandler.RunAction(scaleDown);

		DungeonManager.Instance.RemoveEnemy(PosX, PosY);
		GameManager.Instance.EnemyList.Remove(this);
	}

	private void GeneratePath()
	{
		mPath = enemyAlgorithms[(int)MovementType].GeneratePath(PosX, PosY);
	}

	private void ExecuteMove()
	{
		Node targetNode = mPath.First.Next.Value;
		MovePosition(targetNode.PosX, targetNode.PosY);
		MoveToAction moveToPos = new MoveToAction(this.transform, Graph.InverseExponential,
			DungeonManager.Instance.GridPosToWorldPos(targetNode.PosX, targetNode.PosY), 0.5f);
		moveToPos.OnActionFinish = () => { mTurnStatus = EnemyTurnStatus.Processed; };
		ActionHandler.RunAction(moveToPos);

		BoardScroller.Instance.FocusCameraToPos(
			DungeonManager.Instance.GridPosToWorldPos(PosX, PosY),
			0.2f,
			Graph.InverseExponential);
	}

	private void ExecuteAttack()
	{
		mTurnStatus = EnemyTurnStatus.Running;

		Node targetNode = mPath.First.Next.Value;
		// Do not move the enemy in the grid for attack.
		ScaleToAction scaleUp = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.75f, 0.5f);

		MoveToAction moveToPos = new MoveToAction(this.transform, Graph.Dipper,
			DungeonManager.Instance.GridPosToWorldPos(targetNode.PosX, targetNode.PosY), 0.25f);
		ScaleToAction scaleDownHit = new ScaleToAction(this.transform, Graph.Dipper, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.1f, 0.25f);
		ActionParallel hitParallel = new ActionParallel(moveToPos, scaleDownHit);
		hitParallel.OnActionFinish += () => {
			GameManager.Instance.Player.TakeDamage(typeAlgorithms[(int)UnitType].GetDamagePower());
		};

		DelayAction returnDelay = new DelayAction(0.1f);

		MoveToAction moveBack = new MoveToAction(this.transform, Graph.SmoothStep,
			DungeonManager.Instance.GridPosToWorldPos(PosX, PosY), 0.5f);
		ScaleToAction scaleDownReturn = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier, 0.5f);
		ActionParallel returnParallel = new ActionParallel(moveBack, scaleDownReturn);
		
		ActionSequence sequence = new ActionSequence(scaleUp, hitParallel, returnDelay, returnParallel);
		sequence.OnActionFinish = () => { mTurnStatus = EnemyTurnStatus.Processed; };
		ActionHandler.RunAction(sequence);
	}
	
	public void ExecuteTurn()
	{
		if (mTurnStatus == EnemyTurnStatus.Waiting)
		{
			ExecuteAttack();
			return;
		}

		mTurnStatus = EnemyTurnStatus.Running;

		GeneratePath();
		
		if (mPath == null)
		{
			Debug.LogWarning("Path returned Null");
			mTurnStatus = EnemyTurnStatus.Processed;
			return;
		}

		Node targetNode = mPath.First.Next.Value;
		if (DungeonManager.Instance.IsPlayerPos(targetNode.PosX, targetNode.PosY))
		{
			mTurnStatus = EnemyTurnStatus.Waiting;
		}
		else
		{
			ExecuteMove();
		}
	}

	public void ResetTurnStatus()
	{
		mTurnStatus = EnemyTurnStatus.Unprocessed;
	}
}
