using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using UnityEngine;
using System;
using System.Threading;

public class EDITOR_ServerData : EditorWindow
{
    #region Render Variables

    private Vector2 _instancesViewPos;
    private GUIContent[] _icons = new GUIContent[1];

    private string _instancesFilter = "";
    private Entities _currentTab;
    private enum Entities
    {
        Constants,
    }

    private string _errorMessage = "";
    private bool _isError = false;

    #region View Switchers

    #region Constants

    private enum ConstantsTypes
    {
        ItemType,
        ItemRarity
    }

    private ConstantsTypes _currentChoosedConstantType;
    private bool _constantTypeChoosed = false;
    private int _choosedConstantIndex = -1;
    private string _newConstantName = "";

    #endregion

    #endregion

    #endregion

    #region Data Buffer

    #region Misc

    private static EDITOR_ServerData _window;

    #endregion

    #region Login

    private bool _isLoggedIn = false;
    private string _token = "";
    private string _loginBuffer = "";
    private string _passwordBuffer = "";

    #endregion

    #region Models

    private ItemRarityModel[] _itemsRarities;
    private ItemTypeModel[] _itemsTypes;

    #endregion

    #endregion

    [MenuItem("Editors/Serverdata")]
    public static void ShowLocomotiveEditorWindow()
    {
        _window = GetWindow<EDITOR_ServerData>("Server Data");
        _window.LoadResouces();
    }

    public void LoadResouces()
    {
        _icons[0] = new GUIContent((Texture)EditorGUIUtility.Load("icons/constants.png"), "Constants");
        //_icons[1] = new GUIContent((Texture)EditorGUIUtility.Load("icons/carriage.png"), "Carriages");
        //_icons[2] = new GUIContent((Texture)EditorGUIUtility.Load("icons/item.png"), "Items");
        //_icons[3] = new GUIContent((Texture)EditorGUIUtility.Load("icons/building.png"), "Buildings");
        //_icons[4] = new GUIContent((Texture)EditorGUIUtility.Load("icons/recipe.png"), "Recipes");
        //_icons[5] = new GUIContent((Texture)EditorGUIUtility.Load("icons/map.png"), "Map");
        //_icons[6] = new GUIContent((Texture)EditorGUIUtility.Load("icons/constants.png"), "Constants");
    }

    void OnGUI()
    {
        if(_isError)
        {
            RenderError();
        }
        else
        {
            if (_isLoggedIn)
            {
                _currentTab = (Entities)GUILayout.Toolbar((int)_currentTab, _icons);
            }
            else
            {
                RenderLogin();
            }
        }
    }

    #region RendersMethods

    #region Misc

    private void RenderError()
    {
        GUILayout.FlexibleSpace();
        BeginVertical("box");
        HelpBox(_errorMessage, MessageType.Error);
        if (GUILayout.Button("Ok"))
        {
            _errorMessage = "";
            _isError = false;
        }
        EndVertical();
        GUILayout.FlexibleSpace();
    }

    private void RenderLogin()
    {
        GUILayout.FlexibleSpace();
        BeginVertical("box");
        LabelField("Admin login");
        _loginBuffer = TextField(_loginBuffer);
        LabelField("Admin password");
        _passwordBuffer = PasswordField(_passwordBuffer);
        Space();
        if(GUILayout.Button("Login"))
        {
            Login(_loginBuffer, _passwordBuffer);
        }
        if(GUILayout.Button("Cancel"))
        {
            _window.Close();
        }
        EndVertical();
        GUILayout.FlexibleSpace();
    }

    private void RenderView()
    {
        BeginScrollView(_instancesViewPos);
        switch(_currentTab)
        {
            case Entities.Constants:
                ConstantsTypesView();
                break;
        }
        EndScrollView();
    }

    #endregion

    #region Constants

    private void ConstantsTypesView()
    {
        if(_constantTypeChoosed)
        {
            ConstantsTypeView();
        }
        else
        {
            BeginVertical("box");
            if (GUILayout.Button("Items types"))
            {
                _currentChoosedConstantType = ConstantsTypes.ItemType;
                _constantTypeChoosed = true;
            }
            if (GUILayout.Button("Items rarities"))
            {
                _currentChoosedConstantType = ConstantsTypes.ItemRarity;
                _constantTypeChoosed = true;
            }
            EndVertical();
        }
    }

