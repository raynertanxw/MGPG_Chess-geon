using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RepeatPanelControls : MonoBehaviour
{
	public static CardTier sRepeatTier = CardTier.Bronze;
	private static int snNumRepeatsLeft = 0;
	public static int NumRepeatsLeft { get { return snNumRepeatsLeft; } }

	private Text mInfoText;

	void Awake()
	{
		mInfoText = transform.FindChild("Info Text").GetComponent<Text>();
	}

	public void UpdatePanel()
	{
		int newNumRepeats;

		// If a Repeat has already been used. Player is stacking repeats
		if (snNumRepeatsLeft > 0)
		{
			newNumRepeats = snNumRepeatsLeft;
			switch (sRepeatTier)
			{
			case CardTier.Bronze:
				newNumRepeats *= 2;
				break;
			case CardTier.Silver:
				newNumRepeats *= 3;
				break;
			case CardTier.Gold:
				newNumRepeats *= 5;
				break;
			}
		}
		// Else, it is the first repeat used.
		else
		{
			newNumRepeats = 0;
			switch (sRepeatTier)
			{
			case CardTier.Bronze:
				newNumRepeats = 2;
				break;
			case CardTier.Silver:
				newNumRepeats = 3;
				break;
			case CardTier.Gold:
				newNumRepeats = 5;
				break;
			}
		}

		mInfoText.text = "Use this to repeat next card " + newNumRepeats.ToString() + " times.";
	}

	public static void UseRepeat()
	{
		if (snNumRepeatsLeft <= 0)
			return;

		snNumRepeatsLeft--;
		PlayerInfoManager.Instance.UpdateRepeat(snNumRepeatsLeft);
	}

	public static void ClearRepeats()
	{
		snNumRepeatsLeft = 0;
		PlayerInfoManager.Instance.UpdateRepeat(snNumRepeatsLeft);
	}
		
	#region Button Functions
	public void Use()
	{
		// If a Repeat has already been used. Player is stacking repeats
		if (snNumRepeatsLeft > 0)
		{
			switch (sRepeatTier)
			{
			case CardTier.Bronze:
				snNumRepeatsLeft *= 2;
				break;
			case CardTier.Silver:
				snNumRepeatsLeft *= 3;
				break;
			case CardTier.Gold:
				snNumRepeatsLeft *= 5;
				break;
			}
		}
		// Else, it is the first repeat used.
		else
		{
			switch (sRepeatTier)
			{
			case CardTier.Bronze:
				snNumRepeatsLeft = 2;
				break;
			case CardTier.Silver:
				snNumRepeatsLeft = 3;
				break;
			case CardTier.Gold:
				snNumRepeatsLeft = 5;
				break;
			}
		}

		PlayerInfoManager.Instance.UpdateRepeat(snNumRepeatsLeft);

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.Repeat, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}
	#endregion
}
