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

	void OnCancelClick()
	{
		gameObject.SetActive(false);
	}

    void OnVerifyClick() 
    {
		GameObject.Find("Loading").GetComponent<LoadingController>().showLoading();
		RestClient.verifyEmail (getCreds (), CodeField.text)
			.Subscribe(
				x => { parseSuccess(x); },
				e => { parseError(e); }
			);
    }

	private void parseSuccess(WWW response) {
		try {
			string token = new JSONObject(response.text)["accessToken"].str;
			PlayerPrefs.SetString ("token", token);
			RestClient.getProfile(token)
				.Subscribe(
					x => parseProfile(x),
					e => showValidationError(e.ToString())
				); 
		} catch (Exception e) {
			GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
			showValidationError(e.ToString());
			Debug.Log(e);
		}
		
	}

	private void parseProfile(string profileJson) {
		ProfileRepository.Instance.SaveProfileJson(profileJson);
		SceneManager.LoadSceneAsync ("CachedDynamicLoader");
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
		GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
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
		GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
		popup.Title = "Verification";
		popup.Message = message;
		PopUpWindow.gameObject.SetActive(true);
	}
}