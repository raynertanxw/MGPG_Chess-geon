using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;
using DaburuTools;
using UnityEngine.SceneManagement;

public class LandingSceneManager : MonoBehaviour
{
	private CanvasGroup mSplashPanelCG;
	private Transform mLogoTransform;
	private Transform mSpiningCard;
	private SpriteRenderer mCardSpriteRen;
	private Text mTapToStartText, mHighscoreText;
	private Button mStartButton;

	private CanvasGroup mAboutPanel;

	private Sprite mSpriteCardBack;
	private Sprite[] mCardSprites;

	void Awake()
	{
		mSpriteCardBack = Resources.Load<Sprite>("Sprites/Card Back");
		mCardSprites = Resources.LoadAll<Sprite>("Sprites/chessgeon_cards");

		mSplashPanelCG = transform.FindChild("Splash Panel").GetComponent<CanvasGroup>();
		mSplashPanelCG.alpha = 1.0f;
		mLogoTransform = mSplashPanelCG.transform.FindChild("Daburu Logo");
//		mSpiningCard = transform.FindChild("Spining Card");
		mSpiningCard = GameObject.Find("Spining Card").transform;
		mCardSpriteRen = mSpiningCard.FindChild("Card Sprite").GetComponent<SpriteRenderer>();
		mCardSpriteRen.sprite = mSpriteCardBack;
		mTapToStartText = transform.FindChild("Tap To Start Text").GetComponent<Text>();
		mTapToStartText.enabled = false;
		mHighscoreText = transform.FindChild("Highscore Text").GetComponent<Text>();
		if (PlayerPrefs.HasKey(Constants.kStrHighscore))
		{
			mHighscoreText.text = "Highscore:\n" + PlayerPrefs.GetInt(Constants.kStrHighscore).ToString("N0");
			mHighscoreText.enabled = true;
		}
		else
		{
			mHighscoreText.text = "Highscore:\nNone... YET!";
			mHighscoreText.enabled = true;
		}
		mStartButton = transform.FindChild("Start Button").GetComponent<Button>();
		mStartButton.interactable = false;

		mAboutPanel = transform.FindChild("About Panel").GetComponent<CanvasGroup>();
		CloseAboutPanel();

		DelayAction prePulseDelay = new DelayAction(1.5f);
		PulseAction clickPulse = new PulseAction(
			mLogoTransform,
			1,
			Graph.Exponential,
			0.25f,
			Vector3.one,
			Vector3.one * 0.8f);
		clickPulse.OnActionStart += () => { AudioManager.PlayPenClickSound(); };
		DelayAction postPulseDelay = new DelayAction(0.25f);

		MoveByAction anticipateLeft = new MoveByAction(mLogoTransform, Graph.InverseExponential, Vector3.left * (100) * (Screen.height / 1920.0f), 0.5f);
		ScaleToAction anticipateSquash = new ScaleToAction(mLogoTransform, Graph.InverseExponential, new Vector3(0.75f, 1.25f, 1.25f), 0.5f);
		ActionParallel anticipateParallel = new ActionParallel(anticipateLeft, anticipateSquash);
		DelayAction postAnticipateDelay = new DelayAction(0.1f);
		MoveByAction zoomRight = new MoveByAction(mLogoTransform, Graph.InverseExponential, Vector3.right * (1024 + 400) * (Screen.height / 1920.0f), 0.3f);
		ScaleToAction scaleDown = new ScaleToAction(mLogoTransform, Graph.InverseExponential, new Vector3(1.5f, 0.5f, 0.5f), 0.5f);
		ActionParallel zoomRightParallel = new ActionParallel(zoomRight, scaleDown);

		DelayAction preFadeDelay = new DelayAction(0.2f);
		CanvasGroupAlphaFadeToAction fadeCGAway = new CanvasGroupAlphaFadeToAction(mSplashPanelCG, 0.0f, 1.5f);
		fadeCGAway.OnActionFinish += () => {
			mSplashPanelCG.blocksRaycasts = false;
		};

		DelayAction TurnOn = new DelayAction(0.5f);
		TurnOn.OnActionFinish += () => {
			mTapToStartText.enabled = true;
			if (mStartButton.interactable == false)
				mStartButton.interactable = true;
		};
		DelayAction TurnOff = new DelayAction(1.0f);
		TurnOff.OnActionFinish += () => { mTapToStartText.enabled = false; };
		ActionSequence tapTextFlashSeq = new ActionSequence(TurnOn, TurnOff);
		ActionRepeatForever repeatFlash = new ActionRepeatForever(tapTextFlashSeq);

		ActionSequence splashSeq = new ActionSequence(
			prePulseDelay, clickPulse, postPulseDelay,
			anticipateParallel, postAnticipateDelay, zoomRightParallel,
			preFadeDelay, fadeCGAway,
			repeatFlash
		);
		ActionHandler.RunAction(splashSeq);



		// Card is already spinning in the background.
		// 0 to 90, snap -90, -90 to 0
		mSpiningCard.localEulerAngles = Vector3.zero;
		// Timing here doesn't matter. Is used to sync. 0.5f just nice to have card back show when canvas fades.
		RotateByAction initSpin = new RotateByAction(mSpiningCard, Graph.Linear, Vector3.up * 90.0f, 0.5f);
		initSpin.OnActionFinish += () => {
			mCardSpriteRen.transform.localScale -= Vector3.right * 2.0f;
			mCardSpriteRen.sprite = mCardSprites[Random.Range(0, mCardSprites.Length)];
		};
		RotateByAction spinA = new RotateByAction(mSpiningCard, Graph.Linear, Vector3.up * 180.0f, 3.0f);
		spinA.OnActionFinish += () => {
			mCardSpriteRen.transform.localScale += Vector3.right * 2.0f;
			mCardSpriteRen.sprite = mSpriteCardBack;
		};
		RotateByAction spinB = new RotateByAction(mSpiningCard, Graph.Linear, Vector3.up * 180.0f, 3.0f);
		spinB.OnActionFinish += () => {
			mCardSpriteRen.transform.localScale -= Vector3.right * 2.0f;
			mCardSpriteRen.sprite = mCardSprites[Random.Range(0, mCardSprites.Length)];
		};

		ActionSequence cardSpinSeq = new ActionSequence(spinA, spinB);
		ActionHandler.RunAction(new ActionSequence(initSpin, new ActionRepeatForever(cardSpinSeq)));
	}

	#region Buttons
	public void StartGame()
	{
		mStartButton.enabled = false;
		CanvasGroupAlphaFadeToAction fadeInNStart = new CanvasGroupAlphaFadeToAction(
			mSplashPanelCG,
			1.0f,
			1.0f);
		fadeInNStart.OnActionFinish += () => {
			SceneManager.LoadScene("DungeonLevelScene");
		};
		ActionHandler.RunAction(fadeInNStart);

		AudioManager.PlayButtonClickSound();
	}

	public void PresentAboutPanel()
	{
		mAboutPanel.alpha = 1.0f;
		mAboutPanel.interactable = true;
		mAboutPanel.blocksRaycasts = true;
	}

	public void CloseAboutPanel()
	{
		mAboutPanel.alpha = 0.0f;
		mAboutPanel.interactable = false;
		mAboutPanel.blocksRaycasts = false;
	}
	#endregion
}
