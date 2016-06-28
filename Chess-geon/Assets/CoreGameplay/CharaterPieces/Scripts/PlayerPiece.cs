using UnityEngine;
using System.Collections;
using DaburuTools.Action;
using DaburuTools;

public enum PlayerTurnStatus { Running, Waiting };

public class PlayerPiece : MonoBehaviour
{
	public SpriteRenderer mSpriteRen;
	private SpriteRenderer mShieldBubbleSpriteRen;
	private SpriteRenderer mCoinSpriteRen;
	private Transform mShieldBubbleTransform;
	private Transform mCoinTransform;
	public Sprite[] PlayerSprites;
	public Sprite ShieldBubble;
	public Sprite Coin;
	private int mnDefaultSpriteOrderInLayer;

	private GridType mMovementType = GridType.Pawn;
	public GridType MovementType { get { return mMovementType; } }

	private int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	private int mnHealth = -1;
	public int Health { get { return mnHealth; } }
	private int mnShield = -1;
	public int Shield { get { return mnShield; } }
	private int mnCoin = -1;
	public int Coins { get { return mnCoin; } }

	private PlayerTurnStatus mTurnStatus;
	public PlayerTurnStatus TurnStatus { get { return mTurnStatus; } }

	void Awake()
	{
		mTurnStatus = PlayerTurnStatus.Waiting;

		mSpriteRen = GetComponent<SpriteRenderer>();
		mSpriteRen.sprite = PlayerSprites[0];
		mnDefaultSpriteOrderInLayer = mSpriteRen.sortingOrder;

		mShieldBubbleTransform = transform.GetChild(0);
		mShieldBubbleSpriteRen = mShieldBubbleTransform.GetComponent<SpriteRenderer>();
		mShieldBubbleSpriteRen.sprite = ShieldBubble;
		mShieldBubbleSpriteRen.enabled = false;

		mCoinTransform = transform.GetChild(1);
		mCoinSpriteRen = mCoinTransform.GetComponent<SpriteRenderer>();
		mCoinSpriteRen.sprite = Coin;
		mCoinSpriteRen.enabled = false;

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}

	public void Initialise(int _startX, int _startY)
	{
		SetPosition(_startX, _startY);
		mnHealth = 3;
		mnShield = 0;
		mnCoin = 0;
	}

	private void SetPosition(int _newX, int _newY)
	{
		// Checking.
		if (DungeonManager.Instance.IsExitCell(_newX, _newY))
		{
//			Debug.Log("Reached EXIT TILE!!!");
		}
		else if (DungeonManager.Instance.IsCellEmpty(_newX, _newY) == false)
		{
			Debug.LogWarning("Player is attempting move to NON-empty cell");
			return;
		}

		mnPosX = _newX;
		mnPosY = _newY;
	}

	public void SetMovementType(GridType _movementType)
	{
		mMovementType = _movementType;
		mSpriteRen.sprite = PlayerSprites[(int)_movementType];
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
		moveToPos.OnActionFinish = () => {
			mTurnStatus = PlayerTurnStatus.Waiting; mSpriteRen.sortingOrder = mnDefaultSpriteOrderInLayer;

			// Check if reached the dungeonExit.
			if (DungeonManager.Instance.IsExitCell(PosX, PosY))
			{
				GameManager.Instance.ReachedFloorExit();
			}
		};
		ActionHandler.RunAction(moveToPos);
	}

	private void ExecuteAttack(int _targetX, int _targetY)
	{
		ActionSequence sequence = new ActionSequence();

		// Do not move the player logical position values for attack.
		ScaleToAction scaleUp = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.75f, 0.5f);

