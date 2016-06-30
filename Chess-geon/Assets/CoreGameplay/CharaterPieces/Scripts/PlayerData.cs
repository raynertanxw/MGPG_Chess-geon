using UnityEngine;
using System.Collections;

public struct PlayerData
{
	public int Health;
	public int Shield;
	public int Coins;

	public PlayerData(int _Health, int _Shield, int _Coins)
	{
		Health = _Health;
		Shield = _Shield;
		Coins = _Coins;
	}
}
