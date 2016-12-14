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
	public Value[] values;

}
