using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;
using DaburuTools;

public class DeckManager : MonoBehaviour
{
	private static DeckManager sInstance = null;
	public static DeckManager Instance { get { return sInstance; } }

	private void OnDestroy()
	{
		if (this == sInstance)
			sInstance = null;
	}

	void Awake()
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

	private static Vector3 svec3DrawPos;
	private static Vector3 svec3UseCardPos;

	public Sprite CardBackSprite;
	private const int knMaxCardsInHand = 5;
	private Transform[] mCardTransforms;
	private Card[] mCards;

	private void Setup()
	{
		mCardTransforms = new Transform[knMaxCardsInHand];
		mCards = new Card[knMaxCardsInHand];
		for (int i = 0; i < knMaxCardsInHand; i++)
		{
			mCardTransforms[i] = transform.FindChild("Card Slot " + i.ToString());
			mCards[i] = mCardTransforms[i].GetComponent<Card>();
		}
	}

	void Start()
	{
		svec3DrawPos = transform.FindChild("CardDrawPos").position;
		svec3UseCardPos = transform.FindChild("UseCardPos").position;
	}

	public void DrawCard()
	{
		for (int i = 0; i < knMaxCardsInHand; i++)
		{
			if (mCards[i].Enabled == false)
			{
				DrawCardTo(i);
				return;
			}
		}

		Debug.LogWarning("UNABLED TO DRAW CARD: No empty slots.");
	}

	// Card pos should be 0 - 4
	private void DrawCardTo(int _cardSlotID)
	{
		// Replace card sprite with card back.
		mCards[_cardSlotID].ToggleCard(true);
		mCards[_cardSlotID].SetCardDraggable(false);
		mCards[_cardSlotID].CardImage.sprite = CardBackSprite;

		// Move the card to slot
		mCardTransforms[_cardSlotID].position = svec3DrawPos;
		mCardTransforms[_cardSlotID].SetAsLastSibling();
		MoveToAction moveToSlot = new MoveToAction(mCardTransforms[_cardSlotID], Graph.InverseExponential,  mCards[_cardSlotID].OriginPos, 0.6f);
		moveToSlot.OnActionFinish += () => {
			mCardTransforms[_cardSlotID].SetSiblingIndex(mCards[_cardSlotID].OriginSiblingIndex);
		};

		// Flip the Card
		// 0 to 90, snap -90, -90 to 0
		mCardTransforms[_cardSlotID].localEulerAngles = Vector3.zero;
		RotateToAction rotHide = new RotateToAction(mCardTransforms[_cardSlotID], Graph.Linear, Vector3.up * 90.0f, 0.5f);
		rotHide.OnActionFinish += () => {
			mCardTransforms[_cardSlotID].localEulerAngles = Vector3.up * 270.0f;
			mCards[_cardSlotID].NewCard();
		};
		RotateToAction rotReveal = new RotateToAction(mCardTransforms[_cardSlotID], Graph.Linear, Vector3.up * 360.0f, 0.5f);
		ActionSequence cardFlipSeq = new ActionSequence(rotHide, rotReveal);

		// Combine all actions here.
		ActionSequence drawCardSeq = new ActionSequence(moveToSlot, cardFlipSeq);
		drawCardSeq.OnActionFinish += () => {
			mCards[_cardSlotID].SetCardDraggable(true);
		};

		ActionHandler.RunAction(drawCardSeq);
	}

	public void ReorganiseCards()
	{
		// Shifting cards around animation.
		for (int iEmpty = 0; iEmpty < knMaxCardsInHand - 1; iEmpty++)
		{
			// If there is an empty slot.
			if (mCards[iEmpty].Enabled == false)
			{
				// Check for any cards after this empty slot.
				for (int iCard = iEmpty + 1; iCard < knMaxCardsInHand; iCard++)
				{
					if (mCards[iCard].Enabled == true)
					{
						ShiftCard(iCard, iEmpty);
						ReorganiseCards();
						break;
					}
				}
				// If no more cards after empty slot, no need to check anymore.
				return;
			}
		}
	}

	public void ReturnExecutedCard()
	{
		ControlAreaManager.ExecutedCard.ToggleCard(true);
		ControlAreaManager.ExecutedCard.SetCardDraggable(false);
		ControlAreaManager.ExecutedCard.transform.SetAsLastSibling();

		// Animate toCard move from fromCard to toCard.
		ControlAreaManager.ExecutedCard.transform.position = svec3UseCardPos;
		MoveToAction moveCard = new MoveToAction(
			ControlAreaManager.ExecutedCard.transform,
			Graph.InverseExponential,
			ControlAreaManager.ExecutedCard.OriginPos,
			0.4f);
		moveCard.OnActionFinish += () => {
			ControlAreaManager.ExecutedCard.SetCardDraggable(true);
			ControlAreaManager.ExecutedCard.transform.SetSiblingIndex(ControlAreaManager.ExecutedCard.OriginSiblingIndex);
		};
		ActionHandler.RunAction(moveCard);
	}

	private void ShiftCard(int _fromSlotID, int _toSlotID)
	{
		mCards[_toSlotID].CopyCardData(mCards[_fromSlotID]);
		mCards[_fromSlotID].ToggleCard(false);
		mCards[_toSlotID].ToggleCard(true);
		mCards[_toSlotID].SetCardDraggable(false);

		// Animate toCard move from fromCard to toCard.
		mCardTransforms[_toSlotID].position = mCards[_fromSlotID].OriginPos;
		MoveToAction moveCard = new MoveToAction(mCardTransforms[_toSlotID], Graph.InverseExponential, mCards[_toSlotID].OriginPos, 0.6f);
		moveCard.OnActionFinish += () => {
			mCards[_toSlotID].SetCardDraggable(true);
		};
		ActionHandler.RunAction(moveCard);
	}
}
