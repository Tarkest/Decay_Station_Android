using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginVariables {

    public string googleId;

    public LoginVariables(string id)
    {
        googleId = id;
    }
}

[Serializable]
public class AccountData
{
    public string userName;
    public CrewMember driver;
    public MapSector currentMapSector;
    public Locomotive locomotive;
    public Carriage[] carriages;
}
