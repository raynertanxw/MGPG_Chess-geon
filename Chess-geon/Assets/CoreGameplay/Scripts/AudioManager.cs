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





	private void Setup()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}
