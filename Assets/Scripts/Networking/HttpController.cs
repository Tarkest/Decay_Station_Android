using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpController : MonoBehaviour
{
    public static HttpController instance; 
    private static Queue<IEnumerator> queue = new Queue<IEnumerator>();
    private string authToken;

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

    public void SetAuthToken(string token)
    {
        authToken = "Bearer " + token;
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
            if(authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
            }
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
            byte[] bytes = GetBytes(json);
            UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
            request.uploadHandler = uH;
            request.SetRequestHeader("Content-Type", "application/json");
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
            }
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
            byte[] bytes = GetBytes(json);
            UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
            request.uploadHandler = uH;
            request.SetRequestHeader("Content-Type", "application/json");
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
            }
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
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
            }
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

    UnityWebRequest CreateUnityWebRequest(string url, string param, string method)
    {
        UnityWebRequest requestU = new UnityWebRequest(url, method);
        byte[] bytes = GetBytes(param);
        UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
        requestU.uploadHandler = uH;
        requestU.SetRequestHeader("Content-Type", "application/json");
        CastleDownloadHandler dH = new CastleDownloadHandler();
        requestU.downloadHandler = dH;
        return requestU;
    }

    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return bytes;
    }
}


class CastleDownloadHandler : DownloadHandlerScript
{
    public delegate void Finished();
    public event Finished onFinished;

    protected override void CompleteContent()
    {
        base.CompleteContent();
        if (onFinished != null)
        {
            onFinished();
        }
    }
}
