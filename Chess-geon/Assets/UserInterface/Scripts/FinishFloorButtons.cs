using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishFloorButtons : MonoBehaviour
{
	public void RestartButton()
	{
		SceneManager.LoadScene("DungeonLevelScene");
	}

	public void NextFloor()
	{
		// TODO: Store player data for next floor.

		SceneManager.LoadScene("DungeonLevelScene");
	}
}
