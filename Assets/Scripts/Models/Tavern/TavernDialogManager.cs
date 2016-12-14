using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TavernDialogManager : MonoBehaviour {

	[SerializeField] public Button DeclineButton;
    [SerializeField] public Button AcceptButton;
	[SerializeField] public Text MessageText;

	public string TavernName;

	void Start () {
		DeclineButton.onClick.AddListener(OnDeclineClick);
        AcceptButton.onClick.AddListener(OnAcceptClick);
	}

	void OnEnable() {
		MessageText.text = "Are you want to enter \"" + TavernName + "\"?";
	}

	void OnDeclineClick()
	{
		gameObject.SetActive(false);
	}

    void OnAcceptClick() 
    {
		//TODO: Blocked feature
		SceneManager.LoadSceneAsync("Tavern");
		// gameObject.SetActive(false);
    }

}
