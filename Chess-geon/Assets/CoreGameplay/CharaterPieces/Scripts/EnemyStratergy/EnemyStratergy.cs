using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyStratergy
{
	public abstract LinkedList<Node> GeneratePath(int _enemyPosX, int _enemyPosY);
}
