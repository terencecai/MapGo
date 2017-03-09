using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DepotBehaviour : MonoBehaviour {

	[SerializeField] public GameObject DepotPanel;

	public Depot Depot;

	void Update()
	{
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
						performClick();
					}
				}
			}
		#endif
	}

	void OnMouseDown()
	{
		#if !UNITY_EDITOR
			return;
		#endif

		if (!checkObject()) 
		{
			performClick();
		}
		
	}

	private void performClick()
	{
		var skillController = DepotPanel.GetComponent<DepotSkillController>();
		skillController.Depot = Depot;
		DepotPanel.SetActive(true);
	}

	private bool checkObject() 
	{
		#if UNITY_EDITOR
			return EventSystem.current.IsPointerOverGameObject();
		#else
			return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
		#endif
	}
}
