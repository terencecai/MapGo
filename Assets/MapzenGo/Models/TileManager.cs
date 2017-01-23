using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Helpers;
using MapzenGo.Models.Factories;
using MapzenGo.Models.Plugins;
using UniRx;
using UnityEngine;
using MapzenGo.Helpers.VectorD;

namespace MapzenGo.Models {
    public class TileManager : MonoBehaviour {
        
        
        [SerializeField] public float Latitude = 40.752710f;

        [SerializeField] public float Longitude = 39.979307f;
        [SerializeField]
        public int Range = 3;
        [SerializeField]
        public int Zoom = 18;
        [SerializeField]
        public float TileSize = 200;

        protected readonly string _mapzenUrl = "http://tile.mapzen.com/mapzen/vector/v1/{0}/{1}/{2}/{3}.{4}?api_key={5}";
        [SerializeField]
        protected string _key = "mapzen-C4E6Siz"; //try getting your own key if this doesn't work
        protected readonly string _mapzenLayers = "roads,buildings,pois";
        [SerializeField]
        protected Material MapMaterial;
        protected readonly string _mapzenFormat = "json";
        protected Transform TileHost;

        private List<Plugin> _plugins;

        protected Dictionary<Vector2d, Tile> Tiles; //will use this later on
        protected Vector2d CenterTms; //tms tile coordinate
        protected Vector2d CenterInMercator; //this is like distance (meters) in mercator

        private LocationManager locManager;

        public virtual void Start() {
            LiveParams.InitFields();
            locManager = GetComponent<LocationManager>();

            if(locManager.getStartLocation().latitude != 0) {
                Latitude = locManager.getStartLocation().latitude;
                Longitude = locManager.getStartLocation().longitude;
            }

            if(MapMaterial == null)
                MapMaterial = Resources.Load<Material>("Ground");

            InitFactories();

            
            InitMap();
        }

        public virtual void InitMap() 
        {
            var location = locManager.GetLastLocation();
            if (location == null) return;
            Latitude = (float) location.GetLatitude();
            Longitude = (float) location.GetLongitude();
            var v2 = GM.LatLonToMeters(Latitude, Longitude);
            var tile = GM.MetersToTile(v2, Zoom);
            TileHost = new GameObject("Tiles").transform;
            TileHost.SetParent(transform, false);

            Tiles = new Dictionary<Vector2d, Tile>();
            CenterTms = tile;
            CenterInMercator = GM.TileBounds(CenterTms, Zoom).Center;

            var rect = GM.TileBounds(CenterTms, Zoom);
            transform.localScale = Vector3.one * (float) (TileSize / rect.Width);

            LoadTiles(CenterTms, CenterInMercator);
        }

        public virtual void InitMap(Location loc)
        {
            Latitude = (float) loc.GetLatitude();
            Longitude = (float) loc.GetLongitude();
            var v2 = GM.LatLonToMeters(Latitude, Longitude);
            var tile = GM.MetersToTile(v2, Zoom);
            TileHost = new GameObject("Tiles").transform;
            TileHost.SetParent(transform, false);

            Tiles = new Dictionary<Vector2d, Tile>();
            CenterTms = tile;
            CenterInMercator = GM.TileBounds(CenterTms, Zoom).Center;

            var rect = GM.TileBounds(CenterTms, Zoom);
            transform.localScale = Vector3.one * (float) (TileSize / rect.Width);

            LoadTiles(CenterTms, CenterInMercator);
        }

        private void InitFactories() {
            _plugins = new List<Plugin>();
            foreach(var plugin in GetComponentsInChildren<Plugin>()) {
                _plugins.Add(plugin);
            }
        }       

        public void setLat(float lat) {
           Latitude = lat;
        }

        public void setLon(float lon) {
            Longitude = lon;
        }

        public void LoadTiles(Vector2d tms, Vector2d center) {
            for(int i = -Range; i <= Range; i++) {
                for(int j = -Range; j <= Range; j++) {
                    var v = new Vector2d(tms.x + i, tms.y + j);
                    if(Tiles.ContainsKey(v))
                        continue;
                    StartCoroutine(CreateTile(v, center));
                }
            }
        }

        protected virtual IEnumerator CreateTile(Vector2d tileTms, Vector2d centerInMercator) {
            var rect = GM.TileBounds(tileTms, Zoom);
            var tile = new GameObject("tile " + tileTms.x + "-" + tileTms.y).AddComponent<Tile>();

            tile.Zoom = Zoom;
            tile.TileTms = tileTms;
            tile.TileCenter = rect.Center;
            tile.Material = MapMaterial;
            tile.Rect = GM.TileBounds(tileTms, Zoom);

            Tiles.Add(tileTms, tile);
            tile.transform.position = (rect.Center - centerInMercator).ToVector3();
            tile.transform.SetParent(TileHost, false);
            LoadTile(tileTms, tile);

            yield return null;
        }

        protected virtual void LoadTile(Vector2d tileTms, Tile tile) {
            var url = string.Format(_mapzenUrl, _mapzenLayers, Zoom, tileTms.x, tileTms.y, _mapzenFormat, _key);

            ObservableWWW.Get(url)
                .Subscribe(
                    text => { ConstructTile(text, tile); }, //success
                    exp => {
                        Debug.Log("Error fetching -> " + url);
                        Debug.LogWarning("We've probably exceeded Mapzen query limit. You better get your own Api key! (https://mapzen.com/developers/)");
                    }); //failure
        }

        protected void ConstructTile(string text, Tile tile) {
            var heavyMethod = Observable.Start(() => new JSONObject(text));

            heavyMethod.ObserveOnMainThread().Subscribe(mapData => {
                if(!tile) // checks if tile still exists and haven't destroyed yet
                    return;
                tile.Data = mapData;

                foreach(var factory in _plugins) {
                    factory.Create(tile);
                }
            });
        }

        protected void setCenterTms(Vector2d CenterTms) {
            this.CenterTms = CenterTms;
        }     
    }
}
