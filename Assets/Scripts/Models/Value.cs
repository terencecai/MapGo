using System;


/*
    "valueId": 1,
    "name": "Authority",
    "level": 3,
    "maxLevel": 100,
    "chosen": true,
    "current": true
*/
[Serializable]
public class Value {
    public string valueId;
    public string name = "";
    public int level;
    public int maxLevel;
    public bool chosen;
    public bool current;

    // override object.Equals
    public override bool Equals (object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        return this.valueId == (obj as Value).valueId;
    }

}