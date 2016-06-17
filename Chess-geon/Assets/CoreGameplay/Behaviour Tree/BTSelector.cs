using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BTSelector : IBTNode
{
	int curIndex = 0;

	public BTSelector(params IBTNode[] _nodes)
	{
		mNodeList = new List<IBTNode>();

		for (int i = 0; i < _nodes.Length; i++)
		{
			mNodeList.Add(_nodes[i]);
			_nodes[i].SetParent(this);
		}
	}

	public void AddBTNode(params IBTNode[] _nodes)
	{
		for (int i = 0; i < _nodes.Length; i++)
		{
			mNodeList.Add(_nodes[i]);
			_nodes[i].SetParent(this);
		}
	}

	public override List<IBTNode> GetChildren()
	{
		return mNodeList;
	}

	public override void Execute()
	{
		if (curIndex >= mNodeList.Count)
		{
			mStatus = BTStatus.Failure;
			mTree.SetCurNode(mParent);
			return;
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
			mStatus = BTStatus.Success;
			mTree.SetCurNode(mParent);
		}
		// If the node has failed, exit sequence.
		else if (node.Status == BTStatus.Failure)
		{
			curIndex++;
		}
	}

	public override void ResetNode()
	{
		mStatus = BTStatus.Running;
		curIndex = 0;

		for (int i = 0; i < mNodeList.Count; i++)
			mNodeList[i].ResetNode();
	}

	public override void SetTree(BehaviourTree _mTree)
	{
		mTree = _mTree;

		for (int i = 0; i < mNodeList.Count; i++)
			mNodeList[i].SetTree(_mTree);
	}
}
