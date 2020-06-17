using System;
using UnityEngine;

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
    public ConstantsServerData(int id)
    {
        this.id = id;
    }
}
