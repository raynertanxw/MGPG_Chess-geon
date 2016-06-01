using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BTSequence : IBTNode
{
	int curIndex = 0;

	public BTSequence(params IBTNode[] _nodes)
	{
		mNodeList = new List<IBTNode>();

		for (int i = 0; i < _nodes.Length; i++)
			mNodeList.Add(_nodes[i]);
	}

	public void AddBTNode(params IBTNode[] _nodes)
	{
		for (int i = 0; i < _nodes.Length; i++)
			mNodeList.Add(_nodes[i]);
	}

	public override List<IBTNode> GetChildren()
	{
		return mNodeList;
	}

	public override void Execute()
	{
		if (curIndex >= mNodeList.Count)
		{
			mStatus = BTStatus.Success;
			mTree.SetCurNode(mParent);
		}
		IBTNode node = mNodeList[curIndex];
		// If the node is Running, means haven't been started yet.
		// If node has already been set, this part won't run until the node returns success.
		if (node.Status == BTStatus.Running)
		{
			mTree.SetCurNode(node);
		}
		// If the node succeded, the node would have set this sequence to curNode in BehaviourTree.
		// When it comes around,
		else if (node.Status == BTStatus.Success)
		{
			curIndex++;
		}
		// If the node has failed, exit sequence.
		else if (node.Status == BTStatus.Failure)
		{
			mStatus = BTStatus.Failure;
			mTree.SetCurNode(mParent);
		}
	}

	public override void ResetNode()
	{
		mStatus = BTStatus.Running;
		curIndex = 0;

		for (int i = 0; i < mNodeList.Count; i++)
			mNodeList[i].ResetNode();
	}
}
