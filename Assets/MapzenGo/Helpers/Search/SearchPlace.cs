﻿using System.Collections.Generic;
using MapzenGo.Models;
using UniRx;
using UnityEngine;

namespace MapzenGo.Helpers.Search
{
    [ExecuteInEditMode]
    [AddComponentMenu("Mapzen/SearchPlace")]
    public class SearchPlace : MonoBehaviour {

        const string seachUrl = "https://search.mapzen.com/v1/autocomplete?text=";
        public string namePlace;
        public string namePlaceСache;
        public List<StructSeachData> dataList;
    

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
        }
        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
        }

        public void SearchInMapzen()
        {
            if (namePlace!=string.Empty&&namePlaceСache != namePlace)
            {
                namePlaceСache = namePlace;
                ObservableWWW.Get(seachUrl + namePlace).Subscribe(
                    success =>
                    {
                        DataProcessing(success);
                    },
                    error =>
                    {
                        Debug.Log(error);
                    });
            }
        }

        public void SetupToTileManager(float Latitude, float Longitude)
        {
            TileManager tm = GetComponent<TileManager>();
            tm.setLat(Latitude);
            tm.setLon(Longitude);
        }

        public void DataProcessing(string success)
        {
            JSONObject obj = new JSONObject(success);
            dataList = new List<StructSeachData>();
            foreach (JSONObject jsonObject in obj["features"].list)
            {
                dataList.Add(new StructSeachData()
                {
                    coordinates = new Vector2(jsonObject["geometry"]["coordinates"][0].f, jsonObject["geometry"]["coordinates"][1].f),
                    label = jsonObject["properties"]["label"].str
                });
            }
        }
    }
}
