using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	[SerializeField] public Button ProfileButton;
	void Start () {
		ProfileButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("ProfileScene"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
