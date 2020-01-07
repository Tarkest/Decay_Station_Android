using System;
using UnityEngine;

#region Misc

public class ServerData
{
    public int id;
    public string name;
}

[Serializable]
public class AccountData
{
    public string login;
    public string password;

    public AccountData(string login, string password)
    {
        this.login = login;
        this.password = password;
    }
}

[Serializable]
public class NewEntity
{
    public string name;

    public NewEntity(string name)
    {
        this.name = name;
    }
}

#endregion

#region Constants

public class ConstantsModel
{
    public ConstantsServerData serverData;
    public ScriptableObject prefab;

    public ConstantsModel(ConstantsServerData serverData, ScriptableObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }
}

[Serializable]
public class ConstantsServerData : ServerData
{

}

#endregion

#region Locomotive

public class LocomotiveModel
{
    public LocomotiveModel(LocomotiveServerType serverData, GameObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public LocomotiveServerType serverData;
    public GameObject prefab;
}

[Serializable]
public class LocomotiveServerType
{
    public LocomotiveServerType(string name)
    {
        this.name = name;
    }

    public int id;
    public string name;
    public int count;
    public LocomotiveUpgradeRecipe[] upgradesRecipes;
}

[Serializable]
public class LocomotiveUpgradeRecipe
{
    public LocomotiveUpgradeItem[] items;
}

[Serializable]
public class LocomotiveUpgradeItem
{
    public int id;
    public int itemId;
    public int count;
}

#endregion

#region Carriage

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
    public string appearenceVersion = "0000000";
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

#endregion

#region Item

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
public class ItemServerData : ServerData
{
    public int maxCount;
    public ConstantsServerData type;
    public ConstantsServerData rarity;
    public ItemServerBufferData updateBuffer;
    public int count;
}

[Serializable]
public class ItemServerBufferData : ItemServerData
{

}

#endregion

#region Building

public class BuildingModel
{
    public BuildingModel(BuildingServerType serverData, GameObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public BuildingServerType serverData;
    public GameObject prefab;
}

[Serializable]
public class BuildingServerType
{
    public int id;
    public string name;
    public string type;
    public string appearenceVersion = "0000000";
    public int count;
    public int size;
}

#endregion

#region Recipe

public class RecipeModel
{
    public RecipeModel(RecipeServerType serverData, RecipeObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public RecipeServerType serverData;
    public RecipeObject prefab;
}

[Serializable]
public class RecipeServerType
{
    public int id;
    public RecipeItem[] items;
    public string appearenceVersion = "0000000";
    public int resultItemId;
    public int timeForUnit;
}

[Serializable]
public class RecipeItem
{
    public int itemId;
    public int count;
}

#endregion

#region Map

public class MapModel
{
    public MapModel(RecipeServerType serverData, RecipeObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public RecipeServerType serverData;
    public RecipeObject prefab;
}

[Serializable]
public class MapServerType
{
    public MapSector[] sectors;
}

[Serializable]
public class MapSector
{
    public int id;
    public int xPos;
    public int yPos;
    public string environmentName;
}

#endregion
