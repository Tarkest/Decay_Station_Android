[System.Serializable]
public class Account
{
    public int id;
    public string nickname;
    public string accountKey;
    public int level;
    public int accountExperience;
    public Zone currentZone;
    public Locomotive[] locomotives;
    public Carriage[] carriages;
    public Character[] characters;

}

[System.Serializable]
public class Zone
{
    public string name;
}

[System.Serializable]
public class Locomotive
{
    public int id;
    public int level;
    public string name;
    public LocomotiveType type;
    public Building[] inner;
    public Building[] outer;
}

[System.Serializable] 
public class LocomotiveType
{
    public string name;
}

[System.Serializable]
public class LocomotiveInventory
{
    public int id;
    public Item[] items;
}

[System.Serializable]
public class Carriage
{
    public int id;
    public int order;
    public CarriageType type;
    public Building[] inner;
    public Building[] outer;
}

[System.Serializable]
public class CarriageType
{
    public string name;
}

[System.Serializable]
public class CarriageInventory
{
    public int id;
    public Item[] items;
}

[System.Serializable]
public class Building
{
    public int id;
    public string name;
    public bool isOuter;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public int strength;
    public int agility;
    public int intelligence;
    public CharacterSpecialization specialization;
    public CharacterParametersExperience paramsExperience;
}

[System.Serializable]
public class CharacterSpecialization
{
    public string name;
}

[System.Serializable]
public class CharacterParametersExperience
{
    public int strength;
    public int agility;
    public int intelligence;
}

[System.Serializable]
public class CharacterInventory
{
    public int id;
    public Item[] items;
}

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public int amount;
    public int cellId;
}

[System.Serializable]
public class Inventories
{
    public LocomotiveInventory[] locomotives;
    public CarriageInventory[] carriages;
    public CharacterInventory[] characters;
}

//Packages for send
[System.Serializable]
public class InventoryReplaceData
{
    public int from;
    public int to;
}
