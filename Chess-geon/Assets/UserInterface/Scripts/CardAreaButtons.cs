using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardAreaButtons : MonoBehaviour
{
	private static CardAreaButtons sInstance = null;
	public static CardAreaButtons Instance { get { return sInstance; } }

	private static bool mbPanelOpened;
	public static bool PanelOpened { get { return mbPanelOpened; } }
	private CanvasGroup[] mPanelCGs;
	private Image mBlockingPanel;

	private MovementPanelControls mMovementPanelCtrls;

	private void Awake()
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

	private void OnDestroy()
	{
		if (this == sInstance)
			sInstance = null;
	}

	private void Setup()
	{
		mBlockingPanel = transform.FindChild("BlockingPanel").GetComponent<Image>();
		mBlockingPanel.enabled = false;

		mPanelCGs = new CanvasGroup[(int)CardType.NumTypes];

		for (int i = 0; i < (int)CardType.NumTypes; i++)
			mPanelCGs[i] = transform.FindChild(((CardType)i).ToString() + "Panel").GetComponent<CanvasGroup>();

		mMovementPanelCtrls = mPanelCGs[(int)CardType.Movement].gameObject.GetComponent<MovementPanelControls>();
			
		mbPanelOpened = false;
		HideAllCardPanels();
	}

	public void DoneButton()
	{
		HideAllCardPanels();
	}

	public static void SetCardPanelVisibility(CardType _type, bool _visible)
	{
		if (_visible)
		{
			switch(_type)
			{
			case CardType.Movement:
				Instance.mMovementPanelCtrls.UpdatePanel();
				break;
			}

			Instance.mPanelCGs[(int)_type].alpha = 1.0f;
			Instance.mPanelCGs[(int)_type].interactable = true;
			Instance.mPanelCGs[(int)_type].blocksRaycasts = true;
		}
		else
		{
			Instance.mPanelCGs[(int)_type].alpha = 0.0f;
			Instance.mPanelCGs[(int)_type].interactable = false;
			Instance.mPanelCGs[(int)_type].blocksRaycasts = false;
		}

		Instance.mBlockingPanel.enabled = _visible;
		mbPanelOpened = _visible;
	}

	public static void HideAllCardPanels()
	{
		for (int i = 0; i < (int)CardType.NumTypes; i++)
			SetCardPanelVisibility((CardType)i, false);
	}
}
