using UnityEngine;
using UnityEngine.UI;
public class MenuController : MonoBehaviour {

	[SerializeField] public Button ProfileButton;
	[SerializeField] public GameObject ProfilePanel;
	void Start () {
		ProfileButton.onClick.AddListener(() => ProfilePanel.SetActive(true));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
