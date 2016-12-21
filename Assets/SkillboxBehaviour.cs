using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class SkillboxBehaviour : MonoBehaviour
{

    [SerializeField] public Button Close;
	[SerializeField] public List<GameObject> SkillPanels;
	[SerializeField] public Text TitleText;
    public string Title;
	public List<Skill> Skills;

    void Start()
    {
		Close.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void OnEnable()
    {
		TitleText.text = Title;
		var mySkills = ProfileRepository.Instance.LoadProfile().skills;
		var count = Mathf.Max(Skills.Count, 5);

		SkillLevelBehaviour temp;
		for (int i = 0; i < count; i++)
		{
			temp = SkillPanels[i].GetComponent<SkillLevelBehaviour>();
			var ms = mySkills.Find(x => x.skillId == Skills[i].skillId);
			if (ms != null) {
				temp.Bind(ms, true);
			} else {
				temp.Bind(Skills[i], false);
			}
		}
    }

}
