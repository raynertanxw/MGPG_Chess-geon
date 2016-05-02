using UnityEngine;
using System.Collections;

public enum BlockState { Wall, Enemy, Empty, Player, Selectable };

public class DungeonBlock
{
	private BlockState mState;
	public BlockState State { get { return mState; } }

	public DungeonBlock(BlockState _state)
	{
		mState = _state;
	}
}