    private void ConstantsTypeView()
    {
        BeginVertical("box");
        switch(_currentChoosedConstantType)
        {
            case ConstantsTypes.ItemRarity:
                for (int constantIndex = 0; constantIndex < _itemsRarities.Length; constantIndex++)
                {
                    BeginHorizontal();
                    if(_choosedConstantIndex == constantIndex)
                    {
                        GUI.color = Color.cyan;
                    }
                    if(GUILayout.Button(_itemsRarities[constantIndex].serverData.name))
                    {
                        AssetDatabase.OpenAsset(_itemsRarities[constantIndex].prefab);
                        _choosedConstantIndex = constantIndex;
                    }
                    if (_choosedConstantIndex == constantIndex)
                    {
                        GUI.color = Color.white;
                    }
                    if(GUILayout.Button(new GUIContent("X", "Delete " + _itemsRarities[constantIndex].serverData.name + " rarity")))
                    {
                        DeleteItemsRarity(_itemsRarities[constantIndex].serverData.id);
                    }
                    EndHorizontal();
                }
                GUILayout.Space(120);
                LabelField("Create new item rarity");
                BeginHorizontal();
                _newConstantName = TextField(_newConstantName);
                if(GUILayout.Button(new GUIContent("Add", "Add " + _newConstantName + " to " + _currentChoosedConstantType.ToString())))
                {
                    CreateItemsRarity(_newConstantName);
                }
                EndHorizontal();
                break;

            case ConstantsTypes.ItemType:
                for (int constantIndex = 0; constantIndex < _itemsTypes.Length; constantIndex++)
                {
                    BeginHorizontal();
                    if (_choosedConstantIndex == constantIndex)
                    {
                        GUI.color = Color.cyan;
                    }
                    if (GUILayout.Button(_itemsTypes[constantIndex].serverData.name))
                    {
                        AssetDatabase.OpenAsset(_itemsTypes[constantIndex].prefab);
                        _choosedConstantIndex = constantIndex;
                    }
                    if (_choosedConstantIndex == constantIndex)
                    {
                        GUI.color = Color.white;
                    }
                    if (GUILayout.Button(new GUIContent("X", "Delete " + _itemsTypes[constantIndex].serverData.name + " type")))
                    {
                        DeleteItemsTypes(_itemsTypes[constantIndex].serverData.id);
                    }
                    EndHorizontal();
                }
                GUILayout.Space(120);
                LabelField("Create new item rarity");
                BeginHorizontal();
                _newConstantName = TextField(_newConstantName);
                if (GUILayout.Button(new GUIContent("Add", "Add " + _newConstantName + " to " + _currentChoosedConstantType.ToString())))
                {
                    CreateItemsType(_newConstantName);
                }
                EndHorizontal();
                break;
        }
        if(GUILayout.Button("Back"))
        {
            _constantTypeChoosed = true;
        }
        EndHorizontal();
    }

    #endregion

    #region Locomotive

    #endregion

    #region Carriage

    #endregion

    #region Item

    #endregion

    #endregion

    #region Network Methods

    #region Misc

    private void Login(string login, string password)
    {
        EDITOR_Untility.POST("login", JSON.ToJSON(new AccountData(login, password)), LoginCallback, _token);
    }

    #endregion

    #region Constants

    private void GetItemsTypes()
    {
        EDITOR_Untility.GET("constants/itemstypes", GetItemsTypeCallback, _token);
    }

    private void GetItemsRarity()
    {
        EDITOR_Untility.GET("constants/itemsrarities", GetItemsRarityCallback, _token);
    }

    private void CreateItemsType(string name)
    {
        EDITOR_Untility.POST("constants/itemstypes", JSON.ToJSON(new NewEntity(name)), CreateItemsTypeCallback, _token);
    }

    private void CreateItemsRarity(string name)
    {
        EDITOR_Untility.POST("constants/itemsrarities", JSON.ToJSON(new NewEntity(name)), CreateItemsRarityCallback, _token);
    }

    private void DeleteItemsTypes(int id)
    {
        EDITOR_Untility.DELETE("constants/itemstypes", id, DeleteItemsTypeCallback, _token);
    }

    private void DeleteItemsRarity(int id)
    {
        EDITOR_Untility.DELETE("constants/itemsrarities", id, DeleteItemsRarityCallback, _token);
    }

    #endregion

    #region Locomotive

    #endregion

    #region Carriage

    #endregion

    #region Item

    #endregion

    #endregion

    #region Network CallBacks

    #region Misc

