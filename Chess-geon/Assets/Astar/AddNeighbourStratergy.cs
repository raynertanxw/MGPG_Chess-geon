using UnityEngine;
using System.Collections;

public class AddNeighbourStratergy
{
	protected int mnSizeX;
	protected int mnSizeY;
	protected Node[,] nodes;

	public virtual void GetNSetNodeNeighbours(Node _node) {}

	protected void AssignNeighbour(int _x, int _y, Node _node)
	{
		if (_x < 0 || _y < 0 || _x >= mnSizeX || _y >= mnSizeY)
		{
//			Debug.LogWarning("Failed Attempt to Assigne Node: Neighbour out of index.");
			return;
		}

		_node.neighbours.AddFirst(nodes[_x, _y]);
	}
}
