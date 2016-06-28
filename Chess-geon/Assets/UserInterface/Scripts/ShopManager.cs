using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	private static ShopManager sInstance = null;
	public static ShopManager Instance { get { return sInstance; } }

	private void OnDestroy()
	{
		if (this == sInstance)
			sInstance = null;
	}

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			Setup();
		}
		else if (sInstance != this)
		{
			Destroy(this.gameObject);
			return;
		}
	}

	private Sprite[] cardSprites = null;
	private string[] cardSpriteNames = null;

	private CanvasGroup mCG;
	private Image[] mCardBtnImages;
	private CardType[] cardTypes;

	private const int knNumCardsForSale = 9;

	private void Setup()
	{
		cardSprites = Resources.LoadAll<Sprite>("Sprites/chessgeon_cards");
		cardSpriteNames = new string[cardSprites.Length];
		for(int i = 0; i < cardSpriteNames.Length; i++) {
			cardSpriteNames[i] = cardSprites[i].name;
		}

		mCG = gameObject.GetComponent<CanvasGroup>();
		mCG.alpha = 0.0f;
		mCG.blocksRaycasts = false;
		mCG.interactable = false;
		mCardBtnImages = new Image[knNumCardsForSale];
		for (int i = 0; i < knNumCardsForSale; i++)
		{
			mCardBtnImages[i] = transform.FindChild("Button_" + i.ToString()).GetComponent<Image>();
		}

		GenerateNewCardsForSale();
	}

	private Sprite GetSprite(CardType _type, CardTier _tier)
	{
		return cardSprites[Array.IndexOf(cardSpriteNames, "Card_" + _type.ToString() + "_" + _tier.ToString())];
	}

	private void GenerateNewCardsForSale()
	{
		cardTypes = new CardType[knNumCardsForSale];
		for (int i = 0; i < knNumCardsForSale; i++)
		{
			cardTypes[i] = (CardType) UnityEngine.Random.Range(1, (int)CardType.NumTypes);
		}

		UpdateShopButtons();
	}

	private void UpdateShopButtons()
	{
		for (int i = 0; i < knNumCardsForSale; i++)
		{
			CardTier tier = (CardTier) (i/3);

			mCardBtnImages[i].sprite = GetSprite(cardTypes[i], tier);
		}
	}

	private void SetShopPanelVisible(bool _visible)
	{
		if (_visible)
		{
			mCG.alpha = 1.0f;
			mCG.blocksRaycasts = true;
			mCG.interactable = true;
		}
		else
		{
			mCG.alpha = 0.0f;
			mCG.blocksRaycasts = false;
			mCG.interactable = false;
		}

		ControlAreaManager.Instance.SetControlBlockerEnabled(_visible);
	}

	#region Funcitons for Buttons
	public void BuyCard(int _cardID)
	{
		// Check the tier of the card being purchased and calculate the price.
		CardTier tier = (CardTier) (_cardID/3);
		int cost = -1;
		switch (tier)
		{
		case CardTier.Bronze:
			cost = 1;
			break;
		case CardTier.Silver:
			cost = 2;
			break;
		case CardTier.Gold:
			cost = 5;
			break;
		}

		// Check if enough coins.
		if (GameManager.Instance.Player.Coins < cost)
		{
			// Display not enough coins message.
			InfoPanelManager.Instance.DisplayInfoPanel("You do not have enough coins!");

			return;
		}

		// Spend the coins and draw the purchased card.
		GameManager.Instance.Player.SpendCoins(cost);
		DeckManager.Instance.DrawSpecificCard(cardTypes[_cardID], tier);
		CloseShop();
	}

	public void CloseShop()
	{
		SetShopPanelVisible(false);
	}

	public void OpenShop()
	{
		SetShopPanelVisible(true);
	}

	public void Shuffle()
	{
		if (GameManager.Instance.Player.Coins < 25)
		{
			// Display not enough coins message.
			InfoPanelManager.Instance.DisplayInfoPanel("You do not have enough coins!");

			return;
		}

		GenerateNewCardsForSale();
	}
	#endregion
}
