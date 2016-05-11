using UnityEngine;
using System.Collections;

public class RoomPattern : ScriptableObject
{
	[SerializeField]
	private int RoomSizeX = 5;
	public int SizeX { get { return RoomSizeX; } }
	[SerializeField]
	private int RoomSizeY = 5;
	public int SizeY { get { return RoomSizeY; } }
	[SerializeField]
	private float mfMatchPercentage = 1.0f;
	public float MatchPercentage { get { return mfMatchPercentage; } }

	[SerializeField]
	private TerrainType[] mArrBlockTerrainType;
	public TerrainType[] BlockTerrainType { get { return mArrBlockTerrainType; } }
}
