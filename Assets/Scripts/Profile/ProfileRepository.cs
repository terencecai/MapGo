using System;
using UnityEngine;
using UniRx;

public class ProfileRepository  {

	private static ProfileRepository instance;

	private ProfileRepository() {}

	public static ProfileRepository Instance 
	{
		get 
		{
			if (instance == null) 
				instance = new ProfileRepository();
			return instance;
		}
	}

	//TODO: implement Serializable
	public Profile GetProfie() {
		var profile = new Profile();

		profile.nickName 	= PlayerPrefs.GetString("profile_name", "");
		profile.birthDay 	= PlayerPrefs.GetString("profile_birthday", "");
		profile.gender 		= PlayerPrefs.GetString("profile_gender", "");
		profile.credentials = new Credentials(
									PlayerPrefs.GetString("email", ""),
									PlayerPrefs.GetString("password", ""));

		return profile;
	}

	//TODO: implement Serializable
	public void SaveProfile(Profile profile) {
		PlayerPrefs.SetString("profile_name",     profile.nickName);
		PlayerPrefs.SetString("profile_birthday", profile.birthDay);
		PlayerPrefs.SetString("profile_gender",   profile.gender);
		if (profile.credentials != null) 
		{
			PlayerPrefs.SetString("email",    profile.credentials.GetEmail());
			PlayerPrefs.SetString("password", profile.credentials.GetPassword());
		}

		PlayerPrefs.Save();
	}

	public void SaveProfileJson(Profile profile) {
		var s = JsonUtility.ToJson(profile);
		PlayerPrefs.SetString("profile", s);
		PlayerPrefs.Save();
	}

	public void SaveProfileJson(string profile) {

		PlayerPrefs.SetString("profile", profile);
		PlayerPrefs.Save();
	}

	public Profile LoadProfile() {
		return JsonUtility.FromJson<Profile>(PlayerPrefs.GetString("profile", ""));
	}

	public void UpdateProfile()
	{
		sync(PlayerPrefs.GetString("token", ""));
	}

	private void sync(string token)
    {
        Observable.WhenAll(
            RestClient.getProfile(token),
            RestClient.getMySkills(token),
            RestClient.getAllSkills(token))
            .Subscribe(
                update,
                Debug.Log
            );
    }

    private void update(string[] prof)
    {
        var profile = JsonUtility.FromJson<Profile>(prof[0]);
        profile.skills = ApplicationLoadController.convertSkills(prof[1]);
        profile.allSkills = ApplicationLoadController.convertSkills(prof[2]);
        ProfileRepository.Instance.SaveProfileJson(profile);
	}
}
