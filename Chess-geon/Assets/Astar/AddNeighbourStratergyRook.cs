using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddNeighbourStratergyRook : AddNeighbourStratergy
{
	public AddNeighbourStratergyRook(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		mnSizeX = _sizeX;
		mnSizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		base.GetNSetNodeNeighbours (_node);

		_node.neighbours = new LinkedList<Node>();

		// Up
		AssignNeighbour(_node.PosX, _node.PosY + 1, _node);
		// Down
		AssignNeighbour(_node.PosX, _node.PosY - 1, _node);
		// Left
		AssignNeighbour(_node.PosX - 1, _node.PosY, _node);
		// Right
		AssignNeighbour(_node.PosX + 1, _node.PosY, _node);
	}
}
