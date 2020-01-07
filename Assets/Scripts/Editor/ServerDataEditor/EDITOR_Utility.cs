using System;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class EDITOR_Utility
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

    public static void DELETE(string route, int id, Action<string, string> callback, string authToken)
    {
        StartBackgroundTask(DeleteCoroutine(route, id, callback, authToken));
    }

    public static M[] AssociateModels<M, SD, PR>(SD[] serverData, PR[] prefabsList) 
        where M : class
        where SD : ServerData
        where PR : UnityEngine.Object
    {
        M[] _buffer = new M[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if (serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = (M)Activator.CreateInstance(typeof(M), serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (PR prefab in prefabsList)
        {
            if (prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = (M)Activator.CreateInstance(typeof(M), null, prefab);
            }
        }
        foreach (SD sData in serverData)
        {
            if (sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = (M)Activator.CreateInstance(typeof(M), sData, null);
            }
        }
        return _buffer;
    }

    private static IEnumerator GetCoroutine(string route, Action<string, string> callback, string authToken)
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
                callback.Invoke(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback.Invoke(null, request.downloadHandler.text);
            }
            else
            {
                callback.Invoke(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator PostCoroutine(string route, string json, Action<string, string> callback, string authToken)
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
                callback.Invoke(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback.Invoke(null, request.downloadHandler.text);
            }
            else
            {
                callback.Invoke(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator PutCoroutine(string route, string json, Action<string, string> callback, string authToken)
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
                callback.Invoke(null, request.error);
            }
            else if(request.isHttpError)
            {
                callback.Invoke(null, request.downloadHandler.text);
            }
            else
            {
                callback.Invoke(request.downloadHandler.text, null);
            }
        }
    }

    private static IEnumerator DeleteCoroutine(string route, int id, Action<string, string> callback, string authToken)
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
                callback.Invoke(null, request.error);
            }
            else if (request.isHttpError)
            {
                callback.Invoke(null, request.downloadHandler.text);
            }
            else
            {
                callback.Invoke(request.downloadHandler.text, null);
            }
        }
    }

    private static byte[] GetBytes(string json)
    {
        return Encoding.UTF8.GetBytes(json);
    }
}
