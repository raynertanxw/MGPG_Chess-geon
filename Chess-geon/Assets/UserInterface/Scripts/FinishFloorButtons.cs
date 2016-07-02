using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishFloorButtons : MonoBehaviour
{
	public void RestartButton()
	{
		AudioManager.PlayButtonClickSound();

		SceneManager.LoadScene("DungeonLevelScene");
	}

	public void NextFloor()
	{
		AudioManager.PlayButtonClickSound();

		SceneManager.LoadScene("DungeonLevelScene");
	}

	public void Exit()
	{
		AudioManager.PlayButtonClickSound();

		SceneManager.LoadScene("TitleScene");
	}
}
