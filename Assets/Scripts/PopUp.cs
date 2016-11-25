using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUp : MonoBehaviour {

	public string Title = "";
	public string Message = "";

	[SerializeField]
	public Button CancelButton;

	[SerializeField]
	public Text TitleText;

	[SerializeField]
	public Text MessageText;


	void Start () {
		CancelButton.onClick.AddListener(OnButtonClick);
	}

	void OnEnable() {
		TitleText.text = Title;
		MessageText.text = Message;
	}

	void OnDisable() {
		TitleText.text = "";
		TitleText.text = "";
		MessageText.text = "";
	}

	void Update () {
	}

	void OnButtonClick()
	{
		gameObject.SetActive(false);
	}
}
