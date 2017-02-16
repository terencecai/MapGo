using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


public class ProfileController : MonoBehaviour
{

    [SerializeField] public Text Level;
    [SerializeField] public Text ValueLabe;
    [SerializeField] public Image ValueImage;
    [SerializeField] public InputField Nickname;
    [SerializeField] public InputField Password;
    [SerializeField] public InputField Day;
    [SerializeField] public InputField Month;
    [SerializeField] public InputField Year;
    [SerializeField] public Toggle Male;
    [SerializeField] public Toggle Female;
    [SerializeField] public Button Save;
    [SerializeField] public Button Back;

    [SerializeField] public GameObject ErrorWindow;

    [SerializeField] public List<Sprite> ValuePictures;

    private Profile profile;
    private Validator validator;
    private string birthdayPattern = "^(0[1-9]|[12][0-9]|3[01])[. /.](0?[1-9]|1[012])[. /.](19|20)\\d\\d$";

    void Start()
    {
        profile = ProfileRepository.Instance.LoadProfile();
        initValidators();
        fill();
        Save.onClick.AddListener(send);
        Back.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void fill()
    {
        Nickname.text = profile.nickName;
        Password.text = PlayerPrefs.GetString("password", "");
        if (profile.gender.ToLower() == "male")
        {
            Male.isOn = true;
        }
        else
        {
            Female.isOn = true;
        }
        try
        {
            var b = profile.getBirthdayString().Split('.');
            Day.text = b[0];
            Month.text = b[1];
            Year.text = b[2];
        }
        catch (IndexOutOfRangeException) { }

        ValueLabe.text = profile.chosenValue.name;
        if (profile.chosenValue.name == "Authority")
        {
            ValueImage.sprite = ValuePictures[0];
        }
        else if (profile.chosenValue.name == "Compassion")
        {
            ValueImage.sprite = ValuePictures[1];
        }
        else
        {
            ValueImage.sprite = ValuePictures[2];
        }
    }

    private void initValidators()
    {
        validator = new Validator(showError);
        validator.AddValidator(() => Nickname.text != "", "Enter valid nickname.");
        validator.AddValidator(() => Password.text.Length >= 6, "Password length must be at least 6 characters");
        validator.AddValidator(() => Regex.IsMatch(birthday(), birthdayPattern), "Enter valid birth date");
        validator.AddValidator(() =>
        {
            return (!((Male.isOn && Female.isOn) ||
                    (!Male.isOn && !Female.isOn)));
        }, "Please, choose your gender");
    }

    private void showError(string message)
    {
        var p = ErrorWindow.GetComponent<PopUp>();
        p.Title = "Error";
        p.Message = message;

        ErrorWindow.SetActive(true);
    }

    private string birthday()
    {
        return Day.text + "." + Month.text + "." + Year.text;
    }

    private void send()
    {
        if (!validator.PerformValidate())
            return;

        var p = new Profile();
        p.credentials = new Credentials("", Password.text);
        p.nickName = Nickname.text;
        if (Male.isOn)
            p.gender = "MALE";
        else if (Female.isOn)
            p.gender = "FEMALE";

        p.birthDay = birthday();
		p.chosenValue = profile.chosenValue;

        RestClient.updateProfile(PlayerPrefs.GetString("token", ""), p)
            .Subscribe(
                x => gameObject.SetActive(false),
                e => showError(e.ToString())
            );
    }

}
