using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using MapzenGo.Helpers;

public class DepotValueBehaviour : MonoBehaviour
{

    [SerializeField] public Button Submit;
    [SerializeField] public List<Toggle> Toggles;
    [SerializeField] public ToggleGroup ToggleGroup;
    [SerializeField] public Image Flipper;
    [SerializeField] public Button Close;
    [SerializeField] public GameObject SkillboxWindow;
    [SerializeField] public InputField DepotName;

    private FlipBehaviour _flipper;
    private int currentValue = 0;

    public Action SuccessCallback;
    public Location DepotLocation;

    void Start()
    {
        _flipper = Flipper.GetComponent<FlipBehaviour>();
        foreach (var toggle in Toggles)
        {
            toggle.onValueChanged.AddListener(changeValue);
        }

        Submit.onClick.AddListener(_submit);
        Close.onClick.AddListener(close);
    }

    void OnDisable() {
        DepotName.text = "";
    }

    private void close()
    {
        currentValue = 0;
        _flipper.FlipCallback(0);
        Toggles[0].isOn = true;
        gameObject.SetActive(false);
    }

    void changeValue(bool check)
    {
        if (!check) return;

        Toggle activeToggle = ToggleGroup.ActiveToggles().FirstOrDefault();
        currentValue = Toggles.IndexOf(activeToggle);
        _flipper.FlipImage(currentValue);
    }

    private void _submit()
    {
        showSkillboxWindow();
    }

    private void sendDepot(Depot depot)
    {
        RestClient.createDepot(PlayerPrefs.GetString("token", ""), depot)
                .Subscribe(
                    x =>
                    {
                        if (SuccessCallback != null) SuccessCallback();
						Debug.Log(x);
                        close();
                    },
                    e => {
                        Debug.Log(e);
                        close(); //TODO: check once again
                    }
                );
    }

    private void showSkillboxWindow()
    {
        var allSkills = ProfileRepository.Instance.LoadProfile().allSkills;
        var needSkills = allSkills.FindAll(s => s.valueId == currentValue + 1);
        var temp = SkillboxWindow.GetComponent<SkillChoiceAdapter>();
        temp.skills = needSkills;
        temp.Title.text = getValueName();
        temp.callback = (skills) => {
            var location = getDepotLocation();
            Depot depot = new Depot();
            depot.name = DepotName.text;
            depot.latitude = location.GetLatitude();
            depot.longitude = location.GetLongitude();
            depot.valueId = currentValue + 1;
            depot.skillsId = skills.Select((Skill s) => (long)s.skillId).ToList();
            sendDepot(depot);
        };
        SkillboxWindow.SetActive(true);
    }

    private Location getDepotLocation()
    {
        if (LiveParams.ComingToRealLocation) 
            return GM.PointInCircle(JsonUtility.FromJson<Location>(PlayerPrefs.GetString("last_teleport_location", "")), 50);
        else
            return GM.PointInCircle(new Location(Input.location.lastData), 50);
    }

    private string getValueName() {
        switch (currentValue)
        {
            case 0: return "Authority";
            case 1: return "Compassion";
            case 2: return "Intelligence";
        }
        return "";
    }
}