		MoveToAction moveToPos = new MoveToAction(this.transform, Graph.Dipper,
			DungeonManager.Instance.GridPosToWorldPos(_targetX, _targetY), 0.25f);
		ScaleToAction scaleDownHit = new ScaleToAction(this.transform, Graph.Dipper, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.1f, 0.25f);
		ActionParallel hitParallel = new ActionParallel(moveToPos, scaleDownHit);
		hitParallel.OnActionFinish += () => {
			EnemyPiece target = DungeonManager.Instance.DungeonBlocks[_targetX, _targetY].Enemy;
			target.TakeDamage(1);
			if (target.Health <= 0)
			{
				ScaleToAction scaleDown = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier, 0.5f);
				scaleDown.OnActionStart += () => {
					BoardScroller.Instance.FocusCameraToPos(this.transform.position, 0.5f, Graph.InverseExponential);
				};
				sequence.Add(scaleDown);

				SetPosition(_targetX, _targetY);
			}
			else
			{
				MoveToAction moveBack = new MoveToAction(this.transform, Graph.SmoothStep,
					DungeonManager.Instance.GridPosToWorldPos(PosX, PosY), 0.5f);
				ScaleToAction scaleDownReturn = new ScaleToAction(this.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier, 0.5f);
				ActionParallel returnParallel = new ActionParallel(moveBack, scaleDownReturn);
				sequence.Add(returnParallel);
			}
		};

		DelayAction returnDelay = new DelayAction(0.1f);

		sequence.Add(scaleUp, hitParallel, returnDelay);
		sequence.OnActionFinish = () => { mTurnStatus = PlayerTurnStatus.Waiting; mSpriteRen.sortingOrder = mnDefaultSpriteOrderInLayer; };
		ActionHandler.RunAction(sequence);
	}

	public void TakeDamage(int _damage)
	{
		// Let Shield Take Damage.
		if (Shield > 0)
		{
			DeductShieldPoints(_damage);

			ShakeAction2D camShake = new ShakeAction2D(Camera.main.transform, 10, 1.25f, Graph.InverseLinear);
			camShake.SetShakeByDuration(0.2f, 15);
			ActionHandler.RunAction(camShake);
			return;
		}

		mnHealth -= _damage;
		PlayerInfoManager.Instance.UpdateHealth(mnHealth);

		// TODO: Check if player died. Handling for dying.
		if (mnHealth > 0)
		{
			// Player hasn't died.
			ShakeAction2D camShake = new ShakeAction2D(Camera.main.transform, 10, 1.25f, Graph.InverseLinear);
			camShake.SetShakeByDuration(0.2f, 15);
			ActionHandler.RunAction(camShake);
		}
		else
		{
			// End the game here.
			GameManager.Instance.PlayerDied();

			// More dramatic shake.
			ShakeAction2D camShake = new ShakeAction2D(Camera.main.transform, 10, 1.75f, Graph.InverseLinear);
			camShake.SetShakeByDuration(0.4f, 25);
			ActionHandler.RunAction(camShake);
		}
	}

	public void AddShieldPoints(int _shieldPoints)
	{
		// If re-adding the shield.
		if (Shield <= 0)
		{
			mnShield = 0;
			mnShield += _shieldPoints;
			PlayerInfoManager.Instance.UpdateShield(mnShield);

			// Show shield bubble and start the animation.
			mShieldBubbleTransform.localScale = Vector3.zero;
			mShieldBubbleSpriteRen.enabled = true;
			ScaleToAction shieldScaleUp = new ScaleToAction(mShieldBubbleTransform, Graph.InverseExponential, Vector3.one, 1.0f);
			ActionHandler.RunAction(shieldScaleUp);
		}
		// Else just adding on top of the shield.
		else
		{
			mnShield += _shieldPoints;
			PlayerInfoManager.Instance.UpdateShield(mnShield);
		}
	}

	public void DeductShieldPoints(int _shieldPoints)
	{
		// If shield is already broken or not active, just ignore.
		if (Shield <= 0)
		{
			mTurnStatus = PlayerTurnStatus.Waiting;
			return;
		}

		mnShield -= _shieldPoints;
		PlayerInfoManager.Instance.UpdateShield(mnShield);

		// Check if shield got broken.
		if (mnShield <= 0)
		{
			mTurnStatus = PlayerTurnStatus.Running;

			// Broken sheild animation.
			ShakeAction2D shieldFailureShake = new ShakeAction2D(mShieldBubbleTransform, 100, 0.05f, Graph.InverseLinear);
			shieldFailureShake.SetShakeByDuration(0.8f, 100);
			ScaleToAction shieldScaleDown = new ScaleToAction(mShieldBubbleTransform, Graph.Exponential, Vector3.zero, 1.0f);
			shieldScaleDown.OnActionFinish += () => {
				mShieldBubbleSpriteRen.enabled = false;
				mTurnStatus = PlayerTurnStatus.Waiting;
			};
			ActionHandler.RunAction(shieldFailureShake, shieldScaleDown);
		}
		else
		{
			mTurnStatus = PlayerTurnStatus.Waiting;
		}
	}

	// Interval per coin get
	private const float kfCoinAnimDuration = 0.2f;
	public void AddCoins(int _numCoins)
	{
		// TODO: Coin sound.

		// Play coin get animation.
		MoveByAction moveUp = new MoveByAction(mCoinTransform, Graph.Linear, Vector3.up * 1.4f, kfCoinAnimDuration);
		moveUp.OnActionStart += () => {
			mCoinTransform.position = transform.position;
			mCoinSpriteRen.enabled = true;
			mnCoin++;
			PlayerInfoManager.Instance.UpdateCoins(mnCoin);
		};
		ActionRepeat repeatedCoinGet = new ActionRepeat(moveUp, _numCoins);
		DelayAction hideCoin = new DelayAction(kfCoinAnimDuration * _numCoins + 0.5f);
		hideCoin.OnActionFinish += () => {
			mCoinSpriteRen.enabled = false;
		};
		ActionHandler.RunAction(repeatedCoinGet, hideCoin);
	}

	// CHECkING FOR ENOUGH COINS should be done first.
	public void SpendCoins(int _numCoins)
	{
		// TODO: Coin spending sound?

		mnCoin -= _numCoins;
		PlayerInfoManager.Instance.UpdateCoins(mnCoin);
	}
}
