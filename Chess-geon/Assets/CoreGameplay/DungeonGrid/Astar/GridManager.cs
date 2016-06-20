using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridType { Pawn, Rook, Bishop, Knight, King, Count }

public class GridManager
{
    private int mnSizeX;
    private int mnSizeY;
    public Node[,] nodes { get; set; }
	private DungeonManager mDungeonManager = null;
	private GridStratergy mGridAlgorithms;
	public GridStratergy GridAlgorithms { get { return mGridAlgorithms; } }
	private GridType mGridType;
	public GridType gridType { get { return mGridType; } }

	public GridManager(DungeonManager _dungeon, GridType _type)
    {
		mDungeonManager = _dungeon;
		mnSizeX = mDungeonManager.SizeX;
		mnSizeY = mDungeonManager.SizeY;
		mGridType = _type;

        nodes = new Node[mnSizeX, mnSizeY];

		switch (_type)
		{
		case GridType.Rook:
			mGridAlgorithms = new GridStratergyRook(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.Bishop:
			mGridAlgorithms = new GridStratergyBishop(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.Knight:
			mGridAlgorithms = new GridStratergyKnight(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.King:
			mGridAlgorithms = new GridStratergyKing(mnSizeX, mnSizeY, nodes);
			break;
		case GridType.Pawn:
			mGridAlgorithms = new GridStratergyPawn(mnSizeX, mnSizeY, nodes);
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
				mGridAlgorithms.GetNSetNodeNeighbours(nodes[x, y]);
			}
		}
    }
}
