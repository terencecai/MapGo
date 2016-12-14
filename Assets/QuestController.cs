using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class QuestController : MonoBehaviour {

	[SerializeField] public GameObject QuestItem;
	[SerializeField] public GameObject Content;
	[SerializeField] public List<Toggle> Toggles;
	[SerializeField] public ToggleGroup ToggleGroup;
	[SerializeField] public GameObject Loading;
	[SerializeField] public GameObject ErrorLabel;
	[SerializeField] public GameObject DetailedView;

	private List<GameObject> shownQuests = new List<GameObject>();

	void Start () {
		loadSkills();
		Toggles.ForEach(t => t.onValueChanged.AddListener(onToggle));
	}

	private void onToggle(bool value) 
	{
		if (!value)
			return;
		var toggle = ToggleGroup.ActiveToggles().FirstOrDefault();
		if (toggle.Equals(Toggles[0])) {
			clearQuests();
		} else if (toggle.Equals(Toggles[1])) {
			loadSkills();
		}
	}

	private void parseError(Exception e) {
		ErrorLabel.SetActive(true);
		Loading.SetActive(false);
		QuestItem.SetActive(false);
	}

	private void parseQuests(JSONObject json) 
	{
		Loading.SetActive(false);
		if (!json.IsArray || json.list.Count <= 0) {
			QuestItem.SetActive(false);
			return;
		}
		if (!QuestItem.activeSelf)
			QuestItem.SetActive(true);
	
		Quest quest;
		GameObject item;
		var scaleVector = Vector3.one;
		
		for (int i = 0; i < json.list.Count; i++) {
			quest = JsonUtility.FromJson<Quest>(json.list[i].print());
			if (i == 0) { 
				fillQuestWithData(quest, QuestItem.GetComponent<QuestItemController>());
				continue;
			}

			item = getQuestObjectIfExist(i);
			if (item != null) {
				item.SetActive(true);
				fillQuestWithData(quest, item.GetComponent<QuestItemController>());
				continue;	
			}

			item = Instantiate(QuestItem) as GameObject;
			item.transform.parent = Content.transform;
			item.transform.localScale = scaleVector;
			shownQuests.Add(item);
			fillQuestWithData(quest, item.GetComponent<QuestItemController>());
		}
	}

	private void clearQuests() {
		QuestItem.SetActive(false);
		shownQuests.ForEach((obj) => obj.SetActive(false));
	}

	private GameObject getQuestObjectIfExist(int index) {
		if (index >= shownQuests.Count) 
			return null;

		return shownQuests[index];
	}

	private void loadSkills() {
		ErrorLabel.SetActive(false);
		clearQuests();
		Loading.SetActive(true);
		RestClient.getQuests(PlayerPrefs.GetString("token", ""))
			.Subscribe(
				x => parseQuests(new JSONObject(x.text)),
				e => parseError(e)
			);
	}

	private void fillQuestWithData(Quest quest, QuestItemController view) {
		var d = DetailedView.GetComponent<DetailedController>();
		view.showDetailed = (q) => d.AttachQuestAndShow(q);
		view.bind(quest);
	}
}
