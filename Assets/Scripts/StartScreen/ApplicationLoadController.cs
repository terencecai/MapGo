using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ApplicationLoadController : MonoBehaviour {
	private ScreenManager manager;

	void Start () {
		manager = GetComponent<ScreenManager>();
		checkExisting();

	}

	public void OnClick(string scene)
	{
		manager.ShowNextScreen(Convert.ToInt16(scene));
	}

	public void OnBackClick() {
		manager.ShowPrevScreen();
	}


	//Initial configuration
	private void checkExisting()
	{
	}
}
