using System;
using System.Collections;
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

    public static void GET(string route, Action<string, string> callback, string authToken)
    {
        StartBackgroundTask(GetCoroutine(route, callback, authToken));
    }

    public static void POST(string route, string json, Action<string, string> callback, string authToken)
    {
        StartBackgroundTask(PostCoroutine(route, json, callback, authToken));
    }

    public static void PUT(string route, string json, Action<string, string> callback, string authToken)
    {
        StartBackgroundTask(PutCoroutine(route, json, callback, authToken));
    }

    public static void DELETE(string route, Action<string, string> callback, string authToken)
    {
        StartBackgroundTask(DeleteCoroutine(route, callback, authToken));
    }

    private static IEnumerator GetCoroutine(string route, Action<string, string> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(StaticClasses.SERVER_ADRESS + "/" + route))
        {
            request.SetRequestHeader("auth", authToken);
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
                callback(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator PostCoroutine(string route, string json, Action<string, string> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(StaticClasses.SERVER_ADRESS + "/" + route, json))
        {
            request.SetRequestHeader("auth", authToken);
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
                callback(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator PutCoroutine(string route, string json, Action<string, string> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(StaticClasses.SERVER_ADRESS + "/" + route, json))
        {
            request.SetRequestHeader("auth", authToken);
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
                callback(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator DeleteCoroutine(string route, Action<string, string> callback, string authToken)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(StaticClasses.SERVER_ADRESS + "/" + route))
        {
            request.SetRequestHeader("auth", authToken);
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
                callback(request.downloadHandler.text, null);
            }
        }
    }
}
