using System;
using System.Collections.Generic;

/*
	"skillId": 4,
    "valueId": 2,
    "skillboxId": 4,
    "skillName": "empathy_skill",
    "valueName": "Compassion",
    "skillboxName": "Empathy"
*/
[Serializable]
public class Skill {

	public int skillId = 0;
	public int valueId = 0;
	public int skillboxId = 0;
	public string skillName = "";
	public string valueName = "";
	public string skillboxName = "";
	public int level = 0;
	public float maxLevel = 100;

	// override object.Equals
	public override bool Equals (object obj)
	{		
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}

		return (obj as Skill).skillId == this.skillId;
	}
	

}

public class SkillHolder
{
	public List<Skill> skills;
}

[Serializable]
public class QuestSkill
{
	public string name;
}
