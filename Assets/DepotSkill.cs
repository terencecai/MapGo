using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepotSkill : MonoBehaviour {

	[SerializeField] public Image Picture;
	[SerializeField] public Text SkillboxName;
	[SerializeField] public Text SkillName;

	public void SetSkill(Skill skill, Sprite image)
	{
		SkillboxName.text = skill.skillboxName;
		SkillName.text = skill.skillName;
		Picture.sprite = image;
	}
}
