using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static string nickname;
    public static int accountLevel;
    public static int accountExperience;
    public Slider loadingBar;
    public GameObject loadingScreen;

    public static GameManager instance;
    public static LoadingManager loadingManager;

    public static int language
    {
        get
        {
            return PlayerPrefs.GetInt("Language");
        }
        set
        {
            PlayerPrefs.SetInt("Language", value);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        loadingScreen.gameObject.SetActive(true);
    }

    async void Start()
    {
        if (PlayerPrefs.GetInt("Language", -1) == -1)
        {
            int _currentLanguage = 0;
            if (Application.systemLanguage == SystemLanguage.English)
                _currentLanguage = 0;
            else if (Application.systemLanguage == SystemLanguage.Russian)
                _currentLanguage = 1;
            PlayerPrefs.SetInt("Language", _currentLanguage);
            PlayerPrefs.Save();
        }
        if(await GoogleController.Authentificate())
        {
            HttpController.SetAuthToken(await HttpController.POST<string>("login", JSON.ToJSON(new LoginVariables("test"))));
            LoadMainScene();
        }
    }

    private void Update()
    {
        if(loadingManager != null)
        {
            loadingBar.value = loadingManager.progress / 100f;
        }
    }

    private async void LoadMainScene()
    {
        var _accountData = await HttpController.GET<AccountData>("getAccountInfo");
        await SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MAIN));
        loadingManager = LoadingManager.instance;
        StartCoroutine(loadingManager.LoadTrainCoroutine(_accountData));
    }
}
