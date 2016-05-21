using UnityEngine;
using System.Collections;

public class PieceRenderController : MonoBehaviour
{
	private SpriteRenderer mSpriteRen;

	private void Awake()
	{
		mSpriteRen = GetComponent<SpriteRenderer>();

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}
}
