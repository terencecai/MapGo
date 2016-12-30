using UnityEngine;
using UniRx;
using Hash = System.Collections.Generic.Dictionary<string, string>;
using System.Text;
using System;
using System.Linq;
public class RestClient : MonoBehaviour
{

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
        form.AddField("birthDay", user.getBirthdayTime().ToString());

        return ObservableWWW.PostWWW(URL + "/registration", form);
    }

    public static IObservable<WWW> updateProfile(string token, Profile user)
    {
        var s = "{";
        s += "\"nickName\": \"" + user.nickName + "\", ";
        s += "\"gender\": \"" + user.gender.ToUpper() + "\", ";
        s += "\"birthDay\": " + Convert.ToInt64(user.getBirthdayTime()).ToString() + ", ";
        s += "\"chosenValueId\": " + user.chosenValue.valueId + "}";

        var data = Encoding.UTF8.GetBytes(s);
        // var form = new Hash();
        // form.Add("nickName", user.nickName);
        // form.Add("gender", user.gender.ToUpper());
        // form.Add("birthDay", Convert.ToInt64(user.getBirthdayTime()).ToString());
        // form.Add("chosenValueId", user.chosenValue.valueId);

        return ObservableWWW.PostWWW(URL + "/profile", data, new Hash() { { "X-Auth-Token", token } ,
                                               { "Content-Type", "application/json" }});
    }

    public static IObservable<WWW> sendFBToken(string token)
    {
        WWWForm form = new WWWForm();
        form.AddField("accessToken", token);
        return ObservableWWW.PostWWW(URL + "/facebook_login", form);
    }

    public static IObservable<WWW> verifyEmail(Credentials creds, string code)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", creds.GetEmail());
        form.AddField("password", creds.GetPassword());
        form.AddField("code", code);

        return ObservableWWW.PostWWW(URL + "/verify_email", form);
    }

    public static IObservable<WWW> requestCode(Credentials creds)
    {
        return ObservableWWW.GetWWW(URL + "/send_verification_code?email=" + creds.GetEmail());
    }

    public static IObservable<WWW> requestPassword(string email)
    {
        return ObservableWWW.GetWWW(URL + "/reset_password?email=" + email);
    }

    public static IObservable<WWW> requestSkills(string token)
    {
        return ObservableWWW.GetWWW(URL + "/skills/generate_test", new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<WWW> pickupSkill(string token, string skillId)
    {
        WWWForm form = new WWWForm();
        form.AddField("skillId", skillId);
        return ObservableWWW.PostWWW(URL + "/profile/skill", form, new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<WWW> chooseValue(string token, int valueId)
    {
        var s = "{\"chosenValueId\":" + valueId + "}";
        Debug.Log(s);
        var b = Encoding.UTF8.GetBytes(s);

        return ObservableWWW.PostWWW(URL + "/profile", b,
                                    new Hash(){{"X-HTTP-Method-Override", "PUT"},
                                               { "X-Auth-Token", token },
                                               { "Content-Type", "application/json" }});
    }

    public static IObservable<string> getProfile(string token)
    {
        return ObservableWWW.Get(URL + "/profile", new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> getMySkills(string token)
    {
        return ObservableWWW.Get(URL + "/profile/skill", new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> getAllSkills(string token)
    {
        return ObservableWWW.Get(URL + "/skills", new Hash() { { "X-Auth-Token", token } });
    }

    //------------------------
    //QUESTS
    //------------------------
    public static IObservable<WWW> getQuests(string token, string type)
    {
        string str;
        if (type == null)
            str = "/quests";
        else
            str = "/quests/me/" + type;
        return ObservableWWW.GetWWW(URL + str + "?size=40", new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> createQuest(string token, Hashmap data)
    {
        var d = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data.generateDTO()));
        return ObservableWWW.Post(URL + "/quests", d, 
            new Hash() { { "X-Auth-Token", token }, { "Content-Type", "application/json" }});
    }

    public static IObservable<WWW> getAnswers(string token, string questId)
    {
        return ObservableWWW.GetWWW(URL + "/quests/" + questId + "/answers", new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<WWW> createAnswer(string token, string data, string questId)
    {
        var form = new WWWForm();
        form.AddField("text", data);
        return ObservableWWW.PostWWW(URL + "/quests/" + questId + "/answers", form, new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> approveAnswer(string token, string questId, string answerId)
    {
        return ObservableWWW.Post(URL + "/quests/" + questId + "/answers/" + answerId, Encoding.UTF8.GetBytes("123"), 
        new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> pinQuest(string token, string questId)
    {
        return ObservableWWW.Post(URL + "/quests/me/" + questId, Encoding.UTF8.GetBytes("123"), 
        new Hash() { { "X-Auth-Token", token } });
    }

    public static IObservable<string> unpinQuest(string token, string questId)
    {
        return ObservableWWW.Post(URL + "/quests/me/unpin/" + questId, Encoding.UTF8.GetBytes("123"), 
        new Hash() { { "X-Auth-Token", token } });
    }
    //------------------------
    //Taverns
    //------------------------
    public static IObservable<WWW> getTaverns(string token, double lat, double lon)
    {
        return ObservableWWW.GetWWW(URL + "/nearest_entities?latitude=" + lat + "&longitude=" + lon, new Hash() { { "X-Auth-Token", token } });
    }

    //------------------------
    //Debug
    //------------------------
    public static IObservable<WWW> sendDebug(string data)
    {
        var form = new WWWForm();
        form.AddField("data", data);
        var obs = ObservableWWW.PostWWW(URL + "/debug", form);
        obs.Subscribe(x => { }, e => Debug.Log(e));
        return obs;
    }
}
