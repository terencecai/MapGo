using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBoxController : MonoBehaviour
{
    [SerializeField] public List<Button> Buttons;
    [SerializeField] public GameObject SkillboxWindow;

    private readonly List<int> Skillboxes = new List<int>() {
        1,
        3,
        5,
        2,
        4,
        6
    };

    /**
4 : Empathy
5 : Creativity
6 : Communication
1 : Leadership1
2 : Wisdom5
3 : Spiritualityн
*/

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
                var sk = allSkills.FindAll(s => s.skillboxId == Skillboxes[index]);
                if (sk.Count <= 0) return;

                var temp = SkillboxWindow.GetComponent<SkillboxBehaviour>();
                temp.Skills = sk;
                temp.Title = sk[0].skillboxName;
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
        }
        allSkills = ProfileRepository.Instance.LoadProfile().allSkills;
        for (int i = 0; i < Buttons.Count; i++)
        {
            setOnClick(i);
        }
    }

}
