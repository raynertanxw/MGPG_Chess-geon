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

	private CardData mCardData = new CardData(CardType.None, CardTier.None, GridType.Rook);
	public CardType Type { get { return mCardData.Type; } }
	public GridType MoveType { get { return mCardData.MoveType; } }
	public CardTier Tier { get { return mCardData.Tier; } }
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
		CardAlgorithms[(int)mCardData.Type].ExecuteCard(mCardData.Tier, mCardData.MoveType);
		ControlAreaManager.SetCardPanelVisibility(mCardData.Type, true);
		ToggleCard(false);
	}

	public void NewMovementCard()
	{
		mCardData.Type = CardType.Movement;
		mCardData.MoveType = (GridType)UnityEngine.Random.Range(0, (int)GridType.Count);
		mCardData.Tier = CardTier.None;
	}

	public void SpecificMovementCard(GridType _moveType)
	{
		mCardData.Type = CardType.Movement;
		mCardData.MoveType = _moveType;
		mCardData.Tier = CardTier.None;
	}

	public void SetCard(CardType _type, CardTier _tier)
	{
		mCardData.Type = _type;
		mCardData.Tier = _tier;
	}

	public void UpdateSprite()
	{
		switch (mCardData.Type)
		{
		case CardType.None:
			mImage.sprite = null;
			break;
		case CardType.Movement:
			mImage.sprite = cardSprites[Array.IndexOf(cardSpriteNames, "Card_Movement_" + mCardData.MoveType.ToString())];
			break;
		default:
			mImage.sprite = cardSprites[Array.IndexOf(cardSpriteNames, "Card_" + mCardData.Type.ToString() + "_" + mCardData.Tier.ToString())];
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
		mCardData.Type = _sourceCard.Type;
		mCardData.MoveType = _sourceCard.MoveType;
		mCardData.Tier = _sourceCard.Tier;

		mImage.sprite = _sourceCard.CardImage.sprite;
	}
}
