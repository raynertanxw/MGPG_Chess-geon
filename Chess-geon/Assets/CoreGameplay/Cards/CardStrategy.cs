using UnityEngine;
using System.Collections;

public abstract class CardStrategy
{
	public abstract void ExecuteCard(CardTier _tier, GridType _moveType);
}
