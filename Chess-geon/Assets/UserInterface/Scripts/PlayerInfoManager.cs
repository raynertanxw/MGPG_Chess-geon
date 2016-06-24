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

	private void Setup()
	{
		Transform playerHealth = transform.FindChild("Player Health");
		HealthHearts = new Image[playerHealth.childCount];
		for (int i = 0; i < playerHealth.childCount; i++)
		{
			HealthHearts[i] = playerHealth.GetChild(i).GetComponent<Image>();
			HealthHearts[i].sprite = FullHeart;
		}
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
}
