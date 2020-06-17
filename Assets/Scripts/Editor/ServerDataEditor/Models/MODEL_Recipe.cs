using System;

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
