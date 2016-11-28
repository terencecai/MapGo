using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;


public class RestoreManager : MonoBehaviour {

	[SerializeField] public Button CancelButton;
	[SerializeField] public Button SendButton;
	[SerializeField] public InputField CodeField;
	[SerializeField] public Canvas PopUpWindow;

	private PopUp popup;


	void Start () {
		CancelButton.onClick.AddListener(OnCancelClick);
		SendButton.onClick.AddListener(OnVerifyClick);

		popup = PopUpWindow.GetComponent<PopUp> ();
	}

	void Update () {
	}

	void OnEnable() {
		CodeField.text = "";
	}

	void OnCancelClick()
	{
		gameObject.SetActive(false);
	}

	void OnVerifyClick() 
	{
		RestClient.requestPassword (CodeField.text)
			.Subscribe(
				x => { showCodeMessage("New password have been sent to your email"); },
				e => { parseError(e); }
			);
	}

	private void parseError(Exception e) {
		var err = new JSONObject((e as UniRx.WWWErrorException).Text);
		try {
			showValidationError(err["message"].str);
		} catch (Exception ee) {
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
		popup.Title = "New password";
		popup.Message = message;
		PopUpWindow.gameObject.SetActive(true);
	}
}