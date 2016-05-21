using UnityEngine;
using System.Collections;

public abstract class IEnemyPiece
{
	protected GridType mMovementType;
	public GridType MovementType { get { return mMovementType; } }

	protected int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	protected int mnHealth;
	public int Health { get { return mnHealth; } }

	public abstract void SetPosition(int _newX, int _newY);
	public abstract void TakeDamage(int _damage);

	void Start()
	{
	
	}
	
	void Update()
	{
	
	}
}
