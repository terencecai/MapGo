using UnityEngine;
using MapzenGo.Helpers;
using MapzenGo.Models;
using UniRx;
using System;
using System.Collections.Generic;

public class TavernManager : MonoBehaviour {

	[SerializeField] public GameObject Tavern;

	private List<GameObject> taverns = new List<GameObject>();

	public Tavern activeTavern;
	private SocketManager socketManager;

	void Start () {
		// socketManager = GetComponent<SocketManager> ();
		// InvokeRepeating ("TavernLongPolling", 1, 1);
		
	}

	public void RequestTaverns(LocationInfo location) 
	{
		RestClient.getTaverns(PlayerPrefs.GetString("token", ""), location.latitude, location.longitude)
			.Subscribe(
				onSuccess,
				onError
			);
	}

	private void onSuccess(WWW response)
	{
		Debug.Log("response is : " + response.text);
		var data = JsonUtility.FromJson<DataPacket>(response.text);
		if (data != null) {
			createTaverns(data.nearest_taverns);
		} else {
			Debug.Log("Data is null");
		}
	}

	private void onError(Exception e)
	{
		Debug.Log(e.ToString());
		RestClient.sendDebug(e.ToString());
	}

	void TavernLongPolling() {
		var data = socketManager.lastDataPacket;
		RestClient.sendDebug("Diagram recieved: " + JsonUtility.ToJson(data));
		RestClient.sendDebug("Packet: " + socketManager.jsonPacket);
		if (socketManager.recievingError != "") {
			RestClient.sendDebug(socketManager.recievingError);
			socketManager.recievingError = "";
		}

		if (data == null || data.nearest_taverns.Count < 1) {
			return;
		}

		createTaverns(data.nearest_taverns);
	}

	private void createTaverns(List<Tavern> nearest_taverns)
	{
		if (nearest_taverns == null) {
			Debug.Log("nearest_taverns is null");
			return;
		}

		if (Tavern == null || Tavern.transform == null) {
			return;
		}

		if (taverns == null) {
			taverns = new List<GameObject>();
		}

		for (int i = 0; i < nearest_taverns.Count; i++)
		{
			if (taverns.Count <= i) {
				taverns.Add(Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation));
			}

			var obj = getTavernOrNull(i);
			if (obj == null) {
				obj = Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation);
				taverns.Add(obj);
			}
			showTavern(nearest_taverns[i], obj);
		}
	}

	private GameObject getTavernOrNull(int index)
	{
		try {
			return taverns[index];
		} catch (Exception) {
			return null;
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

		if (!shown) {
			sendDebugIfNotShown(tavern);
		}		
	}

	private void sendDebugIfNotShown(Tavern tavern) {
		var d = "Tavern not shown.\nCoordinates:\n" + tavern.latitude + "\n" + tavern.longitude;
		d += "\nUser coordinates are:";
		d += "\n" + Input.location.lastData.latitude;
		d += "\n" + Input.location.lastData.longitude;
		RestClient.sendDebug(d);
	}

	public void hideTavern(GameObject t) {
		t.SetActive(false);
	}
}
