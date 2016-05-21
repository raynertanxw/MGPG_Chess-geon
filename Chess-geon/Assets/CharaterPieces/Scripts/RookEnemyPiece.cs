using UnityEngine;
using System.Collections;

public class RookEnemyPiece : IEnemyPiece
{
	public RookEnemyPiece(int _posX, int _posY, int _health)
	{
		mMovementType = GridType.Rook;

		mnPosX = _posX;
		mnPosY = _posY;

		mnHealth = Health;
	}

	public override void SetPosition(int _newX, int _newY)
	{
		// TODO: Checking???
		
		mnPosX = _newX;
		mnPosY = _newY;
	}

	public override void TakeDamage(int _damage)
	{
		mnHealth -= _damage;

		// TODO: Check if piece died. Handling for dying.
	}

	void Start()
	{
		
	}
	
	void Update()
	{
	
	}
}
