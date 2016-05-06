using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridType { Rook, Bishop, Knight, King }

public class GridManager
{
    private int mnSizeX;
    private int mnSizeY;
    public Node[,] nodes { get; set; }
	private DungeonManager mDungeonManager = null;
	AddNeighbourStratergy addNeighbourAlgorithm;

	public GridManager(DungeonManager _dungeon, GridType _type)
    {
		mDungeonManager = _dungeon;
		mnSizeX = mDungeonManager.SizeX;
		mnSizeY = mDungeonManager.SizeY;

        nodes = new Node[mnSizeX, mnSizeY];

		switch (_type)
		{
		case GridType.Rook:
			addNeighbourAlgorithm = new AddNeighbourStratergyRook(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.Bishop:
			addNeighbourAlgorithm = new AddNeighbourStratergyBishop(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.Knight:
			break;
		case GridType.King:
			break;
		}

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
				addNeighbourAlgorithm.GetNSetNodeNeighbours(nodes[x, y]);
			}
		}
    }
}
