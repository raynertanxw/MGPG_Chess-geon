using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BTAction : IBTNode
{
	private static BTStatus EmptyFunc() { Debug.LogWarning("WARNING: BTAction should override this delegate."); return BTStatus.Running; }
	private OnExecuteDelegate OnExecute = EmptyFunc;
	public delegate BTStatus OnExecuteDelegate();

	public BTAction(OnExecuteDelegate _OnExecute)
	{
		OnExecute = _OnExecute;
	}

	public override List<IBTNode> GetChildren()
	{
		return mNodeList;
	}

	public override void Execute()
	{
		mStatus = OnExecute();

		if (mStatus == BTStatus.Running)
			return;
		else
			mTree.SetCurNode(mParent);
	}

	public override void ResetNode()
	{
		mStatus = BTStatus.Running;
	}
}
