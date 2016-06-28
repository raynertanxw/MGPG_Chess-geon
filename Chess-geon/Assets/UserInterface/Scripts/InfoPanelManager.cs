using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
	private static InfoPanelManager sInstance = null;
	public static InfoPanelManager Instance { get { return sInstance; } }

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


	private CanvasGroup mCG;
	private Text mInfoText;

	private void Setup()
	{
		mCG = gameObject.GetComponent<CanvasGroup>();
		mInfoText = transform.FindChild("Info Text").GetComponent<Text>();

		SetInfoPanelVisible(false);
	}

	private void SetInfoPanelVisible(bool _visible)
	{
		if (_visible)
		{
			mCG.alpha = 1.0f;
			mCG.blocksRaycasts = true;
			mCG.interactable = true;
		}
		else
		{
			mCG.alpha = 0.0f;
			mCG.blocksRaycasts = false;
			mCG.interactable = false;
		}
	}

	public void DisplayInfoPanel(string _infoText)
	{
		mInfoText.text = _infoText;
		SetInfoPanelVisible(true);
	}

	#region Button Functions
	public void Dismiss()
	{
		SetInfoPanelVisible(false);
	}
	#endregion
}
