using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static string authToken;
    private static bool _authentificationSuccess = false;
    public static string nickname;
    public static int accountLevel;
    public static int accountExperience;
    public Text text;

    public static HttpController networkController;

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
    }

    void Start()
    {
        networkController = HttpController.instance;
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
        GoogleController.Authentificate((success) => {
            _authentificationSuccess = success;
            text.text = "Begin login request";
            var data = new AccountDataTest(Social.localUser.id);
            Debug.Log(data);
            var id = JSON.ToJSON(data);
            Debug.Log(id);
            text.text = id;
            networkController.POST("login", id, (loginData, loginError) => { 
                if(loginError != "")
                {
                    text.text = loginError;
                }
                else
                {
                    text.text = "Token: " + loginData;
                    networkController.SetAuthToken(loginData);
                    networkController.GET("getAccountInfo", (accountData, accountError) => {
                        if(accountError != "")
                        {
                        }
                        else
                        {
                            text.text = accountData;
                        }
                    });
                }
            }); 
        });
    }
}

[Serializable]
public class AccountDataTest
{
    public string googleId;

    public AccountDataTest(string id)
    {
        googleId = id;
    }
}
