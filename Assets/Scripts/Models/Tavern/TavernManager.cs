using UnityEngine;
using MapzenGo.Helpers;
using MapzenGo.Models;
using UniRx;
using System;
using System.Collections.Generic;

public class TavernManager : MonoBehaviour
{

    [SerializeField] public GameObject Tavern;
    [SerializeField] public GameObject Depot;

    private List<GameObject> taverns = new List<GameObject>();
    private List<GameObject> depots = new List<GameObject>();

    public Tavern activeTavern;
    private SocketManager socketManager;

    void Start()
    {
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

    public void RequestTaverns(Location location) {
        RestClient.getTaverns(PlayerPrefs.GetString("token", ""), location.GetLatitude(), location.GetLongitude())
            .Subscribe(
                onSuccess,
                onError
            );
    }

    private void onSuccess(WWW response)
    {
        Debug.Log("response is : " + response.text);
        var data = JsonUtility.FromJson<DataPacket>(response.text);
        if (data != null)
        {
            createTaverns(data.nearest_taverns);
            createDepots(data.nearest_depots);
        }
        else
        {
            Debug.Log("Data is null");
        }
    }

    private void onError(Exception e)
    {
        Debug.Log(e.ToString());
        RestClient.sendDebug(e.ToString());
    }

    private void createDepots(List<Depot> nearest_depots)
    {
        if (nearest_depots == null)
        {
            Debug.Log("nearest_depots is null");
            return;
        }

        if (Depot == null || Depot.transform == null)
        {
            return;
        }

        if (depots == null)
        {
            taverns = new List<GameObject>();
        }

        for (int i = 0; i < nearest_depots.Count; i++)
        {
            try
            {
                if (depots.Count <= i)
                {
                    depots.Add(Instantiate(Depot, Depot.transform.position, Depot.transform.rotation));
                }

                var obj = getDepotOrNull(i);
                if (obj == null)
                {
                    obj = Instantiate(Depot, Depot.transform.position, Depot.transform.rotation);
                    depots.Add(obj);
                }
                showDepot(nearest_depots[i], obj);
            }
            catch (Exception exc)
            {
                RestClient.sendDebug(exc.ToString());
            }
        }
    }

    private void showDepot(Depot depot, GameObject obj)
    {
        if (obj.activeSelf)
        {
            if (obj.GetComponent<DepotBehaviour>().Depot.name != depot.name)
            {
                obj.SetActive(false);
            } else return;
        }
        Tile tile;
        var meters = GM.LatLonToMeters(depot.latitude, depot.longitude);
        var tiles = GameObject.Find("Tiles");
        if (tiles != null)
        {
            foreach (Transform child in tiles.transform)
            {
                tile = child.GetComponent<Tile>();
                if (tile.Rect.Contains(meters))
                {
                    obj.SetActive(true);
                    obj.transform.SetParent(null);
                    obj.transform.position = (meters - tile.Rect.Center).ToVector3();
                    obj.transform.SetParent(tile.transform, false);
                    obj.GetComponent<DepotBehaviour>().Depot = depot;
                    RestClient.sendDebug("Depot " + depot.name + " created\n");
                    break;
                }

            }
        }
    }

    private GameObject getDepotOrNull(int index)
    {
        if (index >= depots.Count) return null;
        else return depots[index];
    }

    private void createTaverns(List<Tavern> nearest_taverns)
    {
        if (nearest_taverns == null)
        {
            Debug.Log("nearest_taverns is null");
            return;
        }

        if (Tavern == null || Tavern.transform == null)
        {
            return;
        }

        if (taverns == null)
        {
            taverns = new List<GameObject>();
        }

        for (int i = 0; i < nearest_taverns.Count; i++)
        {
            try
            {
                if (taverns.Count <= i)
                {
                    taverns.Add(Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation));
                }

                var obj = getTavernOrNull(i);
                if (obj == null)
                {
                    obj = Instantiate(Tavern, Tavern.transform.position, Tavern.transform.rotation);
                    taverns.Add(obj);
                }
                showTavern(nearest_taverns[i], obj);
            }
            catch (Exception exc)
            {
                RestClient.sendDebug(exc.ToString());
            }
        }
    }

    private GameObject getTavernOrNull(int index)
    {
        if (index >= taverns.Count) return null;
        else return taverns[index];
    }

    public void showTavern(Tavern tavern, GameObject tavernGO)
    {
        bool shown = false;
        if (tavernGO.activeSelf)
        {
            if (tavernGO.GetComponent<TavernBehaviour>().tavern.name != tavern.name)
            {
                tavernGO.SetActive(false);
            }
            else
            {
                RestClient.sendDebug("Tavern " + tavern.name + " not shown due to activeSelf");
                return;
            }
        }

        Tile tile;
        var meters = GM.LatLonToMeters(tavern.latitude, tavern.longitude);
        if (GameObject.Find("Tiles") != null)
        {
            foreach (Transform child in GameObject.Find("Tiles").transform)
            {
                tile = child.GetComponent<Tile>();
                if (tile.Rect.Contains(meters))
                {
                    tavernGO.SetActive(true);
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

        if (!shown)
        {
            sendDebugIfNotShown(tavern);
        }
    }

    private void sendDebugIfNotShown(Tavern tavern)
    {
        var d = "Tavern not shown.\nCoordinates:\n" + tavern.latitude + "\n" + tavern.longitude;
        d += "\nUser coordinates are:";
        d += "\n" + Input.location.lastData.latitude;
        d += "\n" + Input.location.lastData.longitude;
        RestClient.sendDebug(d);
    }

    public void hideTavern(GameObject t)
    {
        t.SetActive(false);
    }
}
