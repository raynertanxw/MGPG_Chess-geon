﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DaburuTools.Action;
using DaburuTools;

public class ControlAreaButtons : MonoBehaviour
{
	private static bool mbCardIsBeingDragged;
	public static bool CardIsBeingDragged { get { return mbCardIsBeingDragged; } }
	private static Card mCurCard;

	private float mfBoardMinY;

	private void Awake()
	{
		mbCardIsBeingDragged = false;
		mCurCard = null;

		mfBoardMinY = Screen.height - Screen.width;
	}

	public void EndTurnButton()
	{
		GameManager.Instance.EndPlayerPhase();
	}

	#region EventTrigger Functions
	public void CardBeginDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		if (mbCardIsBeingDragged)
			return;
		if (mCurCard != null)
			return;
		
		mbCardIsBeingDragged = true;
		mCurCard = pointerData.pointerPressRaycast.gameObject.GetComponent<Card>();
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

		if (pointerData.position.y > mfBoardMinY)
		{
			Debug.Log("Card is used");
			mCurCard.transform.position = mCurCard.OriginPos;
			mCurCard.transform.SetSiblingIndex(mCurCard.OriginSiblingIndex);
			mCurCard.Execute();

			mCurCard = null;
		}
		else
		{
			Debug.Log("Card is returned");
			MoveToAction moveBack = new MoveToAction(mCurCard.transform, Graph.InverseExponential, mCurCard.OriginPos, 0.3f);
			moveBack.OnActionFinish += () => {
				mCurCard.transform.SetSiblingIndex(mCurCard.OriginSiblingIndex);
				mCurCard = null;
			};
			ActionHandler.RunAction(moveBack);
		}
	}
	#endregion
}
