using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using UniRx;

public class ValueSwitch : MonoBehaviour
{

    [SerializeField] public Toggle Autority;
    [SerializeField] public Toggle Compassion;
    [SerializeField] public Toggle Intelligence;
    [SerializeField] public ToggleGroup Group;

    void Start()
    {
        Autority.onValueChanged.AddListener(onChange);
		Compassion.onValueChanged.AddListener(onChange);
		Intelligence.onValueChanged.AddListener(onChange);
    }

    private void onChange(bool val)
    {
        var a = Group.ActiveToggles().FirstOrDefault();
		if (a == null) { setValue(VALUE.Default); return; }
		if (!val) return;

        if (a.Equals(Autority)) setValue(VALUE.Autority);
        else if (a.Equals(Compassion)) setValue(VALUE.Compassion);
        else if (a.Equals(Intelligence)) setValue(VALUE.Intelligence);
    }

    private void setValue(VALUE v)
    {
        var values = ProfileRepository.Instance.LoadProfile().values;
		var def = ProfileRepository.Instance.LoadProfile().chosenValue.valueId;

		if (values == null || values.Count <= 0) return;

		switch (v)
		{
			case VALUE.Default: 	 sendValue(def); break;
			case VALUE.Autority: 	 sendValue(values.Find(x => x.name == "Authority").valueId); break;
			case VALUE.Compassion:   sendValue(values.Find(x => x.name == "Compassion").valueId); break;
			case VALUE.Intelligence: sendValue(values.Find(x => x.name == "Intelligence").valueId); break;
		}
    }

	private void sendValue(string valueId)
	{
		if (valueId == null || valueId == "")
			return;

		RestClient.changeValue(PlayerPrefs.GetString("token", ""), valueId)
			.Subscribe(
				res => Debug.Log(res.text ),
				err => Debug.Log(err)
			);
	}

    private enum VALUE
    {
        Default, Autority, Compassion, Intelligence
    }
}