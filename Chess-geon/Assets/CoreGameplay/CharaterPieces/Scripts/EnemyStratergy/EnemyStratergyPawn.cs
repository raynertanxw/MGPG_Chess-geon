using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStratergyPawn : EnemyStratergy
{
	public override LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY)
	{
		GridManager grid = DungeonManager.Instance.Grids[(int)GridType.Pawn];
		LinkedList<Node> retPath = null;

		// Attack
		if ((_enemyPosY - GameManager.Instance.Player.PosY) == 1)	// Player below
		{
			// Check if Player is below right.
			if (DungeonManager.Instance.IsPlayerPos(_enemyPosX + 1, _enemyPosY - 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX + 1, _enemyPosY - 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
				return retPath;
			}

			// Check if Player is below left.
			if (DungeonManager.Instance.IsPlayerPos(_enemyPosX - 1, _enemyPosY - 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX - 1, _enemyPosY - 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
				return retPath;
			}
		}
		else if ((_enemyPosY - GameManager.Instance.Player.PosY) == -1)	// Player above
		{
			// Check if Player is above right.
			if (DungeonManager.Instance.IsPlayerPos(_enemyPosX + 1, _enemyPosY + 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX + 1, _enemyPosY + 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
				return retPath;
			}

			// Check if Player is above left.
			if (DungeonManager.Instance.IsPlayerPos(_enemyPosX - 1, _enemyPosY + 1))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX - 1, _enemyPosY + 1]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
				return retPath;
			}
		}

		// Movement.
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
			// Random up or down..
			int randDirection = Random.Range(0, 2);
			if (randDirection == 0)
				randDirection = 1;
			else
				randDirection = -1;

			// Check if valid move initially.
			if (DungeonManager.Instance.IsCellEmpty(_enemyPosX, _enemyPosY + randDirection))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY + randDirection]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
			}
			// Check if other only posisble move left is possible.
			else if (DungeonManager.Instance.IsCellEmpty(_enemyPosX, _enemyPosY - randDirection))
			{
				retPath = new LinkedList<Node>();
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY - randDirection]);
				retPath.AddFirst(grid.nodes[_enemyPosX, _enemyPosY]);
			}
			// Otherwise it's just a poor little stuck pawn.
		}

		return retPath;
	}
}
