using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public enum CardType { Movement, Repeat, MovementJoker, Smash, DrawCard, TempShield, NumTypes, None }
public enum CardTier { Bronze, Silver, Gold, None }

public class Card : MonoBehaviour
{
	private static Sprite[] cardSprites = null;
	private static string[] cardSpriteNames = null;

	private Vector2 mvec2OriginPos;
	public Vector2 OriginPos { get { return mvec2OriginPos; } }
	private int mnOriginSiblingIndex;
	public int OriginSiblingIndex { get { return mnOriginSiblingIndex; } }

	[SerializeField]
	private CardType mCardType = CardType.Movement;
	[SerializeField]
	private GridType mCardMovementType = GridType.Rook;
	[SerializeField]
	private CardTier mCardTier = CardTier.None;
	private bool mbNeedUpdateSprite;
	private Image mImage;

	private CardStrategy[] CardAlgorithms;

	void Awake()
	{
		CardAlgorithms = new CardStrategy[(int)CardType.NumTypes];
		CardAlgorithms[(int)CardType.Movement] = new MovementCardStrategy();
		CardAlgorithms[(int)CardType.Repeat] = new RepeatCardStrategy();
		CardAlgorithms[(int)CardType.MovementJoker] = new MovementJokerCardStrategy();
		CardAlgorithms[(int)CardType.Smash] = new SmashCardStrategy();
		CardAlgorithms[(int)CardType.DrawCard] = new DrawCardCardStrategy();
		CardAlgorithms[(int)CardType.TempShield] = new TempShieldCardStrategy();

		mImage = gameObject.GetComponent<Image>();

		if (cardSprites == null)
		{
			cardSprites = Resources.LoadAll<Sprite>("Sprites/chessgeon_cards");
			cardSpriteNames = new string[cardSprites.Length];
		}

		for(int i = 0; i < cardSpriteNames.Length; i++) {
			cardSpriteNames[i] = cardSprites[i].name;
		}

		mbNeedUpdateSprite = true;
	}

	void Start()
	{
		// Has to be after Awake, else everything is at bottom left origin.
		mvec2OriginPos = transform.position;

		mnOriginSiblingIndex = transform.GetSiblingIndex();
	}

	void Update()
	{
		if (mbNeedUpdateSprite)
			UpdateSprite();
	}

	public void Execute()
	{
		// Execute the card & set the respective panel to visible.
		CardAlgorithms[(int)mCardType].ExecuteCard(mCardTier, mCardMovementType);
		CardAreaButtons.SetCardPanelVisibility(mCardType, true);
		ChangeCard();	// TEMP ONLY
	}

	public void ChangeCard()
	{
		if (UnityEngine.Random.value > 0.5f)
		{
			mCardType = CardType.Movement;
			mCardMovementType = (GridType)UnityEngine.Random.Range(0, (int)GridType.Count);
			mCardTier = CardTier.None;
		}
		else
		{
			mCardType = (CardType)UnityEngine.Random.Range(0, (int)CardType.NumTypes);
			mCardTier = (CardTier)UnityEngine.Random.Range(0, 3);	// TEMP
		}

		mbNeedUpdateSprite = true;
	}

	private void UpdateSprite()
	{
		switch (mCardType)
		{
		case CardType.None:
			mImage.sprite = null;
			break;
		case CardType.Movement:
			mImage.sprite = cardSprites[Array.IndexOf(cardSpriteNames, "Card_Movement_" + mCardMovementType.ToString())];
			break;
		default:
			mImage.sprite = cardSprites[Array.IndexOf(cardSpriteNames, "Card_" + mCardType.ToString() + "_" + mCardTier.ToString())];
			break;
		}

		mbNeedUpdateSprite = false;
	}
}
