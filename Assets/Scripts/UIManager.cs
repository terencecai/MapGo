using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Text gps;

	[SerializeField]
	private Image background;

	private bool enabled = false;

    public void enableWarning(string values) {
		if (enabled)
			return;

		enabled = true;
		background.gameObject.SetActive (true);
		gps.gameObject.SetActive (true);
        gps.text = values;
    }

	public void disableWarning() {
		enabled = false;
		background.gameObject.SetActive (false);
		gps.gameObject.SetActive (false);
	}
}
