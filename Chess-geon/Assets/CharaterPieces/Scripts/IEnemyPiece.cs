using UnityEngine;
using System.Collections;

public abstract class IEnemyPiece
{
	protected GridType mMovementType;
	public GridType MovementType { get { return mMovementType; } }
	protected GameObject mPrefab;

	protected int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	protected int mnHealth = -1;
	public int Health { get { return mnHealth; } }

	public abstract void Spawn(int _posX, int _posY, int _health);
	public abstract void SetPosition(int _newX, int _newY);
	public abstract void TakeDamage(int _damage);
}
