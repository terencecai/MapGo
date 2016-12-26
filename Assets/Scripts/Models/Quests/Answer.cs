using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Answer {
    public int id;
    public Profile profile;
    public String questDescription;
    public String text;
    public List<Skill> skills;
    public bool winner;
    public long createdAt;
}