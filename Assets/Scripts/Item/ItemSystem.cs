using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public static Item GetItem(string name)
    {
        return Resources.Load<Item>($"Items/{name}");
    }
}
