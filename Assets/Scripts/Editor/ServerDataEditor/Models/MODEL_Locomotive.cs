using System;
using UnityEngine;

public class LocomotiveModel
{
    public LocomotiveModel(LocomotiveServerData serverData, GameObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public LocomotiveServerData serverData;
    public GameObject prefab;
}

[Serializable]
public class LocomotiveServerData : ServerData
{
    public LocomotiveServerData()
    {
        this.name = "";
    }

    public LocomotiveServerData(string name)
    {
        this.name = name;
    }

    public int maxLevel;
    public bool inRotation;
    public int count;
    public LocomotiveUpgradeItem[] upgradesRecipes;
    public BuildingSlotData[] buildingSlots;
}

[Serializable]
public class LocomotiveUpgradeItem
{
    public LocomotiveUpgradeItem(int level)
    {
        this.level = level;
        count = 1;
    }

    public int id;
    public int level;
    public int count;
    public ItemServerData item;
}

[Serializable]
public class BuildingSlotData
{
    public BuildingSlotData()
    {

    }

    public int id;
    public int level;
    public int index;
    public ConstantsServerData buildingType;
}
