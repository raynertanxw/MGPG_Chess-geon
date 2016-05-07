using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyBishop : GridStratergy
{
	public GridStratergyBishop(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		mnSizeX = _sizeX;
		mnSizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours = new LinkedList<Node>();

		// Top-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY + 1, _node);
		// Top-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY + 1, _node);
		// Btm-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY - 1, _node);
		// Btm-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY - 1, _node);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		if (_curNode.neighbours.Contains(_neighbourNode))
			return 1;
		else
			return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
				+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
