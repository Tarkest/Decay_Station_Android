using UnityEngine;

/// <summary>
/// If you add some fields to item you should add them also to:
///     1. ItemCopy (variable and constructor)
///     2. Item's ApplyCopy function
///     3. ItemEditor inspector field and Validation (if necessary)
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Icon;

    /// <summary>
    /// Function that allows apply to item changes from ItemCopy
    /// used in ItemSystemWindow to undo changes
    /// </summary>
    /// <param name="copy">Item that has source fields for Item</param>
    public void ApplyCopy(ItemCopy copy)
    {
        Name = copy.Name;
        Icon = copy.Icon;
    }
}

public struct ItemCopy
{
    public string Name;
    public Sprite Icon;

    public ItemCopy(Item item)
    {
        this.Name = item.Name;
        this.Icon = item.Icon;
    }
}
