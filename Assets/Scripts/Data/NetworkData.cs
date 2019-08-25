[System.Serializable]
public class Account
{
    public int id;
    public string nickname;
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
    public CharacterType type;
    public CharacterSpecialization specialization;
    public CharacterParametersExperience paramsExperience;
}

[System.Serializable]
public class CharacterType
{
    public string name;
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
public class NInventory
{
    public int id;
    public NCell[] items;
}

[System.Serializable]
public class NCell
{
    public int id;
    public string name;
    public int amount;
    public int cellId;
}

[System.Serializable]
public class Inventories
{
    public NInventory[] locomotives;
    public NInventory[] carriages;
    public NInventory[] characters;
}

//Packages for send
[System.Serializable]
public class InventoryReplaceData
{
    public int from;
    public int to;
}
