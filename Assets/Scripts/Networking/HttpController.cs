using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpController : MonoBehaviour
{
    public static HttpController instance; 
    private static Queue<IEnumerator> queue = new Queue<IEnumerator>();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        for(int i = 0; i<queue.Count; i++)
        {
            StartCoroutine(queue.Dequeue());
        }
    }

    public void GET(string path, Action<string, string> callback)
    {
        queue.Enqueue(GETRequest(path, callback));
    }

    public void POST(string path, string json, Action<string, string> callback)
    {
        queue.Enqueue(POSTrequest(path, json, callback));
    }

    public void PUT(string path, string json, Action<string, string> callback)
    {
        queue.Enqueue(PUTrequest(path, json, callback));
    }

    public void DELETE(string path, Action<string, string> callback)
    {
        queue.Enqueue(DeleteRequest(path, callback));
    }

    public IEnumerator GETRequest(string path, Action<string, string> callback) {
        using (UnityWebRequest request = UnityWebRequest.Get(StaticClasses.SERVER_ADRESS + "/" + path))
        {
            if (PlayerPrefs.HasKey("accountKey"))
            {
                request.SetRequestHeader("auth", PlayerPrefs.GetString("accountKey"));
                if (PlayerPrefs.HasKey("accountGoogleKey"))
                {
                    request.SetRequestHeader("authGoogle", PlayerPrefs.GetString("accountGoogleKey"));
                }
            }
            else
            {
                PlayerPrefs.SetString("accountKey", GenerateAccountKey());
                request.SetRequestHeader("auth", PlayerPrefs.GetString("accountKey"));
                PlayerPrefs.Save();
                if (PlayerPrefs.HasKey("accountGoogleKey"))
                {
                    request.SetRequestHeader("authGoogle", PlayerPrefs.GetString("accountGoogleKey"));
                }
            }
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                callback(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                callback(request.downloadHandler.text, "");
            }
        }
    }

    public IEnumerator POSTrequest(string path, string json, Action<string, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(StaticClasses.SERVER_ADRESS + "/" + path, json))
        {
            if (PlayerPrefs.GetString("accountKey") != "")
                request.SetRequestHeader("auth", PlayerPrefs.GetString("accountKey"));
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                callback(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                callback(request.downloadHandler.text, "");
            }
        }
    }

    public IEnumerator PUTrequest(string path, string json, Action<string, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(StaticClasses.SERVER_ADRESS + "/" + path, json))
        {
            if (PlayerPrefs.GetString("accountKey") != "")
                request.SetRequestHeader("auth", PlayerPrefs.GetString("accountKey"));
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                callback(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                callback(request.downloadHandler.text, "");
            }
        }
    }

    public IEnumerator DeleteRequest(string path, Action<string, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(StaticClasses.SERVER_ADRESS + "/" + path))
        {
            if (PlayerPrefs.GetString("accountKey") != "")
                request.SetRequestHeader("auth", PlayerPrefs.GetString("accountKey"));
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                callback(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                callback(request.downloadHandler.text, "");
            }
        }
    }

    private string GenerateAccountKey()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "");
    }
}
