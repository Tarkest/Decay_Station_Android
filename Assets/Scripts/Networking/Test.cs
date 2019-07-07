using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("HTTP").GetComponent<HttpController>().GET("api/user/info", GetLocomotive);
    }

    private void GetLocomotive(string answer)
    {
        Account l = JsonUtility.FromJson<Account>(answer);
    }
}
