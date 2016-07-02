using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WaitForSplashToFinish : MonoBehaviour
{	
	void Update()
	{
		if (Application.isShowingSplashScreen == false)
		{
			SceneManager.LoadScene("TitleScene");
		}
	}
}
