using UnityEngine;
using System.Collections;

public class PlayerPiece : MonoBehaviour
{
	public SpriteRenderer mSpriteRen;
	public Sprite[] PlayerSprites;

	private GridType mMovementType = GridType.Pawn;
	public GridType MovementType { get { return mMovementType; } }
	public void SetMovementType(GridType _movementType) { mMovementType = _movementType; }

	protected int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	protected int mnHealth = -1;
	public int Health { get { return mnHealth; } }

	private void Awake()
	{
		mSpriteRen = GetComponent<SpriteRenderer>();
		mSpriteRen.sprite = PlayerSprites[0];

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}

	public void SetPosition(int _newX, int _newY)
	{
		// TODO: Checking???

		mnPosX = _newX;
		mnPosY = _newY;
	}

	public void TakeDamage(int _damage)
	{
		mnHealth -= _damage;

		// TODO: Check if piece died. Handling for dying.
	}
}
