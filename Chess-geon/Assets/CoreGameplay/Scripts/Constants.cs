using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour
{
	public const string kStrFloorNumber = "FloorNumber";
	public const string kStrCurScore = "CurScore";
	public const string kStrHighscore = "Highscore";

	// Player Data
	public const string kStrPlayerHealth = "PlayerHealth";
	public const string kStrPlayerShield = "PlayerShield";
	public const string kStrPlayerCoins = "PlayerCoins";

	// Card Data
	public static readonly string[] kStrCardType = {
		"CardType0",
		"CardType1",
		"CardType2",
		"CardType3",
		"CardType4"
	};

	public static readonly string[] kStrCardTier = {
		"CardTier0",
		"CardTier1",
		"CardTier2",
		"CardTier3",
		"CardTier4"
	};

	public static readonly string[] kStrCardMoveType = {
		"CardMoveType0",
		"CardMoveType1",
		"CardMoveType2",
		"CardMoveType3",
		"CardMoveType4"
	};
	public static readonly string kStrNumCardsInHand = "NumCardsInHand";
}
