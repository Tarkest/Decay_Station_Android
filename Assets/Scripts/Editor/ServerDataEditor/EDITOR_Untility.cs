using System;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class EDITOR_Untility
{
    private static void StartBackgroundTask(IEnumerator update, Action end = null)
    {
        EditorApplication.CallbackFunction closureCallback = null;

        closureCallback = () =>
        {
            try
            {
                if (update.MoveNext() == false)
                {
                    end?.Invoke();
                    EditorApplication.update -= closureCallback;
                }
            }
            catch (Exception ex)
            {
                end?.Invoke();
                Debug.LogException(ex);
                EditorApplication.update -= closureCallback;
            }
        };

        EditorApplication.update += closureCallback;
    }

    public static void GET(string route, Action<Responce> callback, string authToken)
    {
        StartBackgroundTask(GetCoroutine(route, callback, authToken));
    }

    public static void POST(string route, string json, Action<Responce> callback, string authToken)
    {
        StartBackgroundTask(PostCoroutine(route, json, callback, authToken));
    }

    public static void PUT(string route, string json, Action<Responce> callback, string authToken)
    {
        StartBackgroundTask(PutCoroutine(route, json, callback, authToken));
    }

    public static void DELETE(string route, int id, Action<Responce> callback, string authToken)
    {
        StartBackgroundTask(DeleteCoroutine(route, id, callback, authToken));
    }

    private static IEnumerator GetCoroutine(string route, Action<Responce> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(StaticClasses.SERVER_ADRESS + "admin/" + route))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            if (request.isNetworkError)
            {
                callback.Invoke(new Responce(request.error));
            }
            else
            {
                callback.Invoke(JSON.FromJSON<Responce>(request.downloadHandler.text));
            }
        }
    }

    private static IEnumerator PostCoroutine(string route, string json, Action<Responce> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(StaticClasses.SERVER_ADRESS + "admin/" + route, json))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(GetBytes(json));
            yield return request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            if (request.isNetworkError)
            {
                callback.Invoke(new Responce(request.error));
            }
            else
            {
                callback.Invoke(JSON.FromJSON<Responce>(request.downloadHandler.text));
            }
        }
    }

    private static IEnumerator PutCoroutine(string route, string json, Action<Responce> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(StaticClasses.SERVER_ADRESS + "admin/" + route, json))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            if (request.isNetworkError)
            {
                callback.Invoke(new Responce(request.error));
            }
            else
            {
                callback.Invoke(JSON.FromJSON<Responce>(request.downloadHandler.text));
            }
        }
    }

    private static IEnumerator DeleteCoroutine(string route, int id, Action<Responce> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(StaticClasses.SERVER_ADRESS + "admin/" + route + "?id=" + id))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }
            if (request.isNetworkError)
            {
                callback.Invoke(new Responce(request.error));
            }
            else
            {
                callback.Invoke(JSON.FromJSON<Responce>(request.downloadHandler.text));
            }
        }
    }

    private static byte[] GetBytes(string json)
    {
        return Encoding.UTF8.GetBytes(json);
    }
}
[Serializable]
public struct Responce
{
    public string data;
    public string error;

    public Responce(string networkError)
    {
        data = null;
        error = networkError;
    }
}
