using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour {

	public string Title = "";
	public string Message = "";

	[SerializeField] public Button CancelButton;
	[SerializeField] public Text TitleText;
	[SerializeField] public Text MessageText;


	void Start () {
		CancelButton.onClick.AddListener(OnButtonClick);
	}

	void OnEnable() {
		TitleText.text = Title;
		MessageText.text = Message;
	}

	void OnDisable() {
		TitleText.text = "";
		MessageText.text = "";
	}

	void OnButtonClick()
	{
		gameObject.SetActive(false);
	}
}
