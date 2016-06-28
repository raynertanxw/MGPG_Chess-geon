using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;

public class DrawCardPanelControls : MonoBehaviour
{
	public static CardTier sDrawTier = CardTier.Bronze;

	private Text mInfoText;

	void Awake()
	{
		mInfoText = transform.FindChild("Info Text").GetComponent<Text>();
	}

	public void UpdatePanel()
	{
		int numCards = 0;
		switch (sDrawTier)
		{
		case CardTier.Bronze:
			numCards = 1;
			break;
		case CardTier.Silver:
			numCards = 3;
			break;
		case CardTier.Gold:
			numCards = 5;
			break;
		}

		// Calcualte number of cards wasted.
		// Number of cards to be drawn - (number of empty slots after this card is used).
		int numWasted = numCards - (DeckManager.knMaxCardsInHand - (DeckManager.Instance.GetNumCardsInHand() - 1));
		if (numWasted > 0)
			mInfoText.text = "This card will draw " + numCards.ToString() + " card(s).\n\nUsing this now will waste\n" + numWasted.ToString() + " cards.";
		else
			mInfoText.text = "This card will draw " + numCards.ToString() + " card(s).\n\nUsing this now will not waste\nany cards.";
	}

	// For the button to call.
	public void Use()
	{
		int numCards = 0;
		switch (sDrawTier)
		{
		case CardTier.Bronze:
			numCards = 1;
			break;
		case CardTier.Silver:
			numCards = 3;
			break;
		case CardTier.Gold:
			numCards = 5;
			break;
		}

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();

		// Draw cards.
		DelayAction draw = new DelayAction(0.3f);
		draw.OnActionFinish += () => { DeckManager.Instance.DrawCard(); };
		ActionRepeat repeatedDraw = new ActionRepeat(draw, numCards);
		ActionHandler.RunAction(repeatedDraw);

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.DrawCard, false);
	}
}
