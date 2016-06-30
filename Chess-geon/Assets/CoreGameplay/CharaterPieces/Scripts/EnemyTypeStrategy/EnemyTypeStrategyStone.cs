using UnityEngine;
using System.Collections;

public class EnemyTypeStrategyStone : EnemyTypeStrategy
{
	public override void SpecialDieEvents(EnemyPiece _enemy)
	{
		// Nothing speical here :)
	}

	public override void SpecialTakeDamageEvents()
	{
		// Nothing speical here :)
	}

	public override int GetDamagePower()
	{
		return 2;
	}
}
