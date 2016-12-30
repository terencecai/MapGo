using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DetailedController : MonoBehaviour {

	[SerializeField] public Text username;
	[SerializeField] public Text title;
	[SerializeField] public Text description;
	[SerializeField] public Text age;
	[SerializeField] public Text val;
	[SerializeField] public Button answers;
	[SerializeField] public Button back;
	[SerializeField] public Button pinQuest;
	[SerializeField] public Button unpinQuest;
	[SerializeField] public GameObject AnswersPanel;
	[SerializeField] public GameObject MessageWindow;

	private Quest _quest;

	void Start() {
		
		answers.onClick.AddListener(OnAnswersClick);
		back.onClick.AddListener(() => gameObject.SetActive(false));
		pinQuest.onClick.AddListener(() =>
		{
			RestClient.pinQuest(PlayerPrefs.GetString("token", ""), _quest.id)
				.Subscribe(
					x => onPin(),
					e => { Debug.Log(e); pinQuest.gameObject.SetActive(false); }
				);
		});

		unpinQuest.onClick.AddListener(() =>
		{
			RestClient.unpinQuest(PlayerPrefs.GetString("token", ""), _quest.id)
				.Subscribe(
					x => onUnPin(),
					e => Debug.Log(e)	
				);
		});
	}

	void OnEnable()
	{
		if (_quest.pinnedByMe)
		{
			pinQuest.gameObject.SetActive(false);
			unpinQuest.gameObject.SetActive(true);
		}
		else
		{
			pinQuest.gameObject.SetActive(true);
			unpinQuest.gameObject.SetActive(false);
		}
	}

	private void onPin()
	{
		pinQuest.gameObject.SetActive(false);
		unpinQuest.gameObject.SetActive(true);
		showMessage("Quest successfully pinned");
	}

	private void onUnPin()
	{
		pinQuest.gameObject.SetActive(true);
		unpinQuest.gameObject.SetActive(false);
		showMessage("Quest successfully unpinned");
	}

	public void AttachQuestAndShow(Quest quest) {
		_quest = quest;
		title.text       = quest.title;
		description.text = quest.description;
		age.text 	  	 = quest.minAge.ToString() + "+";
		val.text 	  	 = quest.value.name;
		username.text 	 = quest.creator.nickName;
		gameObject.SetActive(true);
	}

	void OnDisable()
	{
		pinQuest.gameObject.SetActive(true);
	}

	void OnAnswersClick() {
		AnswersPanel.GetComponent<AnswersManager>().QuestId = _quest.id;
		AnswersPanel.GetComponent<AnswersManager>().Quest = _quest;
		AnswersPanel.SetActive(true);
	}

	private void showMessage(string message)
	{
		var p = MessageWindow.GetComponent<PopUp>();
		p.Title = "Favorite";
		p.Message = message;
		MessageWindow.SetActive(true);
	}
}
