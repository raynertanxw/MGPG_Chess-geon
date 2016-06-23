using UnityEngine;
using System.Collections;

public enum BlockState { Empty, Obstacle, EnemyPiece, DungeonExit };	// Only Empty is traversable.
public enum TerrainType { Tile, Wall, Stairs, Shop };

public class DungeonBlock
{
	private BlockState mState;
	public BlockState State { get { return mState; } }
	private TerrainType mTerrain;
	public TerrainType Terrain { get { return mTerrain; } }
    private int mnPosX, mnPosY;
    public int PosX { get { return mnPosX; } }
    public int PosY { get { return mnPosY; } }

	public DungeonBlock(TerrainType _type, int _x, int _y)
	{
        mnPosX = _x;
        mnPosY = _y;

		mTerrain = _type;
		switch (_type)
		{
		case TerrainType.Tile:
			mState = BlockState.Empty;
			break;
		case TerrainType.Wall:
			mState = BlockState.Obstacle;
			break;
		case TerrainType.Stairs:
			mState = BlockState.DungeonExit;
			break;
		case TerrainType.Shop:
			mState = BlockState.Empty;
			break;
		}
	}

	public void SetBlockState(BlockState _newBlockState)
	{
		#if UNITY_EDITOR
		if (_newBlockState == BlockState.Obstacle)
			Debug.LogWarning("Are you sure you want to set a dungeonblock to obstacle? There shouldn't be such a case");
		else if (_newBlockState == BlockState.DungeonExit)
			Debug.LogWarning("Are you sure you want to set a dungeonblock to dungeonExit? There shouldn't be such as case");
		#endif

		switch (_newBlockState)
		{
		case BlockState.Empty:
			mTerrain = TerrainType.Tile;
			break;
		default:
			break;
		}
		mState = _newBlockState;
	}
}
