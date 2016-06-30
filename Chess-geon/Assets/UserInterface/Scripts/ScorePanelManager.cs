using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;
using DaburuTools;

public class ScorePanelManager : MonoBehaviour
{
	private static ScorePanelManager sInstance = null;
	public static ScorePanelManager Instance { get { return sInstance; } }

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



	private Text mScoreText;
	private PulseAction mPulse;

	private void Setup()
	{
		mScoreText = transform.FindChild("ScoreText").GetComponent<Text>();

		mPulse = null;
		mScoreText.text = GameManager.Instance.Score.ToString("N0");
	}

	#region Client Functions
	public void UpdateScore(int _score)
	{
		mScoreText.text = _score.ToString("N0");

		// TODO: Pulse attention!
		if (mPulse == null)
		{
			mPulse = new PulseAction(
				mScoreText.transform,
				1,
				Graph.Linear,
				Graph.InverseExponential,
				0.08f,
				0.15f,
				Vector3.one,
				Vector3.one * 1.5f);
		}
		mPulse.StopAction(true);
		ActionHandler.RunAction(mPulse);
	}
	#endregion
}
