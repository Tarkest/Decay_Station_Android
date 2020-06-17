using System;
using UnityEngine;

public class ItemModel
{
    public ItemServerData serverData;
    public ItemObject prefab;

    public ItemModel(ItemServerData serverData, ScriptableObject prefab)
    {
        this.serverData = serverData;
        this.prefab = (ItemObject)prefab;
    }
}

[Serializable]
public class ItemData : ServerData
{
    public int maxCount;
    public ConstantsServerData type;
    public ConstantsServerData rarity;
}

[Serializable]
public class ItemServerData : ItemData
{
    public ItemServerData(int id)
    {
        this.id = id;
    }

    public ItemServerData(string name, int maxCount, int typeId, int rarityId)
    {
        this.name = name;
        this.maxCount = maxCount;
        type = new ConstantsServerData(typeId);
        rarity = new ConstantsServerData(rarityId);
    }

    public ItemServerData(int id, int maxCount, int typeId, int rarityId)
    {
        this.id = id;
        this.maxCount = maxCount;
        type = new ConstantsServerData(typeId);
        rarity = new ConstantsServerData(rarityId);
    }


    public ItemServerBufferData updateBuffer;
    public int count;
    public bool inRotation;
}

[Serializable]
public class ItemServerBufferData : ItemData { }
