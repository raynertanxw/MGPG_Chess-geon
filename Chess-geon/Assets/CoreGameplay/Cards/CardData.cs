using UnityEngine;
using System.Collections;

public struct CardData
{
	public CardType Type;
	public CardTier Tier;
	public GridType MoveType;

	public CardData(CardType _type, CardTier _tier, GridType _moveType)
	{
		Type = _type;
		Tier = _tier;
		MoveType = _moveType;
	}
}
