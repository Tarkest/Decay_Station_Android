using UnityEngine;

public class ButtonsCallbacks : MonoBehaviour
{
    public void OnClickInstantiate(GameObject o)
    {
        Instantiate(o, GameObject.Find("Canvas").transform);
    }
}
