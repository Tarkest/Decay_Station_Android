using System;

public class ServerData
{
    public int id;
    public string name;
}

[Serializable]
public class AccountVariables
{
    public string login;
    public string password;

    public AccountVariables(string login, string password)
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
