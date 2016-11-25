using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Text gps;

    public void setGpsValues(string values) {
        gps.text = values;
    }    
}
