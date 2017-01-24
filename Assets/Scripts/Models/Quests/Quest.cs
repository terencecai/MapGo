using System;
using System.Collections.Generic;

[Serializable]
public class Quest {

    public string id;
    public bool close;
    public string closeAt;
    public string createdAt;
    public Creator creator;
    public string description;
    public int minAge;
    public Value value;
    public string title;
    public Answer winner;
    public bool pinnedByMe;
    public List<QuestSkill> skills;

    [Serializable]
    public class Creator {
        public string imgUrl;
        public string nickName;
    }
}