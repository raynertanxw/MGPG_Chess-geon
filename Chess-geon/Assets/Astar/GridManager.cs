using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager
{
    private int mnSizeX;
    private int mnSizeY;
    public Node[,] nodes { get; set; }
	private DungeonManager mDungeonManager = null;

    public GridManager(DungeonManager _dungeon)
    {
		mDungeonManager = _dungeon;
		mnSizeX = mDungeonManager.SizeX;
		mnSizeY = mDungeonManager.SizeY;

        nodes = new Node[mnSizeX, mnSizeY];

        for (int y = 0; y < mnSizeY; y++)
        {
            for (int x = 0; x < mnSizeX; x++)
            {
                nodes[x, y] = new Node(_dungeon.DungeonBlocks[x, y]);
            }
        }

		for (int y = 0; y < mnSizeY; y++)
		{
			for (int x = 0; x < mnSizeX; x++)
			{
				GetNSetNodeNeighbours(nodes[x, y]);
			}
		}
    }

    public void GetNSetNodeNeighbours(Node _node)
    {
		_node.neighbours = new LinkedList<Node>();

		// Up
		AssignNeighbour(_node.dungeonBlock.PosX, _node.dungeonBlock.PosY + 1, _node);
		// Down
		AssignNeighbour(_node.dungeonBlock.PosX, _node.dungeonBlock.PosY - 1, _node);
		// Left
		AssignNeighbour(_node.dungeonBlock.PosX - 1, _node.dungeonBlock.PosY, _node);
		// Right
		AssignNeighbour(_node.dungeonBlock.PosX + 1, _node.dungeonBlock.PosY, _node);
    }

	private void AssignNeighbour(int _x, int _y, Node _node)
    {
		if (_x < 0 || _y < 0 || _x >= mnSizeX || _y >= mnSizeY)
		{
//			Debug.LogWarning("Failed Attempt to Assigne Node: Neighbour out of index.");
			return;
		}

		_node.neighbours.AddFirst(nodes[_x, _y]);
    }
}
