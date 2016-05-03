using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager
{
    private int mnSizeX;
    private int mnSizeY;
    public Node[,] nodes { get; set; }

    public GridManager(DungeonManager _dungeon)
    {
        mnSizeX = _dungeon.SizeX;
        mnSizeY = _dungeon.SizeY;

        nodes = new Node[mnSizeX, mnSizeY];

        for (int y = 0; y < mnSizeY; y++)
        {
            for (int x = 0; x < mnSizeX; x++)
            {
                nodes[x, y] = new Node(_dungeon.DungeonBlocks[x, y]);
            }
        }
    }

    public void GetNeighbours(Node _node, List<Node> _neighbours)
    {

    }

    private void AssignNeighbour(int _x, int _y, List<Node> _neighbours)
    {
        
    }
}
