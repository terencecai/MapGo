using UnityEngine;
using MapzenGo.Helpers;
using MapzenGo.Models;

using System.Collections.Generic;
public class TavernManager : MonoBehaviour {

	[SerializeField] public GameObject Tavern;

	private List<GameObject> taverns = new List<GameObject>();

	public Tavern activeTavern;
	private SocketManager socketManager;

	void Start () {
		socketManager = GetComponent<SocketManager> ();
		InvokeRepeating ("TavernLongPolling", 1, 1);
	}

	void TavernLongPolling() {
		var data = socketManager.lastDataPacket;
		RestClient.sendDebug("Diagram recieved: " + JsonUtility.ToJson(data));
		if (data == null || data.nearest_taverns.Count < 1) {
			return;
		}

		for (int i = 0; i < data.nearest_taverns.Count; i++)
		{
			if (taverns.Count <= i) {
				taverns.Add(Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation));
			}

			var obj = taverns[i];
			if (obj == null) {
				obj = Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation);
			}
			showTavern(data.nearest_taverns[i], obj);
		}
	}

	public void showTavern(Tavern tavern, GameObject tavernGO) {
		bool shown = false;
		if (tavernGO.activeSelf) {
			if (tavernGO.GetComponent<TavernBehaviour>().tavern.name != tavern.name) {
				tavernGO.SetActive(false);
			} else {
				RestClient.sendDebug("Tavern " + tavern.name + " not shown due to activeSelf");
				return;
			}
		}

		Tile tile;
		var meters = GM.LatLonToMeters(tavern.latitude, tavern.longitude);
		if(GameObject.Find("Tiles") != null) {
			foreach(Transform child in GameObject.Find("Tiles").transform) {
				tile = child.GetComponent<Tile>();                
				if(tile.Rect.Contains(meters)) {
					tavernGO.SetActive (true);
					tavernGO.transform.SetParent(null);
					tavernGO.transform.position = (meters - tile.Rect.Center).ToVector3();
					tavernGO.transform.SetParent(tile.transform, false);
					tavernGO.GetComponent<TavernBehaviour>().tavern = tavern;
					RestClient.sendDebug("Tavern " + tavern.name + " created\n");  
					shown = true;                 
					break;
				}

			}
		}

		if (shown) return;
		
		var d = "Tavern not shown.\nCoordinates:\n" + tavern.latitude + "\n" + tavern.longitude;
		d += "\nTavern meters are: " + meters;
		d += "\nUser coordinates are:";
		d += "\n" + Input.location.lastData.latitude;
		d += "\n" + Input.location.lastData.longitude;
		RestClient.sendDebug(d);
	}

	public void hideTavern(GameObject t) {
		t.SetActive(false);
	}
}
