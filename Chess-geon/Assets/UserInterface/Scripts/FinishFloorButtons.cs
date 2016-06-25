using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishFloorButtons : MonoBehaviour
{
	public void RestartButton()
	{
		SceneManager.LoadScene("DungeonLevelScene");
	}
}
