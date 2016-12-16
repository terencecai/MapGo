using UnityEngine;
using UniRx;
using Hash = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
public class RestClient : MonoBehaviour {

	public static string URL = "http://ec2-35-156-153-137.eu-central-1.compute.amazonaws.com:9999";
	//  public static string URL = "http://192.168.10.170:8888";

	public static IObservable<WWW> login(Credentials credentials)
	{
		WWWForm form = new WWWForm();
		form.AddField("email", credentials.GetEmail());
		form.AddField("password", credentials.GetPassword());
		return ObservableWWW.PostWWW(URL + "/login", form);
	}

	public static IObservable<WWW> register(Profile user)
	{
		WWWForm form = new WWWForm();
		form.AddField("email", user.credentials.GetEmail());
		form.AddField("password", user.credentials.GetPassword());
		form.AddField("nickName", user.nickName);
		form.AddField("secondName", "vasyatest");
		form.AddField("gender", user.gender.ToUpper());
		
		return ObservableWWW.PostWWW(URL + "/registration", form);
	}

	public static IObservable<WWW> sendFBToken(string token)
	{
		WWWForm form = new WWWForm();
		form.AddField("accessToken", token);
		return ObservableWWW.PostWWW(URL + "/facebook_login", form);
	}

	public static IObservable<WWW> verifyEmail(Credentials creds, string code) {
		WWWForm form = new WWWForm();
		form.AddField("email", creds.GetEmail());
		form.AddField("password", creds.GetPassword());
		form.AddField("code", code);

		return ObservableWWW.PostWWW(URL + "/verify_email", form);
	}

	public static IObservable<WWW> requestCode(Credentials creds) {
		return ObservableWWW.GetWWW (URL + "/send_verification_code?email=" + creds.GetEmail ());
	}

	public static IObservable<WWW> requestPassword(string email) {
		return ObservableWWW.GetWWW (URL + "/reset_password?email=" + email);
	}

	public static IObservable<WWW> requestSkills(string token) {
		return ObservableWWW.GetWWW(URL + "/skills/generate_test", new Hash(){{ "X-Auth-Token", token }});
	} 

	public static IObservable<WWW> pickupSkill(string token, string skillId) {
		WWWForm form = new WWWForm();
		form.AddField("skillId", skillId);
		return ObservableWWW.PostWWW(URL + "/profile/skill", form, new Hash(){{ "X-Auth-Token", token }});
	}

	public static IObservable<WWW> chooseValue(string token, int valueId) {
		var s = "{\"chosenValueId\":" + valueId + "}";
		Debug.Log(s);
		var b = Encoding.UTF8.GetBytes(s);
		
		return ObservableWWW.PostWWW(URL + "/profile", b, 
									new Hash(){{"X-HTTP-Method-Override", "PUT"},
											   { "X-Auth-Token", token },
											   { "Content-Type", "application/json" }});
	}

	public static IObservable<WWW> getProfile(string token) {
		return ObservableWWW.GetWWW(URL + "/profile", new Hash() {{ "X-Auth-Token", token }});
	}

	//------------------------
	//QUESTS
	//------------------------
	public static IObservable<WWW> getQuests(string token) {
		return ObservableWWW.GetWWW(URL + "/quests", new Hash() {{ "X-Auth-Token", token }});
	}

	public static IObservable<WWW> getAnswers(string token, string questId) {
		return ObservableWWW.GetWWW(URL + "/quests/" + questId + "/answers", new Hash() {{ "X-Auth-Token", token }});
	}

	//------------------------
	//Taverns
	//------------------------
	public static IObservable<WWW> getTaverns(string token, double lat, double lon) {
		return ObservableWWW.GetWWW(URL + "/nearest_entities?latitude=" + lat + "&longitude=" + lon, new Hash() {{ "X-Auth-Token", token }});
	}

	//------------------------
	//Debug
	//------------------------
	public static IObservable<WWW> sendDebug(string data) {
		var form = new WWWForm();
		form.AddField("data", data);
		var obs = ObservableWWW.PostWWW(URL + "/debug", form);
		obs.Subscribe(x => { }, e => Debug.Log(e)); 
		return obs; 
	}
}
