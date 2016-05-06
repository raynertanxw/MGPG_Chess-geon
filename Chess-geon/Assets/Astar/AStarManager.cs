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

	private static int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.dungeonBlock.PosX - _goalNode.dungeonBlock.PosX)
			+ Mathf.Abs(_curNode.dungeonBlock.PosY - _goalNode.dungeonBlock.PosY);
	}

	private static int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		return Mathf.Abs(_curNode.dungeonBlock.PosX - _neighbourNode.dungeonBlock.PosX)
			+ Mathf.Abs(_curNode.dungeonBlock.PosY - _neighbourNode.dungeonBlock.PosY);
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
		if (_startNode.dungeonBlock.State == BlockState.Wall || _startNode.dungeonBlock.State == BlockState.Enemy)
			return null;
		if (_goalNode.dungeonBlock.State == BlockState.Wall || _goalNode.dungeonBlock.State == BlockState.Enemy)
			return null;

		openList = new List<Node>();
		AddToOpenList(_startNode);
		_startNode.nodePathCost = 0.0f;
		_startNode.totalCost = HeuristicEstimatedCost(_startNode, _goalNode) + _startNode.nodePathCost;

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
				if (curNeighbourNode.dungeonBlock.State == BlockState.Wall || curNeighbourNode.dungeonBlock.State == BlockState.Enemy)
					continue;

				if (!closedList.Contains(curNeighbourNode))
				{
					//Cost from current node to this neighbour node
					float cost = NeighbourPathCost(curNode, curNeighbourNode);

					//Total Cost So Far from start to this neighbour node
					float totalPathCost = curNode.nodePathCost + cost;

					//Estimated cost for neighbour node to the goal
					float neighbourNodeEstCost = HeuristicEstimatedCost(curNeighbourNode, _goalNode);

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

		//If finished looping and cannot find the goal then return null
		if (curNode.dungeonBlock != _goalNode.dungeonBlock)
		{
			Debug.LogError("Goal Not Found");
			return null;
		}

		//Calculate the path based on the final node
		return ConvertToPath(curNode);
	}
}
