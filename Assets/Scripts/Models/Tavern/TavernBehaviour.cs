using UnityEngine;

public class TavernBehaviour : MonoBehaviour {

	[SerializeField] public Canvas TavernDialog;
	
	public Tavern tavern;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		if (checkObject()) 
			return;
		showTavernDialog();
		var distance = Vector3.Distance(transform.position, GameObject.Find("ThirdPersonController").transform.position);
		Debug.Log(distance);
	}

	private bool checkObject() 
	{
		#if UNITY_EDITOR
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
		#else
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
		#endif
	}

	private void showTavernDialog() {
		if (TavernDialog.gameObject.activeSelf) 
			return;

		TavernDialog.GetComponent<TavernDialogManager>().TavernName = getActiveTavernName();
		TavernDialog.gameObject.SetActive(true);
	}

	private string getActiveTavernName() {
		return tavern.name;
	}
}
