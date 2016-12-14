using UnityEngine;
using System.Collections;
using MapzenGo.Helpers;
using System;
using MapzenGo.Models;

public class LocationManager : MonoBehaviour {

    private LocationInfo location = new LocationInfo();   
    private LocationInfo prevLocation;
    private LocationInfo startLocation;
    private Vector3 direction;

    private float nextActionTime = 0.0f;
    public float period = 1f;

	private UIManager uiManager;
    private CachedDynamicTileManager tileManager;

    private SocketManager socketManager;

    public Vector3 getDirection() {
        return direction;
    }

	private bool updatesStarted = false;
	private bool coroutineStarted = false;

    IEnumerator Start() {
		socketManager = GetComponent<SocketManager>();
        if (coroutineStarted) {
			yield break;
		}
        

        if(uiManager == null) {
            uiManager = GetComponent<UIManager>();
            //tileManager = GetComponent<CachedDynamicTileManager>();
        }

        // First, check if user has location service enabled
		while (!Input.location.isEnabledByUser) {
			uiManager.enableWarning ("GPS is not enabled!");
			yield return new WaitForSeconds (1);
        }

        // Start service before querying location
		Input.location.Start();
		uiManager.disableWarning ();

        // Wait until service initializes
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if(maxWait < 1) {
			uiManager.enableWarning ("GPS is not enabled!");
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if(Input.location.status == LocationServiceStatus.Failed) {
            print("Unable to determine device location");
			uiManager.enableWarning ("GPS is not enabled!");
            yield break;
        } else {
            startLocation = Input.location.lastData;
            gameObject.AddComponent<CachedDynamicTileManager>();
        }

		coroutineStarted = false;

		if (!updatesStarted) {
			InvokeRepeating ("updateLocation", 1, 1);
			updatesStarted = true;
		}
    }

    public LocationInfo getStartLocation() {
        return startLocation;
    }

	void updateLocation() {
		if (!Input.location.isEnabledByUser) {
			StartCoroutine (Start());
			coroutineStarted = true;
			return;
		}

		prevLocation = location;
		location = Input.location.lastData;

		if(prevLocation.latitude != location.latitude) {
			direction = GetPosition(location);
		}
		if(Time.time > nextActionTime) {
			nextActionTime += period;
			socketManager.SendLocation(location);
		}
	}

    void Update() {

    }

    private Vector3 GetPosition(LocationInfo location) {
        Vector3 targetPos = new Vector3();
        var meters = GM.LatLonToMeters(location.latitude, location.longitude);
        if(GameObject.Find("Tiles") != null) {
            foreach(Transform child in GameObject.Find("Tiles").transform) {
                Tile tile = child.GetComponent<Tile>();
                uiManager.enableWarning("" + location.latitude + "--" + location.longitude);
                if(tile.Rect.Contains(meters)) {
                    var target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    target.transform.position = (meters - tile.Rect.Center).ToVector3();
                    target.transform.localScale = Vector3.zero;
                    target.transform.SetParent(tile.transform, false);
                    targetPos = target.transform.position;
                    Destroy(target.gameObject);
                    break;
                }

            }
        }

        return targetPos;

    }

    
}