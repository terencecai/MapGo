using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestItemController : MonoBehaviour {

	[SerializeField] public Text QuestTitle;
	[SerializeField] public Text QuestValue;
	[SerializeField] public Text QuestSkill;
	[SerializeField] public Text QuestAge;
	[SerializeField] public Text QuestTime;
	[SerializeField] public Image AuthorAvatar; 

	[SerializeField] public Button PressedBack;

	public Action<Quest> showDetailed;
	private Quest _quest;

	void Start () {
		PressedBack.onClick.AddListener(() => {
			showDetailed(_quest);
		});
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void bind(Quest quest) {
		_quest = quest;
		try {
			QuestTitle.text = quest.title;
			QuestValue.text = quest.value.name;
			QuestAge.text = quest.minAge.ToString() + "+";
			QuestSkill.text = "";
		} catch (Exception e) {
			Debug.Log(e);
		}
	}
}
