using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarManager
{
	private static List<Node> openList, closedList;

	private static LinkedList<Node> ConvertToPath(Node _node)
	{
		LinkedList<Node> path = new LinkedList<Node>();
		while (_node != null)
		{
			path.AddFirst(_node);
			_node = _node.parent;
		}
		return path;
	}

	private static void AddToOpenList(Node _node)
	{
		openList.Add(_node);
		openList.Sort((n1, n2) => n1.totalCost.CompareTo(n2.totalCost));
	}

	private static void RemoveFromOpenList(Node _node)
	{
		openList.Remove(_node);
		openList.Sort((n1, n2) => n1.totalCost.CompareTo(n2.totalCost));
	}

	public static LinkedList<Node> FindPath(Node _startNode, Node _goalNode, GridManager _grid)
	{
		if (_startNode.State != BlockState.EnemyPiece)
			return null;
		if (_goalNode.State != BlockState.Empty)
			return null;

		// RESET ALL NODES.
		IEnumerator gridEnumurator = _grid.nodes.GetEnumerator();
		while (gridEnumurator.MoveNext())
		{
			(gridEnumurator.Current as Node).Reset();
		}

		openList = new List<Node>();
		AddToOpenList(_startNode);
		_startNode.nodePathCost = 0.0f;
		_startNode.totalCost = _grid.GridAlgorithms.HeuristicEstimatedCost(_startNode, _goalNode);// + _startNode.nodePathCost;

		closedList = new List<Node>();
		Node curNode = null;

		while (openList.Count > 0)
		{
			// Check if the closed List contains the _goalNode.
			if (closedList.Contains(_goalNode))
			{
				return ConvertToPath(curNode);
			}

			curNode = openList[0];

			for (LinkedListNode<Node> curLinkedNode = curNode.neighbours.First; curLinkedNode != null; curLinkedNode = curLinkedNode.Next)
			{
				Node curNeighbourNode = (Node)curLinkedNode.Value;
				if (curNeighbourNode.State != BlockState.Empty)
					continue;

				if (!closedList.Contains(curNeighbourNode))
				{
					//Cost from current node to this neighbour node
					float cost = _grid.GridAlgorithms.NeighbourPathCost(curNode, curNeighbourNode);

					//Total Cost So Far from start to this neighbour node
					float totalPathCost = curNode.nodePathCost + cost;

					//Estimated cost for neighbour node to the goal
					float neighbourNodeEstCost = _grid.GridAlgorithms.HeuristicEstimatedCost(curNeighbourNode, _goalNode);

					if (openList.Contains(curNeighbourNode)) // Calculated before?
					{
						if (totalPathCost < curNeighbourNode.nodePathCost)
						{
							curNeighbourNode.nodePathCost = totalPathCost;
							curNeighbourNode.parent = curNode;
							curNeighbourNode.totalCost = totalPathCost + neighbourNodeEstCost;
						}
					}
					else
					{
						curNeighbourNode.nodePathCost = totalPathCost;
						curNeighbourNode.parent = curNode;
						curNeighbourNode.totalCost = totalPathCost + neighbourNodeEstCost;

						//Add the neighbour node to the list if not already existed in the list
						AddToOpenList(curNeighbourNode);
					}
				}
			}

			closedList.Add(curNode);
			RemoveFromOpenList(curNode);
		}

		if (closedList.Contains(_goalNode))
			return ConvertToPath(curNode);

		return null;
	}
}
