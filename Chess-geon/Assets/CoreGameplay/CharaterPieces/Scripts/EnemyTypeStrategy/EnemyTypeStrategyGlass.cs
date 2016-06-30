using UnityEngine;
using System.Collections;

public class EnemyTypeStrategyGlass : EnemyTypeStrategy
{
	public override void SpecialDieEvents(EnemyPiece _enemy)
	{
		// Nothing speical here :)
	}

	public override void SpecialTakeDamageEvents()
	{
		GameManager.Instance.Player.TakeDamage(1);
	}

	public override int GetDamagePower()
	{
		return 1;
	}
}
