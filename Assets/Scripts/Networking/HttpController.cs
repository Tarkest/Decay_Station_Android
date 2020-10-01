using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class HttpController
{
    private static string authToken;

    public static void SetAuthToken(string token)
    {
        authToken = "Bearer " + token;
        UnityEngine.Debug.Log(authToken);
    }

    public static async Task<T> GET<T>(string path) {
        using (UnityWebRequest request = UnityWebRequest.Get(StaticClasses.SERVER_ADRESS + path))
        {
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            await request.SendWebRequest();

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(request.downloadHandler.text, typeof(T));
            }
            return JSON.FromJSON<T>(request.downloadHandler.text);
        }
    }

    public static async Task<T> POST<T>(string path, string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(StaticClasses.SERVER_ADRESS + path, json))
        {
            byte[] bytes = GetBytes(json);
            UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
            request.uploadHandler = uH;
            request.SetRequestHeader("Content-Type", "application/json");
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            await request.SendWebRequest();

            if(typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(request.downloadHandler.text, typeof(T));
            }
            return JSON.FromJSON<T>(request.downloadHandler.text);
        }
    }

    public static async Task<T> PUT<T>(string path, string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(StaticClasses.SERVER_ADRESS + path, json))
        {
            byte[] bytes = GetBytes(json);
            UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
            request.uploadHandler = uH;
            request.SetRequestHeader("Content-Type", "application/json");
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            await request.SendWebRequest();

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(request.downloadHandler.text, typeof(T));
            }
            return JSON.FromJSON<T>(request.downloadHandler.text);
        }
    }

    public static async Task<T> Delete<T>(string path)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(StaticClasses.SERVER_ADRESS + path))
        {
            if (authToken != null)
            {
                request.SetRequestHeader("Authorization", authToken);
            }
            await request.SendWebRequest();

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(request.downloadHandler.text, typeof(T));
            }
            return JSON.FromJSON<T>(request.downloadHandler.text);
        }
    }

    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return bytes;
    }
}
