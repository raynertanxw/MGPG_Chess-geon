using UnityEngine;
using System.Collections;

public enum BlockState { Wall, Enemy, Empty, Player, Selectable };

public class DungeonBlock
{
	private BlockState mState;
	public BlockState State { get { return mState; } }
    private int mnPosX, mnPosY;
    public int PosX { get { return mnPosX; } }
    public int PosY { get { return mnPosY; } }

	public DungeonBlock(BlockState _state, int _x, int _y)
	{
        mnPosX = _x;
        mnPosY = _y;
		mState = _state;
	}
}
