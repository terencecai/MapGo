using UnityEngine;
using UniRx;

public class RestClient : MonoBehaviour {

	//public static string URL = "http://ec2-52-57-237-162.eu-central-1.compute.amazonaws.com:9999";
	 public static string URL = "http://192.168.10.170:8888";

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
		form.AddField("nickName", user.name);
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
}
