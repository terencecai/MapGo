using UnityEngine;
using System.Collections;
using MapzenGo.Helpers;
using MapzenGo.Models;

public class CustomObjectPlugin : MonoBehaviour {

    private Vector3 GetPosition(LocationInfo location) {
        Vector3 targetPos = new Vector3();
        var meters = GM.LatLonToMeters(location.latitude, location.longitude);
        foreach(Transform child in transform.GetChild(2).transform) {
            Tile tile = child.GetComponent<Tile>();
            if(tile.Rect.Contains(meters)) {
                var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                target.transform.position = (meters - tile.Rect.Center).ToVector3();
                target.transform.localScale = Vector3.one;
                target.transform.SetParent(tile.transform, false);
                targetPos = target.transform.InverseTransformPoint(target.transform.position);
                
            } 

        }

        return targetPos;

    }
}
