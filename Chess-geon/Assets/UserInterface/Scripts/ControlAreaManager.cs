using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DaburuTools.Action;
using DaburuTools;
using UnityEngine.UI;

public class ControlAreaManager : MonoBehaviour
{
	private static ControlAreaManager sInstance = null;
	public static ControlAreaManager Instance { get { return sInstance; } }

	private static bool mbCardIsBeingDragged;
	public static bool CardIsBeingDragged { get { return mbCardIsBeingDragged; } }
	private static Card mCurCard;
	private static Card mExecutedCard;
	public static Card ExecutedCard { get { return mExecutedCard; } }
	private GameObject mControlBlocker;
	private float mfBoardMinY;

	private static bool mbIsPanelOpen;
	public static bool IsPanelOpen { get { return mbIsPanelOpen; } }
	private Transform mCardAreaCanvas;
	private CanvasGroup[] mPanelCGs;
	private MovementPanelControls mMovementPanelCtrls;

	private SmashPanelControls mSmashPanelCtrls;
	private DrawCardPanelControls mDrawCardPanelCtrls;
	private TempShieldPanelControls mTempShieldPanelCtrls;

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

	private void Setup()
	{
		mControlBlocker = transform.FindChild("ControlBlocker").gameObject;

		mbCardIsBeingDragged = false;
		mCurCard = null;

		mfBoardMinY = Screen.height - Screen.width;


		mCardAreaCanvas = GameObject.Find("CardAreaCanvas").transform;

		mPanelCGs = new CanvasGroup[(int)CardType.NumTypes];
		for (int i = 0; i < (int)CardType.NumTypes; i++)
			mPanelCGs[i] = mCardAreaCanvas.FindChild(((CardType)i).ToString() + "Panel").GetComponent<CanvasGroup>();
		mMovementPanelCtrls = mPanelCGs[(int)CardType.Movement].gameObject.GetComponent<MovementPanelControls>();

		mSmashPanelCtrls = mPanelCGs[(int)CardType.Smash].gameObject.GetComponent<SmashPanelControls>();
		mDrawCardPanelCtrls = mPanelCGs[(int)CardType.DrawCard].gameObject.GetComponent<DrawCardPanelControls>();
		mTempShieldPanelCtrls = mPanelCGs[(int)CardType.TempShield].gameObject.GetComponent<TempShieldPanelControls>();
		mbIsPanelOpen = false;
		HideAllCardPanels();
	}

	#region Main controls
	public void EndTurnButton()
	{
		GameManager.Instance.EndPlayerPhase();
	}

	public void SetControlBlockerEnabled(bool _enabled)
	{
		mControlBlocker.SetActive(_enabled);
	}
	#endregion

	#region EventTrigger Functions
	public void CardBeginDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		if (mbCardIsBeingDragged)
			return;
		if (mCurCard != null)
			return;
		
		mCurCard = pointerData.pointerPressRaycast.gameObject.GetComponent<Card>();
		if (mCurCard.Draggable == false)
		{
			mCurCard = null;
			return;
		}
		mbCardIsBeingDragged = true;
		mCurCard.transform.SetAsLastSibling();
	}

	public void CardDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		if (!mbCardIsBeingDragged)
			return;
		if (pointerData.pointerPressRaycast.gameObject != mCurCard.gameObject)
			return;
		
		mCurCard.transform.position = pointerData.position;
	}

	public void CardEndDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		if (!mbCardIsBeingDragged)
			return;
		if (pointerData.pointerPressRaycast.gameObject != mCurCard.gameObject)
			return;

		mbCardIsBeingDragged = false;

		// Card is used.
		if (pointerData.position.y > mfBoardMinY)
		{
			mCurCard.transform.position = mCurCard.OriginPos;
			mCurCard.transform.SetSiblingIndex(mCurCard.OriginSiblingIndex);
			mCurCard.Execute();

			mExecutedCard = mCurCard;
			mCurCard = null;
		}
		// Card is returned unused.
		else
		{
			MoveToAction moveBack = new MoveToAction(mCurCard.transform, Graph.InverseExponential, mCurCard.OriginPos, 0.3f);
			moveBack.OnActionFinish += () => {
				mCurCard.transform.SetSiblingIndex(mCurCard.OriginSiblingIndex);
				mCurCard = null;
			};
			ActionHandler.RunAction(moveBack);
		}
	}
	#endregion

	#region Card Panel Funcitons
	public void DoneButton()
	{
		HideAllCardPanels();
	}

	public void CancelButton()
	{
		DeckManager.Instance.ReturnExecutedCard();
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
			case CardType.Repeat:
				break;
			case CardType.MovementJoker:
				break;
			case CardType.Smash:
				Instance.mSmashPanelCtrls.UpdatePanel();
				break;
			case CardType.DrawCard:
				Instance.mDrawCardPanelCtrls.UpdatePanel();
				break;
			case CardType.TempShield:
				Instance.mTempShieldPanelCtrls.UpdatePanel();
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

		mbIsPanelOpen = _visible;
	}

	public static void HideAllCardPanels()
	{
		for (int i = 0; i < (int)CardType.NumTypes; i++)
			SetCardPanelVisibility((CardType)i, false);
	}
	#endregion
}
