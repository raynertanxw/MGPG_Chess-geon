using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;

public class MovementJokerPanelControls : MonoBehaviour
{
	public static CardTier sJokerTier = CardTier.Bronze;

	private CanvasGroup mBronzeCG, mSilverCG, mGoldCG;
	private Button mCancelButton;

	void Awake()
	{
		mBronzeCG = transform.Find("BronzeCG").GetComponent<CanvasGroup>();
		mSilverCG = transform.Find("SilverCG").GetComponent<CanvasGroup>();
		mGoldCG = transform.Find("GoldCG").GetComponent<CanvasGroup>();

		mCancelButton = transform.Find("Cancel Button").GetComponent<Button>();

		HideAllCanvasGroups();
	}

	public void UpdatePanel()
	{
		HideAllCanvasGroups();

		switch (sJokerTier)
		{
		case CardTier.Bronze:
			SetCanvasGroupVisible(mBronzeCG, true);
			break;
		case CardTier.Silver:
			SetCanvasGroupVisible(mSilverCG, true);
			break;
		case CardTier.Gold:
			SetCanvasGroupVisible(mGoldCG, true);
			break;
		}
	}

	#region UI Hide/Display Functions
	private void SetCanvasGroupVisible(CanvasGroup _cg, bool _visible)
	{
		if (_visible)
		{
			_cg.alpha = 1.0f;
			_cg.interactable = true;
			_cg.blocksRaycasts = true;
		}
		else
		{
			_cg.alpha = 0.0f;
			_cg.interactable = false;
			_cg.blocksRaycasts = false;
		}
	}

	private void HideAllCanvasGroups()
	{
		SetCanvasGroupVisible(mBronzeCG, false);
		SetCanvasGroupVisible(mSilverCG, false);
		SetCanvasGroupVisible(mGoldCG, false);
	}
	#endregion

	#region Button Functions
	public void Use()
	{
		switch (sJokerTier)
		{
		case CardTier.Bronze:
			if (RepeatPanelControls.NumRepeatsLeft > 0)
			{
				// Cap it at 5. Won't draw more than 5 cards (due to slot limit).
				int numRepeats = RepeatPanelControls.NumRepeatsLeft;
				if (numRepeats > 5)
					numRepeats = 5;

				for (int i = 0; i < numRepeats; i++)
				{
					GridType _randBronzeType = (GridType) Random.Range(0, 3);
					DeckManager.Instance.DrawCard(_randBronzeType);
				}

				RepeatPanelControls.ClearRepeats();
			}
			else
			{
				GridType _randBronzeType = (GridType) Random.Range(0, 3);
				DeckManager.Instance.DrawCard(_randBronzeType);
			}
			break;
		case CardTier.Silver:
			if (RepeatPanelControls.NumRepeatsLeft > 0)
			{
				// Cap it at 5. Won't draw more than 5 cards (due to slot limit).
				int numRepeats = RepeatPanelControls.NumRepeatsLeft;
				if (numRepeats > 5)
					numRepeats = 5;

				for (int i = 0; i < numRepeats; i++)
				{
					GridType _randSilverType = (GridType) (Random.Range(0, 2) + 3);
					DeckManager.Instance.DrawCard(_randSilverType);
				}

				RepeatPanelControls.ClearRepeats();
			}
			else
			{
				GridType _randSilverType = (GridType) (Random.Range(0, 2) + 3);
				DeckManager.Instance.DrawCard(_randSilverType);
			}
			break;
		}

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.MovementJoker, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}

	public void SelectCard(int _id)
	{
		RepeatPanelControls.UseRepeat();
		if (RepeatPanelControls.NumRepeatsLeft > 0)
		{
			if (DeckManager.Instance.GetNumCardsInHand() < DeckManager.knMaxCardsInHand - 1)
			{
				mCancelButton.interactable = false;

				DelayAction nextDrawDelay = new DelayAction(1.6f);
				nextDrawDelay.OnActionStart += () => {
					EventAnimationController.Instance.SetIsAnimating(true);
					ControlAreaManager.Instance.SetControlBlockerEnabled(true);
				};
				nextDrawDelay.OnActionFinish += () => {
					EventAnimationController.Instance.SetIsAnimating(false);
					ControlAreaManager.SetCardPanelVisibility(CardType.MovementJoker, true);
				};
				ActionHandler.RunAction(nextDrawDelay);
			}
			else
			{
				ControlAreaManager.Instance.SetControlBlockerEnabled(false);
				RepeatPanelControls.ClearRepeats();
				mCancelButton.interactable = true;
			}

			GridType _movementType = (GridType) _id;
			DeckManager.Instance.DrawCard(_movementType);
		}
		else
		{
			GridType _movementType = (GridType) _id;
			DeckManager.Instance.DrawCard(_movementType);

			ControlAreaManager.Instance.SetControlBlockerEnabled(false);
			RepeatPanelControls.ClearRepeats();
			mCancelButton.interactable = true;
		}

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.MovementJoker, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}
	#endregion
}
