using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using UniRx;

public class ApplicationLoadController : MonoBehaviour
{
    private ScreenManager manager;

    void Start()
    {
        GameObject.Find("Loading").GetComponent<LoadingController>().showLoading();
        manager = GetComponent<ScreenManager>();
        checkExisting();
    }

    public void OnClick(string scene)
    {
        manager.ShowNextScreen(Convert.ToInt16(scene));
    }

    public void OnBackClick()
    {
        manager.ShowPrevScreen();
    }


    //Initial configuration
    private void checkExisting()
    {
        string token = PlayerPrefs.GetString("token", "");
        if (token != "")
        {
            loadProfile(token);
        }
        else
        {
            GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading();
        }
    }

    private void loadProfile(string token)
    {
        Observable.WhenAll(
            RestClient.getProfile(token),
            RestClient.getMySkills(token),
            RestClient.getAllSkills(token))
            .Subscribe(
                x => loadGame(x),
                e => { Debug.Log(e); GameObject.Find("Loading").GetComponent<LoadingController>().hideLoading(); }
            );
    }

    private void loadGame(string[] prof)
    {
        var profile = JsonUtility.FromJson<Profile>(prof[0]);
        profile.skills = convertSkills(prof[1]);
        profile.allSkills = convertSkills(prof[2]);
        ProfileRepository.Instance.SaveProfileJson(profile);
        SceneManager.LoadSceneAsync("CachedDynamicLoader");
    }

    public static List<Skill> convertSkills(string json)
    {
        return JsonUtility.FromJson<SkillHolder>("{ \"skills\": " + json + "}").skills;
    }
}
