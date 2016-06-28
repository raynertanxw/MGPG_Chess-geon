using UnityEngine;
using System.Collections;

public class DrawCardCardStrategy : CardStrategy
{
	public override void ExecuteCard(CardTier _tier, GridType _moveType)
	{
		DrawCardPanelControls.sDrawTier = _tier;
	}
}
