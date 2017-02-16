using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapzenGo.Models;

public class ZoomCam : MonoBehaviour
{

    [SerializeField] public Camera cam;

	private POI.RootObject obj;

    void Start()
    {
		cam.GetComponent<Cam>().OnZoomStarted = FirstCallback;
		cam.GetComponent<Cam>().OnZoomEnded = SecondCallback;
    }

    public void StartAnim(POI.RootObject obj)
    {
		this.obj = obj;
        LiveParams.TeleportEnabled = true;
        LiveParams.SetComing(true);
		cam.gameObject.SetActive(true);
    }

    void FirstCallback()
    {
		var tileManager = GetComponent<CachedDynamicTileManager>();
        tileManager.ClearAllTiles();
        tileManager.InitMap(new Location(obj.lat, obj.lon));
    }

    void SecondCallback()
    {
		cam.gameObject.SetActive(false);
    }
}
