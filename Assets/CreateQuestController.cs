using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CreateQuestController : MonoBehaviour
{

    [SerializeField] public List<GameObject> Screens;
    [SerializeField] public Button BackButton;

    /* First screen variables */
    [SerializeField] public InputField Title;
    [SerializeField] public InputField Description;
    [SerializeField] public Button FirstNext;

    /* Second screen variables */
    [SerializeField] public Button SecondNext;
    [SerializeField] public ToggleGroup TGroup;
    [SerializeField] public List<Toggle> Values;

    /* Third screen variables */
    [SerializeField] public Button SkillBtn;
    [SerializeField] public InputField Age;
    [SerializeField] public Button Create;

    [SerializeField] public GameObject ErrorWindow;
    [SerializeField] public GameObject SkillWindow;
    [SerializeField] public Text SkillOneName;
    [SerializeField] public Text SkillTwoName;

    private Hashmap QuestData = new Hashmap();

    private int _screenIndex;

    void Start()
    {
        //Set onClicks
        BackButton.onClick.AddListener(moveBack);
        FirstNext.onClick.AddListener(onFirstNext);
        SecondNext.onClick.AddListener(onSecondNext);
        SkillBtn.onClick.AddListener(onSkillClick);

        Create.onClick.AddListener(() =>
        {
            if (Age.text == "")
            {
                showError("Please, enter age");
                return;
            }

            if (QuestData.Get("skill1_id") == null || QuestData.Get("skill2_id") == null)
            {
                showError("Choose 2 skills");
                return;
            }

            QuestData.Add("minAge", Age.text);
            RestClient.createQuest(PlayerPrefs.GetString("token", ""), QuestData)
                .Subscribe(
                    ok => gameObject.SetActive(false),
                    err => showError(err.ToString())
                );
        });

    }

    private void onSkillClick()
    {
        var value = QuestData.Get("value");
        if (value == null) return;

        var allSkills = ProfileRepository.Instance.LoadProfile().allSkills;
        var needSkills = allSkills.FindAll(s => s.valueName == value);
        var temp = SkillWindow.GetComponent<SkillChoiceAdapter>();
        temp.skills = needSkills;
        temp.Title.text = value;
        temp.callback = (skills) =>
        {
            QuestData.Add("skill1_name", skills[0].skillName);
            QuestData.Add("skill1_id", skills[0].skillId.ToString());
            QuestData.Add("skill2_name", skills[1].skillName);
            QuestData.Add("skill2_id", skills[1].skillId.ToString());
            SkillOneName.text = QuestData.Get("skill1_name");
            SkillTwoName.text = QuestData.Get("skill2_name");
        };
        SkillWindow.SetActive(true);
    }

    private void onFirstNext()
    {
        if (Title.text == "")
        {
            showError("Type quest title, please.");
            return;
        }

        if (Description.text == "")
        {
            showError("Type quest description, please");
            return;
        }

        QuestData.Add("title", Title.text);
        QuestData.Add("description", Description.text);
        MoveForward();
    }

    private void onSecondNext()
    {
        var values = ProfileRepository.Instance.LoadProfile().values;
        var v = TGroup.ActiveToggles().FirstOrDefault();
        Debug.Log(v);
        Value va;
        if (v.Equals(Values[0]))
        {
            va = values.Find(x => x.name == "Authority");
        }
        else if (v.Equals(Values[1]))
        {
            va = values.Find(x => x.name == "Compassion");
        }
        else if (v.Equals(Values[2]))
        {
            va = values.Find(x => x.name == "Intelligence");
        }
        else
        {
            showError("Choose value");
            return;
        }

        QuestData.Add("value", va.name);
        QuestData.Add("value_id", va.valueId);

        MoveForward();
    }

    void OnEnable()
    {
        init();
    }

    void OnDisable()
    {
        Title.text = "";
        Description.text = "";
        TGroup.allowSwitchOff = true;
        Values.ForEach(v => v.isOn = false);
        TGroup.allowSwitchOff = false;

        Age.text = "";
        SkillOneName.text = "";
        SkillTwoName.text = "";

    }

    private void init()
    {
        _screenIndex = 0;
        Screens[0].SetActive(true);
        Screens[1].SetActive(false);
        Screens[2].SetActive(false);
        QuestData = new Hashmap();
    }

    private void moveBack()
    {
        if (_screenIndex == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        try
        {
            Screens[_screenIndex].SetActive(false);
            Screens[_screenIndex - 1].SetActive(true);
            _screenIndex -= 1;
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e);
        }
    }

    public void MoveForward()
    {
        if (_screenIndex == 2)
        {
            return;
        }

        try
        {
            Screens[_screenIndex].SetActive(false);
            Screens[_screenIndex + 1].SetActive(true);
            _screenIndex += 1;
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e);
        }
    }

    private void showError(string message)
    {
        var p = ErrorWindow.GetComponent<PopUp>();
        p.Title = "Error";
        p.Message = message;
        ErrorWindow.SetActive(true);
    }
}
