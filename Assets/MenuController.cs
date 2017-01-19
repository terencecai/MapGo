using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] public Button ProfileButton;
    [SerializeField] public Toggle Navigation;
    void Start()
    {
        ProfileButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("ProfileScene"));
        Navigation.onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetString("navigation", v.ToString().ToLower());
            if (!v)
            {
                GameObject.Find("World").GetComponent<UIManager>().disableWarning();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
