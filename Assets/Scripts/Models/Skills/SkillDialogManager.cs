using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillDialogManager : MonoBehaviour {

	
	[SerializeField] public Button DeclineButton;
    [SerializeField] public Button AcceptButton;
	[SerializeField] public Text MessageText;

	public Skill Skill;
	public GameObject skillObject;

	void Start () {
		DeclineButton.onClick.AddListener(OnDeclineClick);
        AcceptButton.onClick.AddListener(OnAcceptClick);
	}

	void OnEnable() {
		MessageText.text = "Do you want to pickup: \"" + Skill.skillName + "\"?";
	}

	void OnDisable() {
		skillObject = null;
		Skill = null;
	}

	void OnDeclineClick()
	{
		gameObject.SetActive(false);
	}

    void OnAcceptClick() 
    {
		skillObject.GetComponent<SkillBehaviour>().pickup();
		gameObject.SetActive(false);
    }
}
