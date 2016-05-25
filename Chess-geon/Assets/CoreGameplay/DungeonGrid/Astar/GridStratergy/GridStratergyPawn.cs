using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyPawn : GridStratergy
{
	public GridStratergyPawn(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		mnSizeX = _sizeX;
		mnSizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours = new LinkedList<Node>();

//		// Up
//		AssignNeighbour(_node.PosX, _node.PosY + 1, _node);
//		// Down
//		AssignNeighbour(_node.PosX, _node.PosY - 1, _node);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		return Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
