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
        Search.onValueChanged.AddListener(v => StartCoroutine(onSearchToggled(v)));

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

    private IEnumerator onSearchToggled(bool v)
    {
        if (!v)
        {
            disableSearch();
            if (LiveParams.ComingToRealLocation)
                refreshMap();
            yield return new WaitForSeconds(0.5f);
        }
        SearchField.gameObject.SetActive(v);
    }

    private void disableSearch()
    {
        var anim = SearchField.GetComponent<Animator>();
        anim.SetBool("Enabled", true);
        SearchField.GetComponent<SearchController>().DisableResults();
        LiveParams.TeleportEnabled = false;
        LiveParams.ComingToRealLocation = false;
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
