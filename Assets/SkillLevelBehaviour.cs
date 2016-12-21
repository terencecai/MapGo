using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLevelBehaviour : MonoBehaviour
{

    [SerializeField] public Text Title;
    [SerializeField] public Image Bar;
	public Skill Skill;

    void Start()
    {

    }

	public void Bind(Skill s, bool owned)
	{
		Skill = s;
		Title.text = Skill.skillName;
		if (owned) {
			Title.color = Color.white;
		} else {
			Title.color = Color.gray;
		}
		Bar.fillAmount = Skill.level / Skill.maxLevel;
	}

    // Update is called once per frame
    void Update()
    {

    }
}
