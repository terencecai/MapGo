using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

public class DepotSkillController : MonoBehaviour
{

    private static Dictionary<int, int> _spitesAndIds = new Dictionary<int, int>()
    {
        {6, 0},
        {5, 1},
        {4, 2},
        {1, 3},
        {3, 4},
        {2, 5}
    };

	[SerializeField] public Button Back;

    [SerializeField] public DepotSkill Panel1;
    [SerializeField] public DepotSkill Panel2;

    [SerializeField] public List<Sprite> SkillboxPictures;
    [SerializeField] public List<Sprite> ValuePictures;

    [SerializeField] public Image ValuePicture;
    [SerializeField] public Text DepotName;
    [SerializeField] public Text DepotText;
    [SerializeField] public GameObject TextPanel;

    public Depot Depot;

	void OnDisable() 
	{
		Depot = null;
		showJoystick();
        DepotText.text = "";
        TextPanel.SetActive(false);
	}

    void Start()
    {
		Back.onClick.AddListener(() =>
		{
			gameObject.SetActive(false);
            return;
		});
	}

	private void showJoystick()
	{
		var j = GameObject.Find("MobileJoystick");
		if (j == null) return;
		j.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		Debug.Log("Show joystick");
	}

	private void hideJoystick()
	{

		var j = GameObject.Find("MobileJoystick");
		if (j == null) return;
		j.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
	}

	void OnEnable()
	{
		hideJoystick();
		if (Depot == null)
        {
			Debug.Log("Depot is null");
            gameObject.SetActive(false);
            return;
        }

        DepotName.text = Depot.name;
        if (Depot.text == null || Depot.text == "")
        {
            TextPanel.SetActive(false);
        } else
        {
            TextPanel.SetActive(true);
            DepotText.text = Depot.text;
        }

        var profile = ProfileRepository.Instance.LoadProfile();
        var value = Depot.value;
        if (value == null)
        {
			Debug.Log("Depot value is null");
            gameObject.SetActive(false);
            return;
        }
        Sprite valImage = null;
        switch (value.name)
        {
            case "Authority": valImage = ValuePictures[0]; break;
            case "Compassion": valImage = ValuePictures[1]; break;
            case "Intelligence": valImage = ValuePictures[2]; break;
            default: break;
        }

        if (valImage == null)
        {
			Debug.Log("val image is null");
            gameObject.SetActive(false);
            return;
        }

        ValuePicture.sprite = valImage;

        var skills = Depot.skills;

		if (skills.Count != 2) 
		{
			Debug.Log("skills != 2 is null");
			gameObject.SetActive(false);
            return;
		}

		if (!_spitesAndIds.ContainsKey(skills[0].skillboxId) ||
			!_spitesAndIds.ContainsKey(skills[1].skillboxId))
		{
			Debug.Log("no image skillbox id is null");
			gameObject.SetActive(false);
            return;
		}
		var sprite1 = SkillboxPictures[_spitesAndIds[skills[0].skillboxId]];
		var sprite2 = SkillboxPictures[_spitesAndIds[skills[1].skillboxId]];
		Panel1.SetSkill(skills[0], sprite1);
		Panel2.SetSkill(skills[1], sprite2);
    }
	
}
/**
4 : Empathy
5 : Creativity
6 : Communication
1 : Leadership1
2 : Wisdom5
3 : Spiritualityн
*/
