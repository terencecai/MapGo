using System;
using System.Collections.Generic;

public class Hashmap
{
    private Dictionary<string, string> _data = new Dictionary<string, string>();

    public void Add(string KEY, string VALUE)
    {
        if (_data.ContainsKey(KEY))
        {
            _data[KEY] = VALUE;
        } 
        else
        {
            _data.Add(KEY, VALUE);
        }
    }

    public string Get(string KEY)
    {
        if (_data.ContainsKey(KEY))
            return _data[KEY];
        else
            return null;
    }

    public QuestDTO generateDTO()
    {
        var item = new QuestDTO();
        item.title       = _data["title"];
        item.description = _data["description"];
        item.minAge      = Int32.Parse(_data["minAge"]);
        item.valueId     = Int32.Parse(_data["value_id"]);
        item.skillIds    = new int[] 
        {
            Int32.Parse(_data["skill1_id"]),
            Int32.Parse(_data["skill2_id"])
        };
        return item;
    }
}

[Serializable]
public class QuestDTO
{
    public string title;
    public string description;
    public int minAge;
    public int valueId;
    public int[] skillIds;
}