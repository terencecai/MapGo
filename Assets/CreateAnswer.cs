using UnityEngine;
using UnityEngine.UI;

using System;

public class CreateAnswer : MonoBehaviour {

	[SerializeField] public Button OK;
	[SerializeField] public Button Cancel;
	[SerializeField] public InputField AnswerField;

	public string questId = "";
	void Start () {
		OK.onClick.AddListener(() => SendAnswer(questId));
		Cancel.onClick.AddListener(() => gameObject.SetActive(false));
	}

	void SendAnswer(string questId) {

	}

	void parseResponce(string json) {

	}

	void parseError(Exception e) {

	}
}
