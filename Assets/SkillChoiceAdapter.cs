using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillChoiceAdapter : MonoBehaviour
{
    private static readonly int MAX_CHOSED = 2;
    [SerializeField] public List<Toggle> boxes;
    [SerializeField] public Text Title;
    [SerializeField] public Button ok;
	[SerializeField] public GameObject ErrorWindow;
    public List<Skill> skills = new List<Skill>();

    private List<Toggle> activeBoxes = new List<Toggle>();
    private Dictionary<Toggle, Skill> rels = new Dictionary<Toggle, Skill>();

    public Action<List<Skill>> callback;

    void Start()
    {
        activeBoxes = new List<Toggle>();
        boxes.ForEach(b => setListeners(b));
        // initSkills();
        ok.onClick.AddListener(() =>
        {
			List<Skill> chosen = new List<Skill>();
			activeBoxes.ForEach(x => {
				chosen.Add(rels[x]);
			});
			if (chosen.Count != 2) {
				showError("Please, choose 2 skills.");
				return;
			}
			callback(chosen);
            gameObject.SetActive(false);
        });
    }

    void OnEnable()
    {
        initSkills();
    }

    void OnDisable()
    {
        rels = new Dictionary<Toggle, Skill>();
        activeBoxes = new List<Toggle>();

        boxes.ForEach(b => b.isOn = false);
    }

    private void setListeners(Toggle t)
    {
        t.onValueChanged.AddListener(value =>
        {
            if (!value)
            {
                if (activeBoxes.Contains(t)) activeBoxes.Remove(t);
            }
            else
            {
                if (activeBoxes.Count >= MAX_CHOSED)
                {
                    t.isOn = false;
                    return;
                }
                activeBoxes.Add(t);
            }
        });
    }

    private void initSkills()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            rels.Add(boxes[i], skills[i]);
            boxes[i].transform.Find("Label").GetComponent<Text>().text = skills[i].skillName;
        }
    }
	private void showError(string message)
    {
        var p = ErrorWindow.GetComponent<PopUp>();
        p.Title = "Error";
        p.Message = message;
        ErrorWindow.SetActive(true);
    }
}
