﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStratergyRook : EnemyStratergy
{
	public override LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY)
	{
		GridManager grid = DungeonManager.Instance.Grids[(int)GridType.Rook];
		LinkedList<Node> retPath = AStarManager.FindPath(
			grid.nodes[_enemyPosX, _enemyPosY],
			grid.nodes[GameManager.Instance.Player.PosX, GameManager.Instance.Player.PosY],
			grid);

		return retPath;
	}
}
