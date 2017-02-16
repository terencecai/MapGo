using UnityEngine;
using MapzenGo.Models;
public class InitController : MonoBehaviour {

	[SerializeField] public GameObject ChoosePanel;

	// Use this for initialization
	void Start () {
		ChoosePanel.SetActive(currentValueIsNull());
		#if UNITY_EDITOR
			gameObject.AddComponent<CachedDynamicTileManager>();
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	bool currentValueIsNull() {
		var p = ProfileRepository.Instance.LoadProfile();
		if (p == null) return true;
		
		var cv = p.chosenValue;
		return (cv == null || cv.valueId == null || cv.valueId == "");
	}
}
