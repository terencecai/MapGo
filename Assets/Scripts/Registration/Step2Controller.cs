using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
public class Step2Controller : MonoBehaviour {

	[SerializeField] public InputField emailField;
	[SerializeField] public InputField passwordField;
	[SerializeField] public Button submitButton;
	[SerializeField] public Canvas popupWindow;

	[SerializeField] public Canvas VerifyDialog;

	private Validator validator;
	private PopUp popup;

	Credentials creds;

	void Start () {
		setListeners();
		validator = new Validator(showValidationError);
		popup = popupWindow.GetComponent<PopUp>();
		setValidation();
	}

	private void setListeners()
	{
		submitButton.onClick.AddListener(onSubmitClick);
	}

	private void setValidation() {
		validator.AddValidator(() => { return emailField.text != ""; }, "Please, enter valid email");
		validator.AddValidator(() => { return passwordField.text != ""; }, "Please, enter password");
	}

	private void onSubmitClick()
	{
		if (!validator.PerformValidate()) { return; }

		Profile user = loadProfileFromStep();
		creds = new Credentials(
			emailField.text, passwordField.text
		);

		PlayerPrefs.SetString ("email", emailField.text);
		PlayerPrefs.SetString ("password", passwordField.text);

		user.credentials = creds;

		GameObject.Find("Loading").GetComponent<LoadingController>().showLoading();

		RestClient.register(user).Subscribe(
			resp => parseSuccess(resp),
			error => parseError(error)
		);
	}

	private Profile loadProfileFromStep()
	{
		Profile user = new Profile();
		user.nickName = PlayerPrefs.GetString("profile_name", "");
		user.birthDay = PlayerPrefs.GetString("profile_birthday", "");
		user.gender = PlayerPrefs.GetString("profile_gender", "");
		return user;
	}

	private void showValidationError(string message) {
		popup.Title = "Error";
		popup.Message = message;
		popupWindow.gameObject.SetActive(true);
	}

	private void parseSuccess(WWW response) {
		GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
		PlayerPrefs.SetString ("email", creds.GetEmail ());
		PlayerPrefs.SetString ("password", creds.GetPassword ());
		showVerifyDialog();
	}

	private void parseError(Exception e) {
		GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
		try {
			if (e is UniRx.WWWErrorException) {
				var err = new JSONObject((e as UniRx.WWWErrorException).Text);
				showValidationError(err["message"].str);
			} else {
				showValidationError(e.ToString());
			}
		} catch (Exception ee) {
			showValidationError(e.ToString());
		}
	}

	private void showVerifyDialog() {
		VerifyDialog.gameObject.SetActive(true);
	}
}
