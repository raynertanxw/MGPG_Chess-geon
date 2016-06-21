using UnityEngine;
using System.Collections;

public class MovementCardStrategy : CardStrategy
{
	public override void ExecuteCard(CardTier _tier, GridType _moveType)
	{
		GameManager.Instance.Player.SetMovementType(_moveType);
	}
}