    private void LoginCallback(Responce res)
    {
        _window.Focus();
        if(res.error == null)
        {
            _loginBuffer = "";
            _passwordBuffer = "";
            _token = res.data;
            _isLoggedIn = true;
        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    #endregion

    #region Constants

    private void GetItemsTypeCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {
            _itemsTypes = AssociateModels<ItemTypeModel>(
                JSON.FromJSONArray<ConstantsServerData>(res.data),
                (from _obj
                 in Resources.LoadAll("UI/UIData/ItemsTypesData")
                 let _typeObject = (ItemsTypesData)_obj
                 select _typeObject
                ).ToArray()
                 );
        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    private void GetItemsRarityCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {

        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    private void CreateItemsTypeCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {
            _newConstantName = "";
        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    private void CreateItemsRarityCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {
            _newConstantName = "";
        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    private void DeleteItemsTypeCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {
            
        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    private void DeleteItemsRarityCallback(Responce res)
    {
        _window.Focus();
        if (res.error == null)
        {

        }
        else
        {
            _errorMessage = res.error;
            _isError = true;
        }
    }

    #endregion

    #region Locomotive

    #endregion

    #region Carriage

    #endregion

    #region Item

    #endregion

    #endregion

    #region AssociationMethods

    private T[] AssociateModels<T>(ConstantsServerData[] serverData, ScriptableObject[] prefabsList) where T : class
    {
        T[] _buffer = new T[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if (serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = (T)Activator.CreateInstance(typeof(T), serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (ScriptableObject prefab in prefabsList)
        {
            if (prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = (T)Activator.CreateInstance(typeof(T), null, prefab);
            }
        }
        foreach (ConstantsServerData sData in serverData)
        {
            if (sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = (T)Activator.CreateInstance(typeof(T), sData, null);
            }
        }
        return _buffer;
    }

    private ItemRarityModel[] AssociateModels(ConstantsServerData[] serverData, ItemsRaritiesData[] prefabsList)
    {
        ItemRarityModel[] _buffer = new ItemRarityModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if (serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = new ItemRarityModel(serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (ItemsRaritiesData prefab in prefabsList)
        {
            if (prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new ItemRarityModel(null, prefab);
            }
        }
        foreach (ConstantsServerData sData in serverData)
        {
            if (sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new ItemRarityModel(sData, null);
            }
        }
        return _buffer;
    }

    private LocomotiveModel[] AssociateModels(LocomotiveServerType[] serverData, GameObject[] prefabsList)
    {
        LocomotiveModel[] _buffer = new LocomotiveModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if(serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = new LocomotiveModel(serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach(GameObject prefab in prefabsList)
        {
            if(prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new LocomotiveModel(null, prefab);
            }
        }
        foreach (LocomotiveServerType sData in serverData)
        {
            if(sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new LocomotiveModel(sData, null);
            }
        }
        return _buffer;
    }

    private CarriageModel[] AssociateModels(CarriageServerType[] serverData, GameObject[] prefabsList)
    {
        CarriageModel[] _buffer = new CarriageModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if (serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = new CarriageModel(serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (GameObject prefab in prefabsList)
        {
            if (prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new CarriageModel(null, prefab);
            }
        }
        foreach (CarriageServerType sData in serverData)
        {
            if (sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new CarriageModel(sData, null);
            }
        }
        return _buffer;
    }

    private ItemModel[] AssociateModels(ItemServerType[] serverData, ItemObject[] prefabsList)
    {
        ItemModel[] _buffer = new ItemModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
            {
                if(serverData[dataIndex] != null)
                {
                    if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                    {
                        Array.Resize(ref _buffer, _buffer.Length + 1);
                        _buffer[_buffer.Length - 1] = new ItemModel(serverData[dataIndex], prefabsList[prefabIndex]);
                        prefabsList[prefabIndex] = null;
                        serverData[dataIndex] = null;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        foreach (ItemObject prefab in prefabsList)
        {
            if (prefab != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new ItemModel(null, prefab);
            }
        }
        foreach (ItemServerType sData in serverData)
        {
            if (sData != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + 1);
                _buffer[_buffer.Length - 1] = new ItemModel(sData, null);
            }
        }
        return _buffer;
    }

    #endregion

    #region MiscMethods

    private void ClearValues()
    {
        _isLoggedIn = false;
        _token = "";
        _loginBuffer = "";
        _passwordBuffer = "";
        _errorMessage = "";
        _isError = false;
    }   

    #endregion
}

#region Models

#region Misc

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

public class ItemRarityModel
{
    public ItemsRaritiesData prefab;
    public ConstantsServerData serverData;

    public ItemRarityModel(ConstantsServerData serverData, ScriptableObject prefab)
    {
        this.prefab = (ItemsRaritiesData)prefab;
        this.serverData = serverData;
    }
}

public class ItemTypeModel
{
    public ItemsTypesData prefab;
    public ConstantsServerData serverData;

    public ItemTypeModel(ConstantsServerData serverData, ScriptableObject prefab)
    {
        this.prefab = (ItemsTypesData)prefab;
        this.serverData = serverData;
    }
}

public class BuildingTypeModel
{
    public BuildingTypeModel()
    {

    }
}

[Serializable]
public class ConstantsServerData
{
    public int id;
    public string name;
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
    public ItemModel(ItemServerType serverData, ItemObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public ItemServerType serverData;
    public ItemObject prefab;
}

[Serializable]
public class ItemServerType
{
    public int id;
    public string name;
    public int maxCount;
    public int type;
    public string appearenceVersion = "0000000";
    public int count;
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

#endregion
