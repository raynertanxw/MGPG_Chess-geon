using UnityEngine;
using System.Collections;

public class ControlAreaButtons : MonoBehaviour
{
	public void EndTurnButton()
	{
		GameManager.Instance.EndPlayerPhase();
	}
}
