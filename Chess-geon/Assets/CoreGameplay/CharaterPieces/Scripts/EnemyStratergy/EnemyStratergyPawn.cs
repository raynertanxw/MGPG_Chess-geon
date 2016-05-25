using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStratergyPawn : EnemyStratergy
{
	public override LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY)
	{
//		GridManager grid = DungeonManager.Instance.Grids[(int)GridType.Knight];
//		LinkedList<Node> retPath = AStarManager.FindPath(
//			grid.nodes[_enemyNode.PosX, _enemyNode.PosY],
//			grid.nodes[_playerNode.PosX, _playerNode.PosY],
//			grid);
//
//		return retPath;
		return null;
	}
}
