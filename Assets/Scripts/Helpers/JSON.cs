using UnityEngine;

public static class JSON
{
    public static T FromJSON<T>(string jsonString)
    {
        return JsonUtility.FromJson<T>(jsonString);
    } 

    public static string ToJSON(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static string ToJSON(object obj, bool prettyPrint)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    public static string ToJSONArray<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJSONArray<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static T[] FromJSONArray<T>(string jsonString)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonString);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}


