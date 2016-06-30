using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
	private static PlayerInfoManager sInstance = null;
	public static PlayerInfoManager Instance { get { return sInstance; } }

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


	public Sprite FullHeart, EmptyHeart;
	private Image[] HealthHearts;

	public Sprite FullShield, EmptyShield;
	private Image ShieldImage, ShieldTextBG;
	private Text ShieldText;

	private Text CoinText;

	private CanvasGroup mRepeatCG;
	private Text mNumRepeats;

	private Text mFloorText;

	private void Setup()
	{
		Transform playerHealth = transform.FindChild("Player Health");
		HealthHearts = new Image[playerHealth.childCount];
		for (int i = 0; i < playerHealth.childCount; i++)
		{
			HealthHearts[i] = playerHealth.GetChild(i).GetComponent<Image>();
			HealthHearts[i].sprite = FullHeart;
		}

		Transform playerShield = transform.FindChild("Player Shield");
		ShieldImage = playerShield.FindChild("Shield Image").GetComponent<Image>();
		ShieldTextBG = playerShield.FindChild("text BG mask").GetChild(0).GetComponent<Image>();
		ShieldText = playerShield.FindChild("Shield Text").GetComponent<Text>();

		Transform playerCoins = transform.FindChild("Player Coins");
		CoinText = playerCoins.FindChild("Coin Text").GetComponent<Text>();

		Transform repeatCount = transform.FindChild("Player Repeat");
		mRepeatCG = repeatCount.GetComponent<CanvasGroup>();
		mRepeatCG.alpha = 0.0f;
		mNumRepeats = repeatCount.FindChild("NumRepeats Text").GetComponent<Text>();

		Transform floorText = transform.FindChild("Player Floor");
		mFloorText = floorText.FindChild("Floor Text").GetComponent<Text>();
		PlayerInfoManager.Instance.UpdateFloor(GameManager.Instance.FloorNumber);
	}

	void Start()
	{
		UpdateHealth(GameManager.Instance.Player.Health);
		UpdateShield(GameManager.Instance.Player.Shield);
		UpdateCoins(GameManager.Instance.Player.Coins);
	}

	public void UpdateHealth(int _health)
	{
		for (int i = 0; i < HealthHearts.Length; i++)
		{
			if (i < _health)
				HealthHearts[i].sprite = FullHeart;
			else
				HealthHearts[i].sprite = EmptyHeart;
		}
	}

	public void UpdateShield(int _shield)
	{
		if (_shield <= 0)
		{
			ShieldTextBG.enabled = false;
			ShieldText.enabled = false;

			ShieldImage.sprite = EmptyShield;
		}
		else
		{
			ShieldTextBG.enabled = true;
			ShieldText.enabled = true;

			ShieldImage.sprite = FullShield;
			ShieldText.text = _shield.ToString();
		}
	}

	public void UpdateCoins(int _coins)
	{
		CoinText.text = "x" + _coins.ToString("000");
	}

	public void UpdateRepeat(int _numRepeat)
	{
		// Show
		if (_numRepeat > 0)
		{
			mNumRepeats.text = "x" + _numRepeat.ToString();
			mRepeatCG.alpha = 1.0f;
		}
		// Hide
		else
		{
			mRepeatCG.alpha = 0.0f;
		}
	}

	public void UpdateFloor(int _floorNum)
	{
		mFloorText.text = "Floor:" + _floorNum.ToString();
	}
}
