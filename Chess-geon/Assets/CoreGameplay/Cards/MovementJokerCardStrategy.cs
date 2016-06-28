using UnityEngine;
using System.Collections;

public class MovementJokerCardStrategy : CardStrategy
{
	public override void ExecuteCard(CardTier _tier, GridType _moveType)
	{
		MovementJokerPanelControls.sJokerTier = _tier;
	}
}
