using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
	public GameObject ConfirmationPanel;

	private void Start()
	{
		ConfirmationPanel.SetActive(false);
	}
	

	public void OpenConfirmation()
	{
		ConfirmationPanel.SetActive(true);
	}

	public void CloseConfirmation()
	{
		ConfirmationPanel.SetActive(false);
	}
	
	public void TrueQuit()
	{
		Debug.Log("Game quits");
		Application.Quit();
	}
}
