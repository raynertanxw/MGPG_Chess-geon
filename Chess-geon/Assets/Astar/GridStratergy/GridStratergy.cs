using UnityEngine;
using System.Collections;

public abstract class GridStratergy
{
	protected int mnSizeX;
	protected int mnSizeY;
	protected Node[,] nodes;

	public abstract void GetNSetNodeNeighbours(Node _node);
	public abstract int HeuristicEstimatedCost(Node _curNode, Node _goalNode);
	public abstract int NeighbourPathCost(Node _curNode, Node _neighbourNode);

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
