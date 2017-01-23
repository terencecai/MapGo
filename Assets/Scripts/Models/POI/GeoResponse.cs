using System;
using System.Collections.Generic;
using UnityEngine;

namespace POI
{   
    [Serializable]
    public class Address
    {
        public string house_number;
        public string road;
        public string city_district;
        public string city;
        public string county;
        public string region;
        public string state;
        public string postcode;
        public string country;
        public string country_code;
    }
    [Serializable]
    public class Extratags
    {
    }

    [Serializable]
    public class RootObject
    {
        public string place_id;
        public string licence;
        public string osm_type;
        public string osm_id;
        public string lat;
        public string lon;
        public string display_name;
        public Address address;
        public Extratags extratags;
        public List<string> boundingbox;

        public string GetPlaceName()
        {
            return display_name.Split(',')[0];
        }
    }

    [Serializable]
    public class SearchResponse
    {
        public List<RootObject> results = new List<RootObject>();

        public static SearchResponse FromJson(string json)
        {
            SearchResponse result;
            try { result = JsonUtility.FromJson<SearchResponse>("{\"results\": " + json + "}"); }
            catch (Exception) { result = new SearchResponse(); }

            return result;
    
        }
    }
}