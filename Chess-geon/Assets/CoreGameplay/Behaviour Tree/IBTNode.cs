using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BTStatus { Running, Success, Failure };

public abstract class IBTNode
{
	// Running is default state.
	protected BTStatus mStatus = BTStatus.Running;
	public BTStatus Status { get { return mStatus; } }
	protected IBTNode mParent = null;
	protected List<IBTNode> mNodeList = null;
	protected BehaviourTree mTree;

	public abstract List<IBTNode> GetChildren();
	public abstract void Execute();
	public abstract void ResetNode();
}
