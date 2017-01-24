using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
public class RegistrationController : MonoBehaviour
{

	[SerializeField] public Canvas PopupWindow;

	[SerializeField] public Toggle maleButton;
	[SerializeField] public Toggle femaleButton;
	[SerializeField] public InputField nameField;
	[SerializeField] public Button submit;

	[SerializeField] public InputField day;
	[SerializeField] public InputField month;
	[SerializeField] public InputField year;

	[SerializeField] public Button addPhoto;
	[SerializeField] public Button backButton;

	private string birthdayPattern = "^(0[1-9]|[12][0-9]|3[01])[. /.]([1-9]|1[012])[. /.](19|20)\\d\\d$";

	private PopUp popup;
	private Validator validator;

	private string gender = "";
	private string username = "";
	private string birthday = "";

	[SerializeField] public GameObject SceneController;
	private ScreenManager screenManager;

	void Start()
	{
		screenManager = SceneController.GetComponent<ScreenManager>();

		setListeners();
		popup = PopupWindow.GetComponent<PopUp>();
		validator = new Validator(showValidationError);
		setValidation();
	}

	private void setListeners()
	{
		maleButton.onValueChanged.AddListener((value) =>
		{
			if (!value) return;

			gender = "male";
			if (femaleButton.isOn) { femaleButton.isOn = false; }
		});

		femaleButton.onValueChanged.AddListener((value) =>
		{
			if (!value) return;

			gender = "female";
			if (maleButton.isOn) { maleButton.isOn = false; }
		});

		submit.onClick.AddListener(onSubmitClick);
		addPhoto.onClick.AddListener(loadPhoto);
		backButton.onClick.AddListener(() => {
			screenManager.ShowPrevScreen();
		});
	}

	private void setValidation() {
		validator.AddValidator(() => { return !(username.Equals("")); }, "Please, enter your name");
		validator.AddValidator(() => { return Regex.IsMatch(birthday, birthdayPattern); }, "Please, enter valid birth date");
		validator.AddValidator(() => 
		{ 
			return (!((gender.Equals("")) || (maleButton.isOn && femaleButton.isOn) ||
					(!maleButton.isOn && !femaleButton.isOn))); 
		}, "Please, choose your gender");
	}

	private void checkPrefs() {
		var name = PlayerPrefs.GetString("profile_name", "");
		var age = PlayerPrefs.GetString("profile_birthday", "");
		var gender = PlayerPrefs.GetString("profile_gender", "");

		if (!name.Equals("")) nameField.text = name;
		if (!gender.Equals("")) {
			switch (gender) {
				case "male": 
					maleButton.isOn = true;
					femaleButton.isOn = false;
					break;
				case "female":
					maleButton.isOn = false;
					femaleButton.isOn = true;
					break;
				default: break;
			}
		}

		if (!age.Equals("")) {
			var dates = age.Split('.');
			day.text = dates[0];
			month.text = dates[1];
			year.text = dates[2];
		}
	}

	private void collectData()
	{
		username = nameField.text;
		birthday = day.text + "." + month.text + "." + year.text;
	}

	void onSubmitClick()
	{
		collectData();
		if (!validator.PerformValidate()) return;

		Profile user = new Profile();
		user.nickName     = username;
		user.birthDay = birthday;
		user.gender   = gender;

		saveUserForStep(user);
		screenManager.ShowNextScreen(3);
	}

	private void saveUserForStep(Profile user)
	{
		ProfileRepository.Instance.SaveProfile(user);
	}

	private void showValidationError(string message)
	{
		popup.Title = "Error!";
		popup.Message = message;
		PopupWindow.gameObject.SetActive(true);
	}

	private void loadPhoto() {
	
	}

}
