using UnityEngine;
using System.Collections;

public class RookEnemyPiece : IEnemyPiece
{
	public RookEnemyPiece(GameObject _prefab)
	{
		mMovementType = GridType.Rook;

		mPrefab = _prefab;
	}

	public override void Spawn (int _posX, int _posY, int _health)
	{
		if (Health > 0)
		{
			Debug.LogError("Piece has already been spawned");
			return;
		}

		mnPosX = _posX;
		mnPosY = _posY;
		mnHealth = _health;

		DungeonManager.Instance.PlaceEnemy(_posX, _posY);
		GameManager.Instantiate(mPrefab, DungeonManager.Instance.GridPosToWorldPos(_posX, _posY), Quaternion.identity);
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
}
