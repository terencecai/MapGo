using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class AnswersItemManager : MonoBehaviour
{

    [SerializeField] public Text Name;
    [SerializeField] public Text Date;
    [SerializeField] public Text Message;
    [SerializeField] public Button AcceptBtn;

    private Answer _answer;
	private Quest _quest;

	private Action<string> callback;

    public void bind(Answer item, Quest quest, Action<string> callback, bool winned)
    {
        _answer = item;
		_quest = quest;
        Name.text = item.profile.nickName;
        Message.text = item.text;
        var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        date = date.AddSeconds(item.createdAt / 1000);
        Date.text = date.ToString("dd MMM HH:mm");
		var name = ProfileRepository.Instance.LoadProfile().nickName;
        var questWinned = (_quest.winner != null && _quest.winner.id != 0) || winned;
        if (item.profile.nickName == name ||questWinned || _quest.creator.nickName != name)
        {
            AcceptBtn.gameObject.SetActive(false);
        }
        else
        {
            AcceptBtn.gameObject.SetActive(true);
        }

        AcceptBtn.onClick.AddListener(() =>
        {
			RestClient.approveAnswer(PlayerPrefs.GetString("token", ""), _quest.id, _answer.id.ToString())
				.Subscribe(
					x => callback(_quest.id),
					e => Debug.Log(e)
				);
        });
    }
}
