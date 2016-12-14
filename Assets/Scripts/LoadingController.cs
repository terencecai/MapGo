
using UnityEngine;

public class LoadingController : MonoBehaviour {

	[SerializeField] public GameObject LoadingIndicator;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showLoading() {
		if (LoadingIndicator != null)
			LoadingIndicator.SetActive(true);
	}

	public void hideLoading() {
		if (LoadingIndicator != null)
			LoadingIndicator.SetActive(false);
	}
}
