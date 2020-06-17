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
