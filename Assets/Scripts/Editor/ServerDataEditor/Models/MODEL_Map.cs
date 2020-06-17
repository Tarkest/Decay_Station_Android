using System;

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
