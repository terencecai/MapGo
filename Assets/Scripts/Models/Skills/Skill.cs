using System;


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

	// override object.Equals
	public override bool Equals (object obj)
	{		
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		
		// TODO: write your implementation of Equals() here
		return (obj as Skill).skillId == this.skillId;
	}
	

}
