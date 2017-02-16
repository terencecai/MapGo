using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Text gps;

	[SerializeField]
	private Image background;

	[SerializeField]
	public GameObject Joystick;

	private bool enabled = false;

    public void enableWarning(string values) {
		if (enabled) {
			gps.text = values;
			return;
		}

		enabled = true;
		background.gameObject.SetActive (true);
		gps.gameObject.SetActive (true);
        gps.text = values;
    }

	public void disableWarning() {
		if (!enabled) return;
		enabled = false;
		background.gameObject.SetActive (false);
		gps.gameObject.SetActive (false);
	}
}
