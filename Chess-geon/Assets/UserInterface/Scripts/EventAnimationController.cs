using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;
using DaburuTools;

public class EventAnimationController : MonoBehaviour
{
	private static EventAnimationController sInstance = null;
	public static EventAnimationController Instance { get { return sInstance; } }

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

	void Start()
	{
		ExecutePhaseAnimation(GamePhase.PlayerPhase);
	}

	private ControlAreaButtons mCtrlArea;

	private Image mPhaseTop, mPhaseBottom;
	private Image mBGOverlayImage;
	public Sprite PlayerPhaseTopSprite, PlayerPhaseBottomSprite, EnemyPhaseTopSprite, EnemyPhaseBottomSprite;
	private Graph InverseSmoothStep;

	private bool mbIsAnimating = false;
	public bool IsAnimating { get { return mbIsAnimating; } }

	private void Setup()
	{
		mCtrlArea = GameObject.Find("ControlAreaCanvas").GetComponent<ControlAreaButtons>();

		mPhaseTop = transform.FindChild("Phase_Top").GetComponent<Image>();
		mPhaseBottom = transform.FindChild("Phase_Bottom").GetComponent<Image>();

		mBGOverlayImage = transform.FindChild("BGOverlayImage").GetComponent<Image>();
		mBGOverlayImage.enabled = false;

		InverseSmoothStep = new Graph((float _x) => {
			return Mathf.Lerp(
				1f - ((1f - _x) * (1f - _x)),
				_x * _x,
				_x);
		});
	}

	public void ExecutePhaseAnimation(GamePhase _phase)
	{
		mbIsAnimating = true;
		mCtrlArea.SetControlBlockerEnabled(true);

		Color overlayCol = mBGOverlayImage.color;
		overlayCol.a = 0.0f;
		mBGOverlayImage.color = overlayCol;
		mBGOverlayImage.enabled = true;
		switch (_phase)
		{
		case GamePhase.PlayerPhase:
			mPhaseTop.sprite = PlayerPhaseTopSprite;
			mPhaseBottom.sprite = PlayerPhaseBottomSprite;
			break;
		case GamePhase.EnemyPhase:
			mPhaseTop.sprite = EnemyPhaseTopSprite;
			mPhaseBottom.sprite = EnemyPhaseBottomSprite;
			break;
		}

		mPhaseTop.rectTransform.localEulerAngles = Vector3.forward * 90.0f;
		mPhaseBottom.rectTransform.localEulerAngles = Vector3.forward * 90.0f;

		RotateByAction2D topRotIn = new RotateByAction2D(mPhaseTop.transform, Graph.InverseExponential, -86.0f, 0.6f);
		RotateByAction2D bottomRotIn = new RotateByAction2D(mPhaseBottom.transform, Graph.InverseExponential, -86.0f, 0.6f);
		ActionParallel rotateIn = new ActionParallel(topRotIn, bottomRotIn);

		DelayAction rotInOutDelay = new DelayAction(0.6f);

		RotateByAction2D topRotOut = new RotateByAction2D(mPhaseTop.transform, Graph.InverseExponential, -86.0f, 0.6f);
		RotateByAction2D bottomRotOut = new RotateByAction2D(mPhaseBottom.transform, Graph.InverseExponential, -86.0f, 0.6f);
		ActionParallel rotateOut = new ActionParallel(topRotOut, bottomRotOut);
		
		ActionSequence rotInOutSeq = new ActionSequence(rotateIn, rotInOutDelay, rotateOut);
		rotInOutSeq.OnActionFinish += () => {
			mBGOverlayImage.enabled = false;
			mCtrlArea.SetControlBlockerEnabled(false);
			mbIsAnimating = false;
		};



		DelayAction stallFrontDelay = new DelayAction(0.5f);
		DelayAction stallEndDelay = new DelayAction(0.5f);

		RotateByAction2D topRotStall = new RotateByAction2D(mPhaseTop.transform, InverseSmoothStep, -8.0f, 0.8f);
		RotateByAction2D bottomRotStall = new RotateByAction2D(mPhaseBottom.transform, InverseSmoothStep, -8.0f, 0.8f);
		ActionParallel rotateStall = new ActionParallel(topRotStall, bottomRotStall);

		ActionSequence rotStallSeq = new ActionSequence(stallFrontDelay, rotateStall, stallEndDelay);


		DelayAction alphaDelay = new DelayAction(0.8f);
		ImageAlphaFadeToAction alphaIn = new ImageAlphaFadeToAction(mBGOverlayImage, Graph.Linear, 0.5f, 0.5f);
		ImageAlphaFadeToAction alphaOut = new ImageAlphaFadeToAction(mBGOverlayImage, Graph.Linear, 0.0f, 0.5f);
		ActionSequence alphaFadeSeq = new ActionSequence(alphaIn, alphaDelay, alphaOut);

		ActionHandler.RunAction(rotInOutSeq, rotStallSeq, alphaFadeSeq);
	}
}
