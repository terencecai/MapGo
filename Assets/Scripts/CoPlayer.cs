using UnityEngine;
using MapzenGo.Helpers;
using MapzenGo.Models;

public class CoPlayer : MonoBehaviour {

    private string email;

    public void SetEmail(string email) {
        this.email = email;
    }

    public string GetEmail() {
        return email;
    }

    public void SetPosition(Location location) {        
        var meters = GM.LatLonToMeters(location.GetLatitude(), location.GetLongitude());
        if(GameObject.Find("Tiles") != null) {
            foreach(Transform child in GameObject.Find("Tiles").transform) {
                Tile tile = child.GetComponent<Tile>();                
                if(tile.Rect.Contains(meters)) {
                    transform.SetParent(null);
                    transform.position = (meters - tile.Rect.Center).ToVector3();
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    transform.SetParent(tile.transform, false);                    
                    break;
                }

            }
        }
    }	
}
