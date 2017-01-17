using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBoxController : MonoBehaviour
{
    [SerializeField] public List<Button> Buttons;
    [SerializeField] public GameObject SkillboxWindow;

    private readonly List<string> Skillboxes = new List<string>() {
        "Leadership",
        "Spirituality",
        "Creativity",
        "Wisdom",
        "Empathy",
        "Communication"
    };

    private List<Skill> allSkills;
    void Start()
    {
        allSkills = ProfileRepository.Instance.LoadProfile().allSkills;
        StartCoroutine(loadSkills());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setOnClick(int index)
    {

        Buttons[index].onClick.AddListener(() =>
        {
            try
            {
                var sk = allSkills.FindAll(s => s.skillboxName == Skillboxes[index]);
                var temp = SkillboxWindow.GetComponent<SkillboxBehaviour>();
                temp.Skills = sk;
                temp.Title = Skillboxes[index];
                SkillboxWindow.SetActive(true);
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(e);
            }
        });
    }

    System.Collections.IEnumerator loadSkills()
    {
        while (ProfileRepository.Instance.LoadProfile().allSkills.Count <= 0) {
            yield return new WaitForSeconds(0.5f);
            print("WaitAndPrint " + Time.time);
        }
        allSkills = ProfileRepository.Instance.LoadProfile().allSkills;
        for (int i = 0; i < Buttons.Count; i++)
        {
            setOnClick(i);
        }
    }

}
