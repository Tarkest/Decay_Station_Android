﻿using System;
using UnityEngine;

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
    public int count;
    public int size;
}
