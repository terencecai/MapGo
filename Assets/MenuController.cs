using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using MapzenGo.Models;
public class MenuController : MonoBehaviour
{

    [SerializeField] public Button ProfileButton;
    [SerializeField] public Toggle Navigation;
    [SerializeField] public Toggle Search;
    [SerializeField] public InputField SearchField;

    private LocationManager _locationManager;
    private CachedDynamicTileManager _tileManager;
    void Start()
    {
        ProfileButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("ProfileScene"));
        Navigation.onValueChanged.AddListener(onNavigationToggled);
        Search.onValueChanged.AddListener(onSearchToggled);

        var world = GameObject.Find("World");
        _locationManager = world.GetComponent<LocationManager>();
        _tileManager = world.GetComponent<CachedDynamicTileManager>();
    }

    void OnDestroy()
    {
        ProfileButton.onClick.RemoveAllListeners();
        Navigation.onValueChanged.RemoveAllListeners();
        Search.onValueChanged.RemoveAllListeners();
    }

    private void onNavigationToggled(bool v)
    {
        LiveParams.NavigationEnabled = v;
        if (!v)
        {
            GameObject.Find("World").GetComponent<UIManager>().disableWarning();
        }
    }

    private void onSearchToggled(bool v)
    {
        if (!v)
        {
            LiveParams.TeleportEnabled = false;
            if (LiveParams.ComingToRealLocation)
                refreshMap();

            disableSearch();
            return;
        }
        SearchField.gameObject.SetActive(v);
    }

    private void disableSearch()
    {
        SearchField.GetComponent<SearchController>().DisableResults();
        LiveParams.SetComing(false);
    }

    private void refreshMap()
    {
        if (_tileManager == null)
            _tileManager = GameObject.Find("World").GetComponent<CachedDynamicTileManager>();
        _tileManager.ClearAllTiles();
        _tileManager.InitMap();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
