using UnityEngine;
using System.Collections;

public class EnemyTypeStrategySlime : EnemyTypeStrategy
{
	public override void SpecialDieEvents(EnemyPiece _enemy)
	{
		// Don't spawn if pawn already.
		if (_enemy.MovementType == GridType.Pawn)
			return;

		EnemyUnit unit = EnemyUnit.SlimePawn;

		// Spawn two more slimes.
		// Try up down left right.
		int numSpawned = 0;
		// Left
		if (DungeonManager.Instance.IsCellEmpty(_enemy.PosX - 1, _enemy.PosY))
		{
			EnemyPiece.Spawn(_enemy.PosX - 1, _enemy.PosY, unit);
			numSpawned++;
			if (numSpawned >= 2)
				return;
		}

		// Right
		if (DungeonManager.Instance.IsCellEmpty(_enemy.PosX + 1, _enemy.PosY))
		{
			EnemyPiece.Spawn(_enemy.PosX + 1, _enemy.PosY, unit);
			numSpawned++;
			if (numSpawned >= 2)
				return;
		}

		// Up
		if (DungeonManager.Instance.IsCellEmpty(_enemy.PosX, _enemy.PosY + 1))
		{
			EnemyPiece.Spawn(_enemy.PosX, _enemy.PosY + 1, unit);
			numSpawned++;
			if (numSpawned >= 2)
				return;
		}

		// Down
		if (DungeonManager.Instance.IsCellEmpty(_enemy.PosX, _enemy.PosY - 1))
		{
			EnemyPiece.Spawn(_enemy.PosX, _enemy.PosY - 1, unit);
			numSpawned++;
			if (numSpawned >= 2)
				return;
		}
	}

	public override void SpecialTakeDamageEvents()
	{
		// Nothing speical here :)
	}

	public override int GetDamagePower()
	{
		return 1;
	}
}
