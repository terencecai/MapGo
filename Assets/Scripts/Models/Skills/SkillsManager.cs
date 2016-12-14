using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SkillsManager : MonoBehaviour {
	private static int MAX_SKILL_POOL_SIZE = 10; 
	[SerializeField] public GameObject skillPrefab;
	[SerializeField] public GameObject SkillDialog;

	private List<Skill> activeSkills = new List<Skill>();
	private List<GameObject> skillsPool = new List<GameObject>(MAX_SKILL_POOL_SIZE);
	private GameObject player;
	void Start () {
		UpdateSkills();
		player = GameObject.Find("ThirdPersonController");

		if (skillsPool.Count > 0) skillsPool.Clear();
		for (int i = 0; i < MAX_SKILL_POOL_SIZE; i++)
		{
			skillsPool.Add(Instantiate(skillPrefab, Vector3.zero, transform.rotation) as GameObject);
			skillsPool[i].SetActive(false);
		}
	}

	void Update() {
	}

	private void clearPool() {
		activeSkills = new List<Skill>();
		foreach (var p in skillsPool) {
			p.SetActive(false);
		}
	}

	public void UpdateSkills() {
		RestClient.requestSkills(PlayerPrefs.GetString("token", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJvbGVnc2hlbGlha2luQGdtYWlsLmNvbSJ9.HF0-sjvvFuUtkVsTt5srdqtKYZpRfNwWSA6xBi0Cinj9LvIbiqOArhFm39y5gCNAPC3PFpRo8qqlW-Zw5rWDlQ"))
			.Subscribe(
				success => { throwSkills(success.text); },
				failure => { Debug.Log(failure); }
			);
	}

	private void throwSkills(string skillsJson) {
		clearPool();
		var ls = new JSONObject(skillsJson);
		var lst = ls.list.GetRange(0, Mathf.Min(MAX_SKILL_POOL_SIZE, ls.list.Count));
		var playerPosition = player.transform.position;

		for (int i = 0; i < lst.Count; i++)
		{
			var inside = Random.insideUnitCircle * 80;
			var newPos = new Vector3(playerPosition.x + inside.x, 2, playerPosition.z + inside.y);

			var skill = JsonUtility.FromJson<Skill>(lst[i].print());
			activeSkills.Add(skill);
			skillsPool[i].transform.position = newPos;
			skillsPool[i].SetActive(true);	
			skillsPool[i].GetComponent<SkillBehaviour>().Skill = skill;
			skillsPool[i].GetComponent<SkillBehaviour>().skillDialog = SkillDialog;
		}
	}

	public void PickupSkill(GameObject go) {
		if (go == null) {
			Debug.Log("Skill is nooolll!");
			return;
		}
		var skill = go.GetComponent<SkillBehaviour>().Skill; 
		go.GetComponent<SkillBehaviour>().pickup();
		RestClient.pickupSkill(PlayerPrefs.GetString("token", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJvbGVnc2hlbGlha2luQGdtYWlsLmNvbSJ9.HF0-sjvvFuUtkVsTt5srdqtKYZpRfNwWSA6xBi0Cinj9LvIbiqOArhFm39y5gCNAPC3PFpRo8qqlW-Zw5rWDlQ"),
								skill.skillId.ToString()).Subscribe(x => Debug.Log(x), e => Debug.Log(e));
	}
}
