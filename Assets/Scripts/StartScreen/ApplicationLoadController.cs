using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx;

public class ApplicationLoadController : MonoBehaviour {
	private ScreenManager manager;

	void Start () {
		GameObject.Find("Loading").GetComponent<LoadingController>().showLoading();
		manager = GetComponent<ScreenManager>();
		checkExisting();
	}

	public void OnClick(string scene)
	{
		manager.ShowNextScreen(Convert.ToInt16(scene));
	}

	public void OnBackClick() {
		manager.ShowPrevScreen();
	}


	//Initial configuration
	private void checkExisting()
	{
		string token = PlayerPrefs.GetString ("token", "");
		if (token != "") {
			loadProfile(token);
		} else {
			GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
		}
	}

	private void loadProfile(string token) 
	{
		RestClient.getProfile(token)
				.Subscribe(
					x => loadGame(x.text),
					e => { Debug.Log(e); GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading(); }
				);
	}

	private void loadGame(string profileJson) 
	{
		var p = JsonUtility.FromJson<Profile>(profileJson);
		ProfileRepository.Instance.SaveProfileJson(profileJson);
		SceneManager.LoadSceneAsync("CachedDynamicLoader");
	}
}
