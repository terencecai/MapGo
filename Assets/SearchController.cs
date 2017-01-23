using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using MapzenGo.Models;
using UniRx;
public class SearchController : MonoBehaviour
{

    [SerializeField] public GameObject ResultPanel;
    [SerializeField] public List<Text> Texts;

    private InputField field;
    void Start()
    {
        field = GetComponent<InputField>();
    }

    private void setListener()
    {
        if (field == null) {
            field = GetComponent<InputField>();
        }
        field.onValueChanged.AddListener((s) =>
        {
            if (s.Length < 3) return;
            s = s.Replace(",", "%2C");
            s = s.Replace(" ", "%20");
            RestClient.findAddress(s)
                .Subscribe(parseResults,
                err =>
                {
                    Debug.Log(err);
                    ResultPanel.SetActive(false);
                });
        });
    }

    void OnEnable()
    {
        setListener();
    }

    void OnDisable()
    {
        field.text = "";
        field.onValueChanged.RemoveAllListeners();
        Texts.ForEach(x => x.text = "");
    }

    public void DisableResults()
    {
        ResultPanel.SetActive(false);
    }

    private void parseResults(POI.SearchResponse result)
    {
        if (result.results.Count < 1)
        {
            Debug.Log("Results less 1");
            ResultPanel.SetActive(false);
            return;
        }

        ResultPanel.SetActive(true);
        var resultArray = result.results.GetRange(0, Math.Min(result.results.Count, 5));
        for (int i = 0; i < resultArray.Count; i++)
        {
            try
            {
                Texts[i].text = resultArray[i].display_name;
                addOnClick(Texts[i].gameObject, resultArray[i]);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                ResultPanel.SetActive(false);
            }

        }

    }

    private void addOnClick(GameObject go, POI.RootObject resp)
    {
        go.GetComponent<Button>().onClick.RemoveAllListeners();
        go.GetComponent<Button>().onClick.AddListener(() =>
        {
            LiveParams.TeleportEnabled = true;
            LiveParams.ComingToRealLocation = true;
            var tileManager = GameObject.Find("World").GetComponent<CachedDynamicTileManager>();
            tileManager.ClearAllTiles();
            tileManager.InitMap(new Location(resp.lat, resp.lon));
            field.text = resp.display_name;
            ResultPanel.SetActive(false);
        });
    }
}
