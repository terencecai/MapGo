using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileBehaviour : MonoBehaviour {

	[SerializeField] public Text ProfileName;
	[SerializeField] public Text CharType;
	[SerializeField] public Text Level;
	[SerializeField] public Text Exp;
	[SerializeField] public Button BackButton;
	[SerializeField] public Button EditButton; 
	[SerializeField] public Image AutorityBar;
	[SerializeField] public Image CompassionBar;
	[SerializeField] public Image IntelligenceBar;

	
	void Start () {

		BackButton.onClick.AddListener(() => gameObject.SetActive(false));

		Profile profile = ProfileRepository.Instance.LoadProfile();
		ProfileName.text = profile.nickName;
		CharType.text = profile.chosenValue.name;

		AutorityBar.fillAmount     = profile.values[0].level / 100.0f;
		CompassionBar.fillAmount   = profile.values[1].level / 100.0f;
		IntelligenceBar.fillAmount = profile.values[2].level / 100.0f;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
