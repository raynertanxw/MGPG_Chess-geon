using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyKnight : GridStratergy
{
	public GridStratergyKnight(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		mnSizeX = _sizeX;
		mnSizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours = new LinkedList<Node>();

		/*	0 2 0 3 0
		 * 	1 0 0 0 4
		 * 	0 0 X 0 0
		 * 	8 0 0 0 5
		 * 	0 7 0 6 0
		 * 
		 * Where X is (_node.PosX, _node.PosY)
		 */

		// 1
		AssignNeighbour(_node.PosX - 2, _node.PosY + 1, _node);
		// 2
		AssignNeighbour(_node.PosX - 1, _node.PosY + 2, _node);
		// 3
		AssignNeighbour(_node.PosX + 1, _node.PosY + 2, _node);
		// 4
		AssignNeighbour(_node.PosX + 2, _node.PosY + 1, _node);
		// 5
		AssignNeighbour(_node.PosX + 2, _node.PosY - 1, _node);
		// 6
		AssignNeighbour(_node.PosX + 1, _node.PosY - 2, _node);
		// 7
		AssignNeighbour(_node.PosX - 1, _node.PosY - 2, _node);
		// 8
		AssignNeighbour(_node.PosX - 2, _node.PosY - 1, _node);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		// Better to have actual square values, same as manhatten, otherwise, it biases in moving more,
		// if we were to give it a fixed neighbour path cost of 2 or 1 per move. 
		return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
