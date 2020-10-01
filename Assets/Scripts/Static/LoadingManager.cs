using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public float progress = 0;

    public static LoadingManager instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator LoadTrainCoroutine(AccountData accountData)
    {
        progress = 0;
        GameObject locomotive = Instantiate((GameObject)Resources.Load($"Locomotive/Instances/{accountData.locomotive.data.name}", typeof(GameObject)));
        progress += 10f;
        yield return new WaitForEndOfFrame();
    }
}
