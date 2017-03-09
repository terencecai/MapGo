using System;
using System.Collections.Generic;

[Serializable]
public class Depot
{
    public string name;
    public double latitude;
    public double longitude;
    public long valueId;
    public List<long> skillsId;

    public List<Skill> skills = new List<Skill>();
    public Value value;
    public string text = "";
}