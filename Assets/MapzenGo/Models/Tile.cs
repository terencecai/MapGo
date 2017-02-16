using System;
using MapzenGo.Helpers.VectorD;
using MapzenGo.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using POI;

using UniRx;
namespace MapzenGo.Models
{
    public class Tile : MonoBehaviour
    {
        public delegate void DestroyedEventHandler(Tile sender, EventArgs e);
        public event DestroyedEventHandler Destroyed;

        public JSONObject Data { get; set; }

        [SerializeField]
        public RectD Rect;
        public int Zoom { get; set; }
        public Vector2d TileTms { get; set; }
        public Vector3d TileCenter { get; set; }
        public bool UseLayers { get; set; }
        public Material Material { get; set; }

        public void OnDestroy()
        {
            if (Destroyed != null)
            {
                Destroyed(this, null);
            }
        }
    }

    public class BuildingClickHanlder : MonoBehaviour
    {
        public Tile tile;
        public Vector2 lastMousePos;
        public IDisposable lastSubscription;

        void OnMouseUp()
        {
            #if !UNITY_EDITOR
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
                if (Input.touchCount > 0) {
                    lastMousePos = Input.GetTouch(0).position;
                }
            #else
                lastMousePos = Input.mousePosition;
            #endif
            if (!LiveParams.NavigationEnabled)
                return;
            RaycastHit hit;
            if (lastMousePos == null) return;

            Ray ray = Camera.main.ScreenPointToRay(lastMousePos);

            if (Physics.Raycast(ray, out hit))
            {
                if (lastSubscription != null) lastSubscription.Dispose();
                var p = new Vector2d(hit.point.x, hit.point.z) + tile.Rect.Center;
                p = GM.MetersToLatLon(p);
                lastSubscription = RestClient.findPlace(p.x, p.y)
                    .Subscribe(
                        x => onS(x),
                        e => Debug.Log(e)
                    );
            }
        }

        private void onS(RootObject obj)
        {
            string placeName;
            if (obj == null) return;
            try { placeName = obj.GetPlaceName(); } 
            catch (Exception e) 
            { 
                placeName = "Failed to fetch location name";
                RestClient.sendDebug(e.ToString());
            }
            GameObject.Find("World").GetComponent<UIManager>().enableWarning(placeName);
        }
    }
}
