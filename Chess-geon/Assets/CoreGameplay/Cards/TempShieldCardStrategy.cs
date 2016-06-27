using UnityEngine;
using System.Collections;

public class TempShieldCardStrategy : CardStrategy
{
	public override void ExecuteCard(CardTier _tier, GridType _moveType)
	{
		TempShieldPanelControls.sShieldTier = _tier;
	}
}
