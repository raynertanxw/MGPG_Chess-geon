using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddNeighbourStratergyBishop : AddNeighbourStratergy
{
	public AddNeighbourStratergyBishop(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		mnSizeX = _sizeX;
		mnSizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		base.GetNSetNodeNeighbours (_node);

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
}
