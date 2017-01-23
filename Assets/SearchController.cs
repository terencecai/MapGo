using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using UniRx;
public class SearchController : MonoBehaviour
{

    [SerializeField] public GameObject ResultPanel;
    [SerializeField] public List<Text> Texts;

    private InputField field;
    void Start()
    {
        field = GetComponent<InputField>();
        field.onValueChanged.AsObservable()
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Where(x => x.Length > 3)
            .Select(str => str.Replace(",", "%2C"))
            .Select(str => str.Replace(" ", "%20"))
            .SelectMany(RestClient.findAddress)
            .Subscribe( 
                parseResults,
                err =>
                {
                    Debug.Log(err);
                    ResultPanel.SetActive(false);
                }
            );
    }

    void OnDisable()
    {
        field.text = "";
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
            }
			catch (Exception)
			{
				ResultPanel.SetActive(false);
			}

        }

    }
}
