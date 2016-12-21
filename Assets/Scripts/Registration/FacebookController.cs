using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Facebook.Unity;
using UniRx;

public class FacebookController : MonoBehaviour {

	[SerializeField] public Canvas PopupWindow;
	private PopUp popup;

	void Awake()
	{
		if (!FB.IsInitialized) { FB.Init(initCallback, onHideUnity); }
		else { FB.ActivateApp(); }
	}

	void Start()
	{

		popup = PopupWindow.GetComponent<PopUp> ();
		GameObject.Find("FBLogin").GetComponent<Button>().onClick.AddListener(() =>
		{
			LoginWsithFB();
		});
	}

	private void initCallback()
	{
		if (FB.IsInitialized) { FB.ActivateApp(); }
		else { Debug.Log("Failed to init Facebook SDK!"); }
	}

	private void onHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}
	}

	void LoginWsithFB()
	{
		var perms = new List<string>() { "public_profile", "email", "user_friends", "user_birthday" };
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			var aToken = result.AccessToken;
			Debug.Log("Facebook token: " + aToken.TokenString);
			sendLogin(aToken.TokenString);
		}
		else {
			Debug.Log("Failed to log in");
		}
	}

	private void sendLogin(string token)
	{
		RestClient.sendFBToken(token)
				  .Subscribe(
					result => { parseSuccess(result); },
					error => { parseError(error); }
			      );
	}

	private void parseSuccess(WWW response) {
		try {
			string token = new JSONObject (response.text)["accessToken"].str;
			PlayerPrefs.SetString ("token", token);
			RestClient.getProfile(token)
				.Subscribe(
					x => parseProfile(x),
					e => showValidationError(e.ToString())
				);
		} catch (Exception e) {
			Debug.Log(e);
			showValidationError(e.ToString());
		}
		
	}

	private void parseProfile(string profileJson) {
		ProfileRepository.Instance.SaveProfileJson(profileJson);
		SceneManager.LoadSceneAsync ("CachedDynamicLoader");
	}

	private void parseError(Exception e) {
		if (!(e is WWWErrorException)) {
			showValidationError (e.ToString ());
			return;
		}

		var err = new JSONObject((e as UniRx.WWWErrorException).Text);
		try {
			showValidationError (err["message"].str);

		} catch (Exception ee) {
			showValidationError (e.ToString ());
		}
	}

	private void showValidationError(string message)
	{
		popup.Title = "Error!";
		popup.Message = message;
		PopupWindow.gameObject.SetActive(true);
	}

}
