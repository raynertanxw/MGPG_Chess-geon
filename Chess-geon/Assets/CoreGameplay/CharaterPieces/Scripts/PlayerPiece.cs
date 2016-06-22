using UnityEngine;
using System.Collections;
using DaburuTools.Action;
using DaburuTools;

public enum PlayerTurnStatus { Running, Waiting };

public class PlayerPiece : MonoBehaviour
{
	public SpriteRenderer mSpriteRen;
	public Sprite[] PlayerSprites;
	private int mnDefaultSpriteOrderInLayer;

	private GridType mMovementType = GridType.Pawn;
	public GridType MovementType { get { return mMovementType; } }
	public void SetMovementType(GridType _movementType) { mMovementType = _movementType; }

	private int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	private int mnHealth = -1;
	public int Health { get { return mnHealth; } }

	private PlayerTurnStatus mTurnStatus;
	public PlayerTurnStatus TurnStatus { get { return mTurnStatus; } }

	void Awake()
	{
		mTurnStatus = PlayerTurnStatus.Waiting;

		mSpriteRen = GetComponent<SpriteRenderer>();
		mSpriteRen.sprite = PlayerSprites[0];
		mnDefaultSpriteOrderInLayer = mSpriteRen.sortingOrder;

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}

	public void Initialise(int _startX, int _startY)
	{
		SetPosition(_startX, _startY);
	}

	private void SetPosition(int _newX, int _newY)
	{
		// Checking.
		if (DungeonManager.Instance.IsCellEmpty(_newX, _newY) == false)
		{
			Debug.LogWarning("Enemy is attempting move to NON-empty cell");
			return;
		}

		mnPosX = _newX;
		mnPosY = _newY;
	}

	public void ExecuteTurn(int _newX, int _newY)
	{
		mTurnStatus = PlayerTurnStatus.Running;
		mSpriteRen.sortingOrder = 1000;

		if (DungeonManager.Instance.IsEnemyPos(_newX, _newY))
			ExecuteAttack(_newX, _newY);
		else
			ExecuteMove(_newX, _newY);
		
		BoardScroller.Instance.FocusCameraToPos(
			DungeonManager.Instance.GridPosToWorldPos(PosX, PosY),
			0.2f,
			Graph.InverseExponential);
	}

	private void ExecuteMove(int _newX, int _newY)
	{
		SetPosition(_newX, _newY);
		MoveToAction moveToPos = new MoveToAction(this.transform, Graph.InverseExponential,
			DungeonManager.Instance.GridPosToWorldPos(_newX, _newY), 0.5f);
		moveToPos.OnActionFinish = () => { mTurnStatus = PlayerTurnStatus.Waiting; mSpriteRen.sortingOrder = mnDefaultSpriteOrderInLayer; };
		ActionHandler.RunAction(moveToPos);
	}

	private void ExecuteAttack(int _targetX, int _targetY)
	{
		// Do not move the player logical position values for attack.
		ScaleToAction scaleUp = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.75f, 0.5f);

		MoveToAction moveToPos = new MoveToAction(this.transform, Graph.Dipper,
			DungeonManager.Instance.GridPosToWorldPos(_targetX, _targetY), 0.25f);
		ScaleToAction scaleDownHit = new ScaleToAction(this.transform, Graph.Dipper, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.1f, 0.25f);
		ActionParallel hitParallel = new ActionParallel(moveToPos, scaleDownHit);

		DelayAction returnDelay = new DelayAction(0.1f);

		MoveToAction moveBack = new MoveToAction(this.transform, Graph.SmoothStep,
			DungeonManager.Instance.GridPosToWorldPos(PosX, PosY), 0.5f);
		ScaleToAction scaleDownReturn = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier, 0.5f);
		ActionParallel returnParallel = new ActionParallel(moveBack, scaleDownReturn);

		ActionSequence sequence = new ActionSequence(scaleUp, hitParallel, returnDelay, returnParallel);
		sequence.OnActionFinish = () => { mTurnStatus = PlayerTurnStatus.Waiting; mSpriteRen.sortingOrder = mnDefaultSpriteOrderInLayer; };
		ActionHandler.RunAction(sequence);
	}

	public void TakeDamage(int _damage)
	{
		mnHealth -= _damage;

		// TODO: Check if piece died. Handling for dying.
	}
}
