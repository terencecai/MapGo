using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class ProfileBehaviour : MonoBehaviour
{

    [SerializeField] public Text ProfileName;
    [SerializeField] public Text CharType;
    [SerializeField] public Text Level;
    [SerializeField] public Text Exp;
    [SerializeField] public Button BackButton;
    [SerializeField] public Button EditButton;
    [SerializeField] public Button ProfileAvatar;
    [SerializeField] public Image AutorityBar;
    [SerializeField] public Image CompassionBar;
    [SerializeField] public Image IntelligenceBar;
    [SerializeField] public Button Logout;

    [SerializeField] public GameObject EditPanel;
    [SerializeField] public GameObject ValuePanel;

    void Start()
    {
        if (EditPanel.activeInHierarchy)
        {
            EditPanel.SetActive(false);
        }
        Logout.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadSceneAsync("TestScene");
        });
        EditButton.onClick.AddListener(() => EditPanel.SetActive(true));
        BackButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("CachedDynamicLoader"));
    }

    void OnEnable()
    {
        loadProfile();
    }

    private void loadProfile()
    {
        var token = PlayerPrefs.GetString("token", "");
        Observable.WhenAll(RestClient.getMySkills(token), RestClient.getAllSkills(token), RestClient.getProfile(token))
            .Subscribe(
                x =>
                {
                    var p = JsonUtility.FromJson<Profile>(x[2]);
                    p.skills = ApplicationLoadController.convertSkills(x[0]);
                    p.allSkills = ApplicationLoadController.convertSkills(x[1]);
                    ProfileRepository.Instance.SaveProfileJson(p);
                    fillData(p);

                },
                e => Debug.Log(e)
            );
    }

    private void fillData(Profile profile)
    {
        try
        {
            ProfileName.text = profile.nickName;
            CharType.text = profile.chosenValue.name;
            AutorityBar.fillAmount     = profile.values.Find(x => x.name == "Authority").level / 100.0f;
            CompassionBar.fillAmount   = profile.values.Find(x => x.name == "Compassion").level / 100.0f;
            IntelligenceBar.fillAmount = profile.values.Find(x => x.name == "Intelligence").level / 100.0f;
            ValuePanel.GetComponent<ValueSwitch>().SetValue(profile.currentValue);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
