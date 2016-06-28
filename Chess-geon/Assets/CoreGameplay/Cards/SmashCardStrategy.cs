using UnityEngine;
using System.Collections;

public class SmashCardStrategy : CardStrategy
{
	public override void ExecuteCard(CardTier _tier, GridType _moveType)
	{
		SmashPanelControls.sSmashTier = _tier;
	}
}
