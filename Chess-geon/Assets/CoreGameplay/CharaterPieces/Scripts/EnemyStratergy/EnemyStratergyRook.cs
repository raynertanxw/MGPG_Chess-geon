using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStratergyRook : EnemyStratergy
{
	public override LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY)
	{
		GridManager grid = DungeonManager.Instance.Grids[(int)GridType.Rook];
		LinkedList<Node> retPath = null;

		retPath = AStarManager.FindPath(
			grid.nodes[_enemyPosX, _enemyPosY],
			grid.nodes[GameManager.Instance.Player.PosX, GameManager.Instance.Player.PosY],
			grid);

		if (retPath == null)
		{
			/* Here we try the nearest nodes. If the second AStarPath still returns null,
			 * it could be that the enemy piece is completely blocked off.
			 * So don't waste more time trying to find a path, just random
			 * move to the adjacent blocks.
			 */

			// Spiral Nearest when the adjacent tiles are not available. Spiral max radius of 3 tiles.
			int targetPosX = GameManager.Instance.Player.PosX;
			int targetPosY = GameManager.Instance.Player.PosY;

			bool emptyTileFound = false;
			int increment = 1;
			int iMax = 1;
			while ((iMax - 1) < 5)
			{
				for (int y = 0; y < iMax; y++)
				{
					targetPosY += increment;
					if (DungeonManager.Instance.IsCellEmpty(targetPosX, targetPosY))
					{
						retPath = AStarManager.FindPath(
							grid.nodes[_enemyPosX, _enemyPosY],
							grid.nodes[targetPosX, targetPosY],
							grid);
						emptyTileFound = true;
						break;
					}
				}

				for (int x = 0; x < iMax; x++)
				{
					targetPosX += increment;
					if (DungeonManager.Instance.IsCellEmpty(targetPosX, targetPosY))
					{
						retPath = AStarManager.FindPath(
							grid.nodes[_enemyPosX, _enemyPosY],
							grid.nodes[targetPosX, targetPosY],
							grid);
						emptyTileFound = true;
						break;
					}
				}

				if (emptyTileFound)
					break;

				iMax++;
				increment *= -1;
			}

			// If the path is still null (maybe enemy is blocked off by structure), just random anyhow position to the adjacent tiles.
			if (retPath == null)
			{
				LinkedList<Node> neighbours = grid.nodes[_enemyPosX, _enemyPosY].neighbours;
				for (LinkedListNode<Node> curNode = neighbours.First; curNode != null; curNode = curNode.Next)
				{
					if (DungeonManager.Instance.IsCellEmpty(curNode.Value.PosX, curNode.Value.PosY))
					{
						retPath = new LinkedList<Node>();
						retPath.AddFirst(grid.nodes[curNode.Value.PosX, curNode.Value.PosY]);
						retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
						break;
					}
				}
			}
		}

		// If STILL null (can't move all; all neighbours blocked), then no choice, return null and don't move.

		return retPath;
	}
}
