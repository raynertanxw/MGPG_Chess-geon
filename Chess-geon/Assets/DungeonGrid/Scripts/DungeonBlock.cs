using UnityEngine;
using System.Collections;

public enum BlockState { Empty, Obstacle, EnemyPiece };	// Only Empty is traversable.
public enum TerrainType { Tile, Wall };

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
		}
	}

	public void SetBlockState(BlockState _newBlockState)
	{
		#if UNITY_EDITOR
		if (_newBlockState == BlockState.Obstacle)
			Debug.LogWarning("Are you sure you want to set a dungeonblock to obstacle? There shouldn't be such a case");
		#endif

		mState = _newBlockState;
	}
}
