using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MapzenGo.Helpers;

public class SkillsManager : MonoBehaviour
{
    private static int MAX_SKILL_POOL_SIZE = 10;
    [SerializeField] public GameObject skillPrefab;
    [SerializeField] public GameObject SkillDialog;

    private List<Skill> activeSkills = new List<Skill>();
    private List<GameObject> skillsPool = new List<GameObject>(MAX_SKILL_POOL_SIZE);
    private GameObject player;
    void Start()
    {
        player = GameObject.Find("ThirdPersonController");

        if (skillsPool.Count > 0) skillsPool.Clear();
        for (int i = 0; i < MAX_SKILL_POOL_SIZE; i++)
        {
            skillsPool.Add(Instantiate(skillPrefab, Vector3.zero, transform.rotation) as GameObject);
            skillsPool[i].SetActive(false);
        }

        StartCoroutine(waitForCoords());
    }

    private void clearPool()
    {
        activeSkills = new List<Skill>();
        foreach (var p in skillsPool)
        {
            p.SetActive(false);
        }
    }

    private IEnumerator waitForCoords()
    {
        while(!(Input.location.lastData.latitude > 0)) {
            yield return new WaitForSeconds(1);
        }

        UpdateSkills();
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
            checkLastDistanceAndThrow(double.Parse(lastLatitude), double.Parse(lastLongitude), skillsJson);
        }
    }

    private void saveToCacheAndThrow(string skillsJson)
    {
        PlayerPrefs.SetString("last_lat", Input.location.lastData.latitude.ToString());
        PlayerPrefs.SetString("last_lon", Input.location.lastData.longitude.ToString());
        SkillCache.Instance.Save(skillsJson);

        throwSkills(SkillCache.Instance.GetCache());
    }

    private void checkLastDistanceAndThrow(double lastLat, double lastLon, string skillsJson)
    {
        var lat = Input.location.lastData.latitude;
        var lon = Input.location.lastData.longitude;
        var dist = GM.distFrom(lastLat, lastLon, lat, lon);
        var cache = SkillCache.Instance.GetCache();
        if (dist > 500 || cache.skills.Count <= 0)
        {
            saveToCacheAndThrow(skillsJson);
        } else
        {
            throwSkills(cache);
        }
    }

    private void throwSkills(SkillCacheContainer skills)
    {
        clearPool();
        var lst = skills.skills.GetRange(0, Mathf.Min(MAX_SKILL_POOL_SIZE, skills.skills.Count));
        var playerPosition = player.transform.position;

        for (int i = 0; i < lst.Count; i++)
        {
            var inside = Random.insideUnitCircle * 80;
            var newPos = new Vector3(playerPosition.x + inside.x, 2, playerPosition.z + inside.y);

            var skill = lst[i];
            activeSkills.Add(skill);
            skillsPool[i].transform.position = newPos;
            skillsPool[i].SetActive(true);
            skillsPool[i].GetComponent<SkillBehaviour>().Skill = skill;
            skillsPool[i].GetComponent<SkillBehaviour>().skillDialog = SkillDialog;
        }
    }
}
