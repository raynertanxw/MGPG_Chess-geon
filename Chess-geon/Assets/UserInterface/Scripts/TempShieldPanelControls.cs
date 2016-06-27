using UnityEngine;
using System.Collections;

public class TempShieldPanelControls : MonoBehaviour
{
	public static CardTier sShieldTier = CardTier.Bronze;

	// For the button to call.
	public void Dismiss()
	{
		int shieldPoints = 0;
		switch (sShieldTier)
		{
		case CardTier.Bronze:
			shieldPoints = 1;
			break;
		case CardTier.Silver:
			shieldPoints = 3;
			break;
		case CardTier.Gold:
			shieldPoints = 5;
			break;
		}
		GameManager.Instance.Player.AddShieldPoints(shieldPoints);

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.TempShield, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}
}
