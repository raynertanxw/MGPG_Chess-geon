using UnityEngine;
using System.Collections;

public abstract class EnemyTypeStrategy
{
	public abstract void SpecialDieEvents(EnemyPiece _enemy);
	public abstract void SpecialTakeDamageEvents();
	public abstract int GetDamagePower();
}
