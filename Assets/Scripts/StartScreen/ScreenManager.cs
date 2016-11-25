using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {

	[SerializeField] public GameObject[] Screens;

	private int currentScreenPosition = 0;

	public void ShowNextScreen(int screen) {
		string outAnimIdentifier = "OutLeft";
		if (screen == 3) outAnimIdentifier = "OutRight";

		Screens[currentScreenPosition].GetComponent<Animator>().Play(outAnimIdentifier);
		Screens[screen].SetActive(true);
		Screens[screen].GetComponent<Animator>().Play("InRight");

		currentScreenPosition = screen;
	}

	public void ShowPrevScreen() {
		int prev = 0;
		if (currentScreenPosition != 2) {
			prev = currentScreenPosition - 1;	
		}
		if (prev < 0) { return; }

		Screens[prev].SetActive(true);
		Screens[prev].GetComponent<Animator>().Play("InLeft");
		Screens[currentScreenPosition].GetComponent<Animator>().Play("OutRight");

		currentScreenPosition = prev;
	}
}
