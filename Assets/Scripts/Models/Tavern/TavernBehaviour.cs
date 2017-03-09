using UnityEngine;
using UnityEngine.EventSystems;

public class TavernBehaviour : MonoBehaviour {

	[SerializeField] public Canvas TavernDialog;
	
	public Tavern tavern;
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IOS
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
			{
				if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
					return;
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				if (Physics.Raycast(ray, out hit))
				{
					if (hit.transform.gameObject == this.gameObject)
					{
						showTavernDialog();
					}
				}
			}
		#endif
	}

	void OnMouseDown() {
		#if !UNITY_EDITOR
			return;
		#endif
		
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
