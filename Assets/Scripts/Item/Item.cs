using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string Name;
    public int Amount;


    public void ApplyCopy(ItemCopy copy)
    {
        Name = copy.Name;
        Amount = copy.Amount;
    }
}

public struct ItemCopy
{
    public string Name;
    public int Amount;

    public ItemCopy(Item item)
    {
        this.Name = item.Name;
        this.Amount = item.Amount;
    }
}
