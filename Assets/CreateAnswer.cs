using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class CreateAnswer : MonoBehaviour {

	[SerializeField] public Button OK;
	[SerializeField] public Button Cancel;
	[SerializeField] public InputField AnswerField;
	[SerializeField] public GameObject PopupWindow;
	private PopUp popup;

	public string questId = "";
	void Start () {
		OK.onClick.AddListener(() => SendAnswer(questId));
		Cancel.onClick.AddListener(() => gameObject.SetActive(false));

		popup = PopupWindow.GetComponent<PopUp>();
	}

	void SendAnswer(string questId) {
		var answer = AnswerField.text;
		if (answer == "") return;
		var token = PlayerPrefs.GetString("token", "");
		RestClient.createAnswer(token, answer, questId)
			.Subscribe(
				x => parseResponce(x.text),
				e => parseError(e)	
			);
	}

	void parseResponce(string json) {
		Debug.Log(json);
	}

	void OnDisable() {
		AnswerField.text = "";
	}

	void parseError(Exception e) {
		string message;
		try {
			message = new JSONObject(e.ToString())["message"].str;
		} catch (Exception) {
			message = e.ToString();
		}
		showPopup("Error", message);
	}

	private void showPopup(string title, string message) {
		popup.Title = title;
		popup.Message = message;
		PopupWindow.SetActive(true);
	}
}
