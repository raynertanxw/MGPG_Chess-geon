using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovementJokerPanelControls : MonoBehaviour
{
	public static CardTier sJokerTier = CardTier.Bronze;

	private CanvasGroup mBronzeCG, mSilverCG, mGoldCG;

	void Awake()
	{
		mBronzeCG = transform.Find("BronzeCG").GetComponent<CanvasGroup>();
		mSilverCG = transform.Find("SilverCG").GetComponent<CanvasGroup>();
		mGoldCG = transform.Find("GoldCG").GetComponent<CanvasGroup>();

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
			GridType _randBronzeType = (GridType) Random.Range(0, 3);
			DeckManager.Instance.DrawCard(_randBronzeType);
			break;
		case CardTier.Silver:
			GridType _randSilverType = (GridType) (Random.Range(0, 2) + 3);
			DeckManager.Instance.DrawCard(_randSilverType);
			break;
		}

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.MovementJoker, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}

	public void SelectCard(int _id)
	{
		GridType _movementType = (GridType) _id;
		DeckManager.Instance.DrawCard(_movementType);

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.MovementJoker, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}
	#endregion
}
