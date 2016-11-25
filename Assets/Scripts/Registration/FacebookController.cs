using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Facebook.Unity;
using UniRx;

public class FacebookController : MonoBehaviour {

	void Awake()
	{
		if (!FB.IsInitialized) { FB.Init(initCallback, onHideUnity); }
		else { FB.ActivateApp(); }
	}

	void Start()
	{
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
		var perms = new List<string>() { "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
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
					  result => Debug.Log(result),
			          error => Debug.Log(error)
			         );
	}

}
