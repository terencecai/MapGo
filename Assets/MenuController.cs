using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
public class MenuController : MonoBehaviour
{

    [SerializeField] public Button ProfileButton;
    [SerializeField] public Toggle Navigation;
    [SerializeField] public Toggle Search;
    [SerializeField] public InputField SearchField;

    private LocationManager _locationManager;
    private MapzenGo.Models.TileManager _tileManager;
    void Start()
    {
        ProfileButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("ProfileScene"));
        Navigation.onValueChanged.AddListener(onNavigationToggled);
        Search.onValueChanged.AddListener(v => StartCoroutine(onSearchToggled(v)));

        var world = GameObject.Find("World");
        _locationManager = world.GetComponent<LocationManager>();
        _tileManager = world.GetComponent<MapzenGo.Models.TileManager>();
    }

    void OnDestroy()
    {
        ProfileButton.onClick.RemoveAllListeners();
        Navigation.onValueChanged.RemoveAllListeners();
        Search.onValueChanged.RemoveAllListeners();
    }

    private void onNavigationToggled(bool v)
    {
        PlayerPrefs.SetString("navigation", v.ToString().ToLower());
        if (!v)
        {
            GameObject.Find("World").GetComponent<UIManager>().disableWarning();
        }
    }

    private IEnumerator onSearchToggled(bool v)
    {
        if (!v) {
            var anim = SearchField.GetComponent<Animator>();
            anim.SetBool("Enabled", true);
            SearchField.GetComponent<SearchController>().DisableResults();
            yield return new WaitForSeconds(0.5f);
        }
        SearchField.gameObject.SetActive(v);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
