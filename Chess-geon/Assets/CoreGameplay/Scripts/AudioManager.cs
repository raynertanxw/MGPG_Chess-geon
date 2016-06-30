using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	private static AudioManager sInstance = null;
	public static AudioManager Instance { get { return sInstance; } }

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



	private AudioSource[] mSFX;

	private void Setup()
	{
		DontDestroyOnLoad(this.gameObject);

		mSFX = new AudioSource[transform.childCount];
		for (int i = 0; i < mSFX.Length; i++)
		{
			mSFX[i] = transform.GetChild(i).GetComponent<AudioSource>();
		}
	}

	public static void PlayButtonClickSound()
	{
		sInstance.mSFX[0].Play();
	}

	public static void PlayCoinGetSound()
	{
		sInstance.mSFX[1].Play();
	}

	public static void PlayCoinSpendSound()
	{
		sInstance.mSFX[2].Play();
	}

	public static void PlayCardDrawSound()
	{
		sInstance.mSFX[3].Play();
	}
}
