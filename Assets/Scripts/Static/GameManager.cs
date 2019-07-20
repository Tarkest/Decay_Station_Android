using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string testJson;
    public static int accountId;
    public static string nickname;
    public static int accountLevel;
    public static int accountExperience;

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
    public static LocomotiveAgent locomotive;
    public static CarriageAgent[] carriages = new CarriageAgent[0];
    public static TrainNPCData[] characters = new TrainNPCData[0];

    void Awake()
    {
        DontDestroyOnLoad(this);
        if(PlayerPrefs.GetInt("Language", -1) == -1)
        {
            int _currentLanguage = 0;
            if (Application.systemLanguage == SystemLanguage.English)
                _currentLanguage = 0;
            else if (Application.systemLanguage == SystemLanguage.Russian)
                _currentLanguage = 1;
            PlayerPrefs.SetInt("Language", _currentLanguage);
            PlayerPrefs.Save();
        }
    }
}
