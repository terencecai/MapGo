using UnityEngine;
using System.Collections.Generic;
using System;
/*
  "nickName": "Oleg",
  "birthDay": 664329600000,
  "age": 25,
  "avatarUrl": "",
  "gender": "MALE",
*/
[Serializable]
public class Profile
{
    public string nickName;
    public string birthDay;
    public string gender;
    public Credentials credentials;
    public Value chosenValue;
    public Value currentValue;
    public List<Value> values;
    public List<Skill> skills;

    public List<Skill> allSkills = new List<Skill>();

    public double getBirthdayTime()
    {
        try
        {
            var date = DateTime.ParseExact(birthDay, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return (TimeZoneInfo.ConvertTimeToUtc(date) -
               new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }catch (Exception)
        {
            var date = DateTime.ParseExact(birthDay, "dd.M.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return (TimeZoneInfo.ConvertTimeToUtc(date) -
               new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }

    public string getBirthdayString()
    {
        try
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Double.Parse(birthDay)).ToLocalTime();
            return dtDateTime.ToString("dd.MM.yyyy");
        }
        catch (FormatException)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Double.Parse(birthDay)).ToLocalTime();
            return dtDateTime.ToString("dd.M.yyyy");
        }
        catch (Exception)
        {
            return "";
        }
    }

}
