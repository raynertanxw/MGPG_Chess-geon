using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public float nodePathCost;
    public float totalCost;	// To get Heuristic, totalCost - nodePathCost.
    public DungeonBlock dungeonBlock;
    public Node parent;
	public LinkedList<Node> neighbours;

	public Node(DungeonBlock _dungeonBlock)
    {
        this.nodePathCost = 1.0f;
		this.totalCost = 0.0f;
        this.dungeonBlock = _dungeonBlock;
        this.parent = null;
    }

    public int CompareTo(Node _otherNode)
    {
		if (this.totalCost < _otherNode.totalCost)
            return -1;
		if (this.totalCost > _otherNode.totalCost)
            return 1;

        return 0;
    }

	// TODO: Might not need. Do check.
	public void Reset()
	{
		this.nodePathCost = 1.0f;
		this.totalCost = 0.0f;
		this.parent = null;
	}
}
