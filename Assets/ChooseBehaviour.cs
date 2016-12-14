using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
public class ChooseBehaviour : MonoBehaviour {


	[SerializeField] public Button Submit;
	[SerializeField] public List<Toggle> Toggles;
	[SerializeField] public ToggleGroup ToggleGroup;
	[SerializeField] public Image Flipper;
	private FlipBehaviour _flipper;
	private int currentValue = 0;

	void Start() 
	{
		_flipper = Flipper.GetComponent<FlipBehaviour>();
		foreach (var toggle in Toggles)
		{
			toggle.onValueChanged.AddListener(changeValue);
		}

		Submit.onClick.AddListener(_submit);
	}

	void changeValue(bool check) 
	{
		if (!check) return;

		Toggle activeToggle = ToggleGroup.ActiveToggles().FirstOrDefault();
		currentValue = Toggles.IndexOf(activeToggle);
		_flipper.FlipImage(currentValue);
	}

	private void _submit() {
		RestClient.chooseValue(PlayerPrefs.GetString("token", ""),
								currentValue + 1)
				.Subscribe( 
					x => GameObject.Find("ChooseChar").SetActive(false),
					e => Debug.Log(e)
				);
	}


}
