using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TempShieldPanelControls : MonoBehaviour
{
	public static CardTier sShieldTier = CardTier.Bronze;

	private Text mInfoText;

	void Awake()
	{
		mInfoText = transform.FindChild("Info Text").GetComponent<Text>();
	}

	public void UpdatePanel()
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

		mInfoText.text = "This Shield card gives " + shieldPoints.ToString() + " units of Shield.";
	}

	// For the button to call.
	public void Use()
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
