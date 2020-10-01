using System;
using UnityEngine;

public class CarriageModel
{
    public CarriageModel(CarriageServerType serverData, GameObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public CarriageServerType serverData;
    public GameObject prefab;
}

[Serializable]
public class CarriageServerType
{
    public int id;
    public string name;
    public CarriageBuildingPosition[] buildingSlot;
    public int count;
    public int storageCapacity;
    public int crewCapacity;
    public CarriageAssemblyItem[] assemblyItems;
}

[Serializable]
public class CarriageAssemblyItem
{
    public int itemId;
    public int count;
}

[Serializable]
public class CarriageBuildingPosition
{
    public string BuildingType;
    public int BuildingSize;
}

[Serializable]
public class Carriage
{
    public int id;
    public CarriageServerType data;
    public CarriageAssembleSlot[] assembleSlots;
    public InventorySlot[] inventory;
}

[Serializable]
public class CarriageAssembleSlot
{
    public int id;
    public int count;
    public int requiredCount;
    public ItemData item;
}
