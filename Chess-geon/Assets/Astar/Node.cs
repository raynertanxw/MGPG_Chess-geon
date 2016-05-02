using UnityEngine;
using System.Collections;

public class Node
{
    public float nodePathCost;
    public float heuristicCost;
    public DungeonBlock dungeonBlock;
    public Node parent;

	public Node(DungeonBlock _dungeonBlock)
    {
        this.nodePathCost = 1.0f;
        this.heuristicCost = 0.0f;
        this.dungeonBlock = _dungeonBlock;
        this.parent = null;
    }

    public int CompareTo(Node _otherNode)
    {
        if (this.heuristicCost < _otherNode.heuristicCost)
            return -1;
        if (this.heuristicCost > _otherNode.heuristicCost)
            return 1;

        return 0;
    }
}
