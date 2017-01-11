using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;
public class AnswersManager : MonoBehaviour {

	[SerializeField] public GameObject AnswerPrefab;
	[SerializeField] public GameObject ErrorLabel;
	[SerializeField] public GameObject Loading;
	[SerializeField] public GameObject Content;
	[SerializeField] public Button Back; 
	[SerializeField] public GameObject AnswerDialog;
	[SerializeField] public Button Add;
	[SerializeField] public ScrollRect scroll;
	private List<GameObject> visibleAnswers = new List<GameObject>();

	public String QuestId;
	public Quest Quest;
	void Start () {
		if (QuestId == null || QuestId == "") {
			ErrorLabel.SetActive(true);
		} else {
			loadAnswers(QuestId);
		}

		Back.onClick.AddListener(() => gameObject.SetActive(false));
		Add.onClick.AddListener(AddAnswer);
	}

	void OnEnable() {
		if (QuestId == null || QuestId == "") {
			ErrorLabel.SetActive(true);
		} else {
			loadAnswers(QuestId);
		}
	}

	void parseResponse(string answersJson) {
		var winned = answersJson.Contains("\"winner\":true");
		scroll.verticalNormalizedPosition = 0.5f;
		Loading.SetActive(false);
		var json = new JSONObject(answersJson);

		if (!json.IsArray || json.list.Count <= 0) {
			AnswerPrefab.SetActive(false);
			ErrorLabel.SetActive(true);
			return;
		}
		if (!AnswerPrefab.activeSelf)
			AnswerPrefab.SetActive(true);

		Answer answer;
		GameObject item;
		var scaleVector = Vector3.one;

		for (int i = 0; i < json.list.Count; i++) {
			answer = JsonUtility.FromJson<Answer>(json.list[i].print());
			if (i == 0) { 
				bind(answer, AnswerPrefab.GetComponent<AnswersItemManager>(), winned);
				continue;
			}

			item = getAnswerIfExist(i);
			if (item != null) {
				item.SetActive(true);
				bind(answer, AnswerPrefab.GetComponent<AnswersItemManager>(), winned);
				continue;	
			}

			item = Instantiate(AnswerPrefab) as GameObject;
			item.transform.parent = Content.transform;
			item.transform.localScale = scaleVector;
			visibleAnswers.Add(item);
			bind(answer, AnswerPrefab.GetComponent<AnswersItemManager>(), winned);
		}
	}

	private GameObject getAnswerIfExist(int i) {
		if (i >= visibleAnswers.Count) 
			return null;

		return visibleAnswers[i];
	}

	void parseError(Exception e) {
		Loading.SetActive(false);
		ErrorLabel.SetActive(true);
		Debug.Log(e);
	}

	void loadAnswers(string questId) {
		Loading.SetActive(true);
		ErrorLabel.SetActive(false);
		clear();

		RestClient.getAnswers(PlayerPrefs.GetString("token", ""), questId)
			.Subscribe(
				resp => parseResponse(resp.text),
				err  => parseError(err)
			);
	}

	void bind(Answer answer, AnswersItemManager item, bool winned) {
		item.bind(answer, Quest, loadAnswers, winned);
	}

	void clear() {
		AnswerPrefab.SetActive(false);
		visibleAnswers.ForEach(x => x.SetActive(false));
	}

	void AddAnswer() {
		AnswerDialog.GetComponent<CreateAnswer>().questId = QuestId;
		AnswerDialog.GetComponent<CreateAnswer>().callback = loadAnswers;
		AnswerDialog.SetActive(true);
	}
}
