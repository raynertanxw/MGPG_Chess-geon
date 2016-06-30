using UnityEngine;
using System.Collections;

public class EnemyTypeStrategyGold : EnemyTypeStrategy
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
		return 1;
	}
}
