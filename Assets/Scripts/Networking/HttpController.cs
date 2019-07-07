using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpController : MonoBehaviour
{
    public static HttpController instance; 
    public delegate void Callback(string json);
    public delegate void CallbackBool(bool result);
    private static Queue<IEnumerator> queue = new Queue<IEnumerator>();

    private void Update()
    {
        for(int i = 0; i<queue.Count; i++)
        {
            StartCoroutine(queue.Dequeue());
        }
    }

    public void GET(string path, Callback callback)
    {
        queue.Enqueue(GETRequest(path, callback));
    }

    public void POST(string path, string json, Callback callback)
    {
        queue.Enqueue(POSTrequest(path, json, callback));
    }

    public void PUT(string path, string json, Callback callback)
    {
        queue.Enqueue(PUTrequest(path, json, callback));
    }

    public void DELETE(string path, CallbackBool callback)
    {
        queue.Enqueue(DeleteRequest(path, callback));
    }

    public IEnumerator GETRequest(string path, Callback callback) {
        UnityWebRequest request = UnityWebRequest.Get(StaticClasses.SERVER_ADRESS+"/"+path);
        if(PlayerPrefs.GetString("accountKey") != "")
            request.SetRequestHeader("accountKey", PlayerPrefs.GetString("accountKey"));
        yield return request.SendWebRequest();

        if(request.isNetworkError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else if(request.isHttpError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator POSTrequest(string path, string json, Callback callback)
    {
        UnityWebRequest request = UnityWebRequest.Post(StaticClasses.SERVER_ADRESS+"/"+path, json);
        if (PlayerPrefs.GetString("accountKey") != "")
            request.SetRequestHeader("accountKey", PlayerPrefs.GetString("accountKey"));
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else if (request.isHttpError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator PUTrequest(string path, string json, Callback callback)
    {
        UnityWebRequest request = UnityWebRequest.Put(StaticClasses.SERVER_ADRESS + "/" + path, json);
        if (PlayerPrefs.GetString("accountKey") != "")
            request.SetRequestHeader("accountKey", PlayerPrefs.GetString("accountKey"));
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else if (request.isHttpError)
        {
            Debug.Log(request.error);
            callback(null);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator DeleteRequest(string path, CallbackBool callback)
    {
        UnityWebRequest request = UnityWebRequest.Delete(StaticClasses.SERVER_ADRESS + "/" + path);
        if (PlayerPrefs.GetString("accountKey") != "")
            request.SetRequestHeader("accountKey", PlayerPrefs.GetString("accountKey"));
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            callback(false);
        }
        else if (request.isHttpError)
        {
            Debug.Log(request.error);
            callback(false);
        }
        else
        {
            callback(true);
        }
    }
}
