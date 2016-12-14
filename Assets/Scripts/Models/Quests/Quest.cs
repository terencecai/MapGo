using System;

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

    [Serializable]
    public class Creator {
        public string imgUrl;
        public string nickName;
    }
}