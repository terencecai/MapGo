using UnityEngine;
using System.Collections;

public class StartScreenBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnAnimationFinished(int activate) {
		if (activate == 0) { 
			gameObject.SetActive(false);
		} else {
			gameObject.SetActive(true);
		}
	}
}
