using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UniRx;

public class LoginController : MonoBehaviour {

	[SerializeField] public InputField EmailField;
	[SerializeField] public InputField PasswordField;
	[SerializeField] public Button SubmitButton;
	[SerializeField] public Canvas PopupWindow;
	[SerializeField] public Canvas VerifyDialog;
	[SerializeField] public Button ForgetButton;

	[SerializeField] public Canvas EmailDialog;

	private Validator validator;
	private PopUp popUp;

	void Start() {
		popUp = PopupWindow.GetComponent<PopUp>();
		validator = new Validator(showValidationError);
		setValidation();
		SubmitButton.onClick.AddListener(login);
		ForgetButton.onClick.AddListener (forget);
	}

	void OnEnable() {
		EmailField.text = "";
		PasswordField.text = "";
	}

	private void setValidation() {
		validator.AddValidator (() => { return EmailField.text.Contains("@"); }, "Enter valid email.");
		validator.AddValidator (() => { return PasswordField.text != ""; }, "Enter password");
		validator.AddValidator (() => { return PasswordField.text.Length >= 6; }, "Password must be at least 6 characters");
	}

	private void showValidationError(string message)
	{
		popUp.Title = "Error!";
		popUp.Message = message;
		PopupWindow.gameObject.SetActive(true);
	}

	private void login() {
		if (!validator.PerformValidate()) return;

		GameObject.Find("Loading").GetComponent<LoadingController>().showLoading();
		Credentials creds = new Credentials(EmailField.text, PasswordField.text);
		PlayerPrefs.SetString ("email", EmailField.text);
		PlayerPrefs.SetString ("password", PasswordField.text);

		RestClient.login(creds)
			.Subscribe(
				x => parseSuccess(x),
				e => parseError(e)
			);
	}

	private void parseSuccess(WWW response) {
		try {
			string token = new JSONObject (response.text)["accessToken"].str;
			PlayerPrefs.SetString ("token", token);
			loadProfile(token);
		} catch (Exception e) {
			Debug.Log(response.text);
			showValidationError (e.ToString ());
		}

	}

	private void loadProfile(string token)
    {
        Observable.WhenAll(
            RestClient.getProfile(token),
            RestClient.getMySkills(token),
            RestClient.getAllSkills(token))
            .Subscribe(
                x => loadGame(x),
                e => { Debug.Log(e); parseError(e); }
            );
    }

    private void loadGame(string[] prof)
    {
        var profile = JsonUtility.FromJson<Profile>(prof[0]);
        profile.skills = ApplicationLoadController.convertSkills(prof[1]);
        profile.allSkills = ApplicationLoadController.convertSkills(prof[2]);
        ProfileRepository.Instance.SaveProfileJson(profile);
        SceneManager.LoadSceneAsync("CachedDynamicLoader");
    }

	private void parseProfile(string json) {
		ProfileRepository.Instance.SaveProfileJson(json);
		SceneManager.LoadSceneAsync ("CachedDynamicLoader");
	}

	private void parseError(Exception e) {
		GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
		if (!(e is WWWErrorException)) {
			showValidationError (e.ToString ());
			return;
		}

		var err = new JSONObject((e as UniRx.WWWErrorException).Text);
		try {
			string code = err ["code"].str;
			if (code != null && code.Equals ("401")) {
				VerifyDialog.gameObject.SetActive (true);
			} else {
				showValidationError (err["message"].str);
			}
		} catch (Exception ee) {
			showValidationError (e.ToString ());
		}
	}

	private void forget() {
		EmailDialog.gameObject.SetActive (true);
	}
}
