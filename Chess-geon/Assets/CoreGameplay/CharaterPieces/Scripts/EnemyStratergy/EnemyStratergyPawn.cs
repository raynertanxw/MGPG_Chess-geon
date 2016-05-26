using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStratergyPawn : EnemyStratergy
{
	public override LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY)
	{
		GridManager grid = DungeonManager.Instance.Grids[(int)GridType.Pawn];
		LinkedList<Node> retPath = null;

		if (_enemyPosY > GameManager.Instance.Player.PosY)
		{
			if (DungeonManager.Instance.IsCellEmpty(_enemyPosX, _enemyPosY - 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY - 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
			}
		}
		else if (_enemyPosY < GameManager.Instance.Player.PosY)
		{
			if (DungeonManager.Instance.IsCellEmpty(_enemyPosX, _enemyPosY + 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY + 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
			}
		}
		else	// Pawn is in same Y pos as player.
		{
			// Do Nothing.
		}

		return retPath;
	}
}
