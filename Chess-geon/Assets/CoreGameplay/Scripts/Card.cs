using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum CardType { Movement, Repeat, MovementJoker, BreakWall, DrawCard, TempShield }

public class Card : MonoBehaviour
{
	private Vector2 mvec2OriginPos;
	public Vector2 OriginPos { get { return mvec2OriginPos; } }
	private int mnOriginSiblingIndex;
	public int OriginSiblingIndex { get { return mnOriginSiblingIndex; } }

	void Start()
	{
		// Has to be after Awake, else everything is at bottom left origin.
		mvec2OriginPos = transform.position;

		mnOriginSiblingIndex = transform.GetSiblingIndex();
	}
	
	void Update()
	{
	
	}
}
