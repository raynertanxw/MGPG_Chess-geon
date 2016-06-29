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

		if (RepeatPanelControls.NumRepeatsLeft > 0)
		{
			numCards *= RepeatPanelControls.NumRepeatsLeft;
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

		if (RepeatPanelControls.NumRepeatsLeft > 0)
		{
			numCards *= RepeatPanelControls.NumRepeatsLeft;
			// Cap at 5, no point going more than that, just wastes processing power.
			if (numCards > 5)
				numCards = 5;

			// Clear the repeats.
			RepeatPanelControls.ClearRepeats();
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
