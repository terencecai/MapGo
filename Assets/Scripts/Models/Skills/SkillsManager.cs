using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MapzenGo.Helpers;
using System;

public class SkillsManager : MonoBehaviour
{
    private static int MAX_SKILL_POOL_SIZE = 2;
    [SerializeField] public GameObject skillPrefab;
    [SerializeField] public GameObject SkillDialog;

    private List<Skill> activeSkills = new List<Skill>();
    private List<GameObject> skillsPool = new List<GameObject>(MAX_SKILL_POOL_SIZE);
    private GameObject player;

    private UIManager UIManager;

    void Start()
    {
        UIManager = GetComponent<UIManager>();
        player = GameObject.Find("ThirdPersonController");

        if (skillsPool.Count > 0) skillsPool.Clear();
        for (int i = 0; i < MAX_SKILL_POOL_SIZE; i++)
        {
            skillsPool.Add(Instantiate(skillPrefab, Vector3.zero, transform.rotation) as GameObject);
            skillsPool[i].SetActive(false);
        }

        StartCoroutine(waitForCoords());
        #if UNITY_EDITOR
         UpdateSkills();   
        #endif
    }

    private void clearPool()
    {
        activeSkills = new List<Skill>();
        foreach (var p in skillsPool)
        {
            if (p != null)
                p.SetActive(false);
        }
    }

    private IEnumerator waitForCoords()
    {
        while (!(Input.location.lastData.latitude > 0))
        {
            yield return new WaitForSeconds(1);
        }

        UpdateSkills();
        InvokeRepeating("skillsUpdates", 10, 5);
        yield break;
    }

    public void UpdateSkills()
    {
        RestClient.requestSkills(PlayerPrefs.GetString("token", ""))
            .Subscribe(
                success => { calculateSkills(success.text); },
                failure => { Debug.Log(failure); }
            );
    }

    private void calculateSkills(string skillsJson)
    {
        var lastLatitude = PlayerPrefs.GetString("last_lat", "");
        var lastLongitude = PlayerPrefs.GetString("last_lon", "");

        if (lastLatitude == "" || lastLongitude == "")
        {
            saveToCacheAndThrow(skillsJson);
        }
        else
        {
            checkLastDistanceAndThrow(skillsJson);
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("last_lat", "");
        PlayerPrefs.SetString("last_lon", "");
    }

    private void skillsUpdates()
    {
        var dist = lastDistance();
        if (dist > 50)
        {
            RestClient.requestSkills(PlayerPrefs.GetString("token", ""))
            .Subscribe(
                success => { saveToCacheAndThrow(success.text); },
                failure => { Debug.Log(failure); }
            );
        }
    }

    private void saveToCacheAndThrow(string skillsJson)
    {
        saveLastLocation();
        SkillCache.Instance.Save(skillsJson);

        throwSkills(SkillCache.Instance.GetCache());
    }

    private void saveLastLocation()
    {
        PlayerPrefs.SetString("last_lat", Input.location.lastData.latitude.ToString());
        PlayerPrefs.SetString("last_lon", Input.location.lastData.longitude.ToString());
    }

    private void checkLastDistanceAndThrow(string skillsJson)
    {
        var dist = lastDistance();
        var cache = SkillCache.Instance.GetCache();

        if (dist > 25 || cache.skills.Count <= 0)
        {
            saveToCacheAndThrow(skillsJson);
        }
        else
        {
            throwSkills(cache);
        }
    }

    private double lastDistance()
    {
        Double savedLat = -1;
        Double savedLon = -1;
        double.TryParse(PlayerPrefs.GetString("last_lat", ""), out savedLat);
        double.TryParse(PlayerPrefs.GetString("last_lon", ""), out savedLon);

        if (savedLat == -1 || savedLon == -1)
        {
            return -1;
        }

        var lat = Input.location.lastData.latitude;
        var lon = Input.location.lastData.longitude;
        var dist = GM.distFrom(savedLat, savedLon, lat, lon);
        return dist;
    }

    private void throwSkills(SkillCacheContainer skills)
    {
        clearPool();
        if (skills.skills == null || skills.skills.Count <= 0)
            return;
        var lst = skills.skills.GetRange(0, Mathf.Min(MAX_SKILL_POOL_SIZE, skills.skills.Count));
        if (player == null)
        {
            player = GameObject.Find("ThirdPersonController");
        }
        var playerPosition = player.transform.position;

        for (int i = 0; i < lst.Count; i++)
        {
            try
            {
                var inside = UnityEngine.Random.insideUnitCircle * 60;
                var newPos = new Vector3(playerPosition.x + inside.x, 3, playerPosition.z + inside.y);

                var skill = lst[i];
                activeSkills.Add(skill);
                skillsPool[i].transform.position = newPos;
                skillsPool[i].SetActive(true);
                skillsPool[i].GetComponent<SkillBehaviour>().Skill = skill;
                skillsPool[i].GetComponent<SkillBehaviour>().skillDialog = SkillDialog;
                var mat = skillsPool[i].GetComponent<MeshRenderer>().material;
                switch (skill.valueName)
                {
                    case "Authority":
                        mat.color = new Color(199 / 255f, 226 / 255f, 73 / 255f);
                        break;
                    case "Intelligence":
                        mat.color = Color.magenta;
                        break;
                    case "Compassion":
                        mat.color = new Color(242 / 255f, 133 / 255f, 0);
                        break;
                }
            }
            catch (Exception e)
            {
                RestClient.sendDebug(e.ToString());
                continue;
            }
        }
    }
}
