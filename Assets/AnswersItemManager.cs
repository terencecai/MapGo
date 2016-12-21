using UnityEngine;
using UnityEngine.UI;
using System;

public class AnswersItemManager : MonoBehaviour {

	[SerializeField] public Text Name;
	[SerializeField] public Text Date;
	[SerializeField] public Text Message;
	[SerializeField] public Button AcceptBtn;

	private Answer _answer;

	public void bind(Answer item) {
		_answer = item;
		Name.text = item.profile.nickName;
		Message.text = item.text;
		var date = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
		date = date.AddSeconds(item.createdAt / 1000);
		Date.text = date.ToString("dd MMM HH:mm");
		if (item.profile.nickName == ProfileRepository.Instance.LoadProfile().nickName) {
			AcceptBtn.gameObject.SetActive(false);
		} else {
			AcceptBtn.gameObject.SetActive(true);
		}
	}
}
