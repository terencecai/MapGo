using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;


public class VerifyManager : MonoBehaviour {
    
    [SerializeField] public Button CancelButton;
    [SerializeField] public Button VerifyButton;
    [SerializeField] public Button RequestCode;
    [SerializeField] public InputField CodeField;
	[SerializeField] public Canvas PopUpWindow;

	private PopUp popup;


	void Start () {
		CancelButton.onClick.AddListener(OnCancelClick);
        VerifyButton.onClick.AddListener(OnVerifyClick);
        RequestCode.onClick.AddListener(OnRequestClick);

		popup = PopUpWindow.GetComponent<PopUp> ();
	}

	void Update () {
	}

	void OnCancelClick()
	{
		gameObject.SetActive(false);
	}

    void OnVerifyClick() 
    {
		Credentials creds = getCreds ();
		RestClient.verifyEmail (creds, CodeField.text)
			.Subscribe(
				x => SceneManager.LoadScene("CachedDynamicLoader"),
				e => { parseError(e); }
			);
    }

    void OnRequestClick() 
    {
		Credentials creds = getCreds ();

		RestClient.requestCode (creds)
			.Subscribe (
				x => { showCodeMessage("Verification code have been sent to your email"); },
				e => { parseError(e); }
			);
    }
				
	private Credentials getCreds() {
		return new Credentials (
			PlayerPrefs.GetString("email", ""),
			PlayerPrefs.GetString("password", "")
		);
	}
	private void parseError(Exception e) {
		if (e is UniRx.WWWErrorException) {
			var err = new JSONObject((e as UniRx.WWWErrorException).Text);
			showValidationError(err.str);
		} else {
			showValidationError(e.ToString());
		}
	}

	private void showValidationError(string message)
	{
		popup.Title = "Error!";
		popup.Message = message;
		PopUpWindow.gameObject.SetActive(true);
	}

	private void showCodeMessage(string message)
	{
		popup.Title = "Verification";
		popup.Message = message;
		PopUpWindow.gameObject.SetActive(true);
	}
}