using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainRoomLoader : MonoBehaviour
{
    private static Slider _loadScreen;

    /// <summary>
    /// Request account information from server and start instantiatng it on scene
    /// </summary>
    public static void LoadTrain()
    {
        HttpController.instance.GET("api/user/info", InstantiateTrain);
        _loadScreen = (Instantiate(Resources.Load("UI/LoadScreens/LoadScreen"), GameObject.Find("Canvas").transform) as GameObject).transform.GetChild(0).GetComponent<Slider>(); 
    }

    /// <summary>
    /// Instantiate locomotive and carriages with building to train scene based on account info json
    /// </summary>
    /// <param name="accountInfo">JSON string what contain account info</param>
    private static void InstantiateTrain(string accountInfo, string error)
    {
        if(error.Length > 0)
        {
            Debug.Log("TrainRoomLoader ON_TRAIN_LOAD " + error);
        }
        else
        {
            Account info = JsonUtility.FromJson<Account>(accountInfo);
            _loadScreen.value += 10;
            GameManager.accountId = info.id;
            GameManager.accountLevel = info.level;
            GameManager.nickname = info.nickname;
            GameManager.accountExperience = info.accountExperience;
            _loadScreen.value += 10;
            LocomotiveAgent locomotiveInstance = (Instantiate(Resources.Load("Locomotive/Instances/" + info.locomotives[0].type.name)) as GameObject).GetComponent<LocomotiveAgent>();
            locomotiveInstance.LoadInstance(info.locomotives[0]);
            GameManager.locomotive = locomotiveInstance;
            _loadScreen.value += 10;
            for (int i = 0; i < info.carriages.Length; i++)
            {
                TrainAgent _previousAgent = GameManager.locomotive;
                if (i > 0)
                    _previousAgent = GameManager.carriages[i - 1];
                CarriageAgent carriageInstance = (Instantiate(Resources.Load("Carriages/Instances/" + info.carriages[i].type.name)) as GameObject).GetComponent<CarriageAgent>();
                carriageInstance.transform.position = new Vector3(_previousAgent.transform.position.x - (_previousAgent.sprite.bounds.extents.x + carriageInstance.sprite.bounds.extents.x), 0);
                carriageInstance.LoadInstance(info.carriages[i]);
                Array.Resize(ref GameManager.carriages, i + 1);
                GameManager.carriages[i] = carriageInstance;
                _loadScreen.value += 35 / info.carriages.Length;
            }
            for (int i = 0; i < info.characters.Length; i++)
            {
                TrainNPCAgent instance = (Instantiate(Resources.Load("Agents/NPC/" + info.characters[i].specialization.name + "/" + info.characters[i].type.name), Vector3.zero, Quaternion.identity) as GameObject).GetComponent<TrainNPCAgent>();
                Array.Resize(ref GameManager.characters, i + 1);
                GameManager.characters[i] = new TrainNPCData(info.characters[i], instance);
                _loadScreen.value += 35 / info.characters.Length;
            }
            Destroy(_loadScreen.transform.parent.gameObject);
        }
    }
}
