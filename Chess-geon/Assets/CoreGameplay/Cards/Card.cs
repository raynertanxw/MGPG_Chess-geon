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

	private CardType mCardType = CardType.None;
	public CardType Type { get { return mCardType; } }
	private GridType mCardMovementType = GridType.Rook;
	public GridType MoveType { get { return mCardMovementType; } }
	private CardTier mCardTier = CardTier.None;
	public CardTier Tier { get { return mCardTier; } }
	private Image mImage;
	public Image CardImage { get { return mImage; } }
	private bool mbEnabled;
	public bool Enabled { get { return mbEnabled; } }
	private bool mbDraggable;
	public bool Draggable { get { return mbDraggable; } }
	public void SetCardDraggable(bool _draggable) { mbDraggable = _draggable; }

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

		ToggleCard(false);
	}

	void Start()
	{
		// Has to be after Awake, else everything is at bottom left origin.
		mvec2OriginPos = transform.position;

		mnOriginSiblingIndex = transform.GetSiblingIndex();
	}

	public void Execute()
	{
		// Execute the card & set the respective panel to visible.
		CardAlgorithms[(int)mCardType].ExecuteCard(mCardTier, mCardMovementType);
		ControlAreaManager.SetCardPanelVisibility(mCardType, true);
		ToggleCard(false);
	}

	public void NewMovementCard()
	{
		mCardType = CardType.Movement;
		mCardMovementType = (GridType)UnityEngine.Random.Range(0, (int)GridType.Count);
		mCardTier = CardTier.None;
	}

	public void SetCard(CardType _type, CardTier _tier)
	{
		mCardType = _type;
		mCardTier = _tier;
	}

	public void UpdateSprite()
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
	}

	public void ToggleCard(bool _enabled)
	{
		mImage.enabled = _enabled;
		mbEnabled = _enabled;
	}

	public void CopyCardData(Card _sourceCard)
	{
		mCardType = _sourceCard.Type;
		mCardMovementType = _sourceCard.MoveType;
		mCardTier = _sourceCard.Tier;

		mImage.sprite = _sourceCard.CardImage.sprite;
	}
}
