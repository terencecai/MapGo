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
		if (data == null || data.nearest_taverns.Count < 1)
			return;

		for (int i = 0; i < data.nearest_taverns.Count; i++)
		{
			if (taverns.Count <= i) {
				taverns.Add(Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation));
			}

			var obj = taverns[i];
			showTavern(data.nearest_taverns[i], obj);
		}
	}

	public void showTavern(Tavern tavern, GameObject tavernGO) {
		if (tavernGO.activeSelf)
			return;

		Location location = new Location (tavern.latitude, tavern.longitude);
		var meters = GM.LatLonToMeters(location.GetLatitude(), location.GetLongitude());
		if(GameObject.Find("Tiles") != null) {
			foreach(Transform child in GameObject.Find("Tiles").transform) {
				Tile tile = child.GetComponent<Tile>();                
				if(tile.Rect.Contains(meters)) {
					tavernGO.SetActive (true);
					tavernGO.transform.SetParent(null);
					tavernGO.transform.position = (meters - tile.Rect.Center).ToVector3();
					tavernGO.transform.SetParent(tile.transform, false);
					tavernGO.GetComponent<TavernBehaviour>().tavern = tavern;                    
					break;
				}

			}
		}
	}

	public void hideTavern(GameObject t) {
		t.SetActive(false);
	}
}
