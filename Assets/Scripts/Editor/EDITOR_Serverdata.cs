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

    private Vector2 instancesViewPos;

    private string _instancesFilter = "";
    private int _selectedLocomotiveInstanceIndex = -1;
    private int _selectedCarriageInstanceIndex = -1;
    private int _selectedItemInstanceIndex = -1;

    private string _newName = "";
    private bool _locomotiveCreateMode = false;
    private bool _carriageCreateMode = false;
    private bool _itemsCreateMode = false;

    private bool _locomotiveLoaded = false;
    private bool _carriagesLoaded = false;
    private bool _itemsLoaded = false;

    private string _login = "";
    private string _password = "";

    private string _authToken = null;

    private bool _isError = false;
    private string _errorMessage = "";

    private int _currentSelectedModels = 0;
    private int _lastSelectedModels = 0;

    #endregion

    #region Data Buffer

    private GameObject _instanceBuffer;

    private LocomotiveModel[] _localLocomotiveData;
    private LocomotiveModel[] _localLocomotiveDataFiltered;
    private GameObject[] _locomotivePrefabsBuffer;
    private LocomotiveServerType[] _locomotiveServerDataBuffer;

    private CarriageModel[] _localCarriageData;
    private CarriageModel[] _localCarriageDataFiltered;
    private GameObject[] _carriagePrefabsBuffer;
    private CarriageServerType[] _carriageServerDataBuffer;

    private ItemModel[] _localItemsData;
    private ItemModel[] _localItemsDataFiltered;
    private GameObject[] _itemsPrefabsBuffer;
    private ItemServerType[] _itemsServerDataBuffer;

    #endregion

    private string TestLocoJson = "{\"items\":[{\"name\":\"Coffin\",\"appearenceVersion\":\"0000001\"}]}";
    private string TestCarriageJson = "{\"items\":[{\"name\":\"passenger\",\"appearenceVersion\":\"0000001\"}]}";

    [MenuItem("Editors/Serverdata")]
    public static void ShowLocomotiveEditorWindow()
    {
        GetWindow<EDITOR_ServerData>("Server Data");
    }

    void OnGUI()
    {
        BeginVertical("box");
        try
        {
            if (!_isError)
            {
                if (_authToken != null)
                {
                    _currentSelectedModels = GUILayout.Toolbar(_currentSelectedModels, new string[] { "Locomotives", "Carriages", "Items", "Buildings", "Recipe", "Map" });
                    switch(_currentSelectedModels)
                    {
                        case 0:
                            if(_lastSelectedModels != _currentSelectedModels)
                            {
                                BeginLoadLocomotives();
                            }
                            RenderLocomotives();
                        break;

                        case 1:
                            if (_lastSelectedModels != _currentSelectedModels)
                            {
                                BeginLoadCarriages();
                            }
                            RenderCarriages();
                        break;

                        case 2:
                            RenderItems();
                        break;

                        case 3:
                            RenderBuildings();
                        break;

                        case 4:
                            RenderNPC();
                        break;

                        case 5:
                            RenderMap();
                        break;
                    }
                    _lastSelectedModels = _currentSelectedModels;
                }
                else
                {
                    LabelField("Dev login");
                    _login = TextField(_login);
                    LabelField("Dev password");
                    _password = PasswordField(_password);
                    if(GUILayout.Button("Login"))
                    {
                        _login = "";
                        _password = "";
                        LoginCallback("123", null);
                        //EDITOR_Untility.GET("Login route", LoginCallback, "");
                    }
                }
            }
            else
            {
                HelpBox(_errorMessage, MessageType.Error);
            }
        }
        catch(Exception e)
        {
            _isError = true;
            _errorMessage = "Prefab unmatch exeption.\nLooks like you have an old client version, please get last version of a client from github";
            Debug.Log(e);
        }
        EndVertical();
    }

    #region RendersMethods
    
    private void RenderLocomotives()
    {
        if(!_locomotiveCreateMode)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Locomotives route", GetLocomotiveCallback, _authToken);
            }
        }
        if (_locomotiveLoaded)
        {
            if (_locomotiveCreateMode)
            {
                LabelField("Name of new locomotive");
                _newName = TextArea(_newName);
                if (GUILayout.Button("Create"))
                {
                    _instanceBuffer = new GameObject(_newName, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(EdgeCollider2D), typeof(LocomotiveAgent));
                    GameObject _foreground = new GameObject("Foreground", typeof(SpriteRenderer), typeof(LocomotiveForeground));
                    GameObject _wheels1 = new GameObject("Wheel 1", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
                    GameObject _wheels2 = new GameObject("Wheel 2", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
                    _foreground.transform.SetParent(_instanceBuffer.transform);
                    _wheels1.transform.SetParent(_instanceBuffer.transform);
                    _wheels2.transform.SetParent(_instanceBuffer.transform);
                    _foreground.transform.SetSiblingIndex(0);
                    _wheels1.transform.SetSiblingIndex(1);
                    _wheels2.transform.SetSiblingIndex(2);
                    //EDITOR_Untility.POST("Get Locomotives route", JsonUtility.ToJson(new LocomotiveType()), LocomotiveCreateCallBack, _authToken);
                }
                if (GUILayout.Button("Cancel"))
                {
                    _locomotiveCreateMode = false;
                    _newName = "";
                }
            }
            else
            {
                _instancesFilter = TextField(_instancesFilter);
                instancesViewPos = BeginScrollView(instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                if (_instancesFilter.Length > 0)
                {
                    _localLocomotiveDataFiltered = (from _obj in _localLocomotiveData where _obj.prefab.name.ToLower().Contains(_instancesFilter.ToLower()) select _obj).ToArray();
                    for (int _locoIndex = 0; _locoIndex < _localLocomotiveDataFiltered.Length; _locoIndex++)
                    {
                        BeginVertical("box");
                        if (GUILayout.Button(_localLocomotiveDataFiltered[_locoIndex].prefab.name))
                        {
                            _selectedLocomotiveInstanceIndex = Array.IndexOf(_localLocomotiveData, _localLocomotiveDataFiltered[_locoIndex]);
                            AssetDatabase.OpenAsset(_localLocomotiveData[_selectedLocomotiveInstanceIndex].prefab);
                        }
                        if (_selectedLocomotiveInstanceIndex == Array.IndexOf(_localLocomotiveData, _localLocomotiveDataFiltered[_locoIndex]))
                        {
                            DrawOpenInfo(_localLocomotiveDataFiltered[_locoIndex]);
                        }
                        EndVertical();
                    }
                }
                else
                {
                    for (int _locoIndex = 0; _locoIndex < _localLocomotiveData.Length; _locoIndex++)
                    {
                        if (_localLocomotiveData[_locoIndex].serverData != null && _localLocomotiveData[_locoIndex].prefab == null)
                        {
                            throw (new Exception("0"));
                        }
                        if (_localLocomotiveData[_locoIndex] != null)
                        {
                            BeginVertical("box");
                            if (GUILayout.Button(_localLocomotiveData[_locoIndex].prefab.name))
                            {
                                _selectedLocomotiveInstanceIndex = _locoIndex;
                                AssetDatabase.OpenAsset(_localLocomotiveData[_selectedLocomotiveInstanceIndex].prefab);
                            }
                            if (_selectedLocomotiveInstanceIndex == _locoIndex)
                            {
                                DrawOpenInfo(_localLocomotiveData[_selectedLocomotiveInstanceIndex]);
                            }
                            EndVertical();
                        }
                        else
                        {
                            BeginLoadLocomotives();
                        }
                    }
                }
                EndScrollView();
                if (GUILayout.Button(new GUIContent("Create instance", "Create new local locomotive instence")))
                {
                    _locomotiveCreateMode = true;
                }
            }
        }
        else
        {
            LabelField("Loading...");
        }
    }

    private void RenderCarriages()
    {
        if(!_carriageCreateMode)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
            }
        }
        if (_carriagesLoaded)
        {
            if (_carriageCreateMode)
            {
                LabelField("Name of new carriage");
                _newName = TextArea(_newName);
                if (GUILayout.Button("Create"))
                {
                    _instanceBuffer = new GameObject(_newName, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(EdgeCollider2D), typeof(EdgeCollider2D), typeof(CarriageAgent));
                    GameObject _topLayer = new GameObject("Top Layer", typeof(SpriteRenderer));
                    GameObject _skirt = new GameObject("Skirt", typeof(SpriteRenderer));
                    GameObject _wheels1 = new GameObject("Wheel 1", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
                    GameObject _wheels2 = new GameObject("Wheel 2", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
                    _topLayer.transform.SetParent(_instanceBuffer.transform);
                    _skirt.transform.SetParent(_instanceBuffer.transform);
                    _wheels1.transform.SetParent(_instanceBuffer.transform);
                    _wheels2.transform.SetParent(_instanceBuffer.transform);
                    _topLayer.transform.SetSiblingIndex(0);
                    _skirt.transform.SetSiblingIndex(1);
                    _wheels1.transform.SetSiblingIndex(2);
                    _wheels2.transform.SetSiblingIndex(3);
                    //EDITOR_Untility.POST("Get Locomotives route", JsonUtility.ToJson(new LocomotiveType()), GetLocomotiveCallback, _authToken);
                }
                if (GUILayout.Button("Cancel"))
                {
                    _carriageCreateMode = false;
                    _newName = "";
                }
            }
            else
            {
                _instancesFilter = TextField(_instancesFilter);
                instancesViewPos = BeginScrollView(instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                if (_instancesFilter.Length > 0)
                {
                    _localCarriageDataFiltered = (from _obj in _localCarriageData where _obj.prefab.name.ToLower().Contains(_instancesFilter.ToLower()) select _obj).ToArray();
                    for (int _carIndex = 0; _carIndex < _localCarriageDataFiltered.Length; _carIndex++)
                    {
                        BeginVertical("box");
                        if (GUILayout.Button(_localCarriageDataFiltered[_carIndex].prefab.name))
                        {
                            _selectedCarriageInstanceIndex = Array.IndexOf(_localCarriageData, _localCarriageDataFiltered[_carIndex]);
                            AssetDatabase.OpenAsset(_localCarriageData[_selectedCarriageInstanceIndex].prefab);
                        }
                        if (_selectedCarriageInstanceIndex == Array.IndexOf(_localCarriageData, _localCarriageDataFiltered[_carIndex]))
                        {
                            DrawOpenInfo(_localCarriageDataFiltered[_carIndex]);
                        }
                        EndVertical();
                    }
                }
                else
                {
                    for (int _carIndex = 0; _carIndex < _localCarriageData.Length; _carIndex++)
                    {
                        if (_localCarriageData[_carIndex].serverData != null && _localCarriageData[_carIndex].prefab == null)
                        {
                            throw (new Exception("0"));
                        }
                        if (_localCarriageData[_carIndex] != null)
                        {
                            BeginVertical("box");
                            if (GUILayout.Button(_localCarriageData[_carIndex].prefab.name))
                            {
                                _selectedCarriageInstanceIndex = _carIndex;
                                AssetDatabase.OpenAsset(_localCarriageData[_selectedCarriageInstanceIndex].prefab);
                            }
                            if (_selectedCarriageInstanceIndex == _carIndex)
                            {
                                DrawOpenInfo(_localCarriageData[_selectedCarriageInstanceIndex]);
                            }
                            EndVertical();
                        }
                        else
                        {
                            BeginLoadLocomotives();
                        }
                    }
                }
                EndScrollView();
                if (GUILayout.Button(new GUIContent("Create instance", "Create new local locomotive instence")))
                {
                    _carriageCreateMode = true;
                }
            }
        }
        else
        {
            LabelField("Loading...");
        }
    }

    private void RenderItems()
    {
        if (!_carriageCreateMode)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
            }
        }
    }

    private void RenderBuildings()
    {
        LabelField("Buildings");
    }

    private void RenderRecipes()
    {
        LabelField("NPC");
    }

    private void RenderMap()
    {
        LabelField("Map");
    }

    #endregion

    #region DrawMethods

    private void DrawOpenInfo(LocomotiveModel model)
    {
        BeginVertical("box");
        GUIStyle _statusStyle = new GUIStyle();
        _statusStyle.richText = true;
        BeginHorizontal();
        LabelField(new GUIContent($"Server status: <color={ (model.serverData != null ? "#009900ff" : "#cf0000ff") }>{ (model.serverData != null ? "Loaded" : "Not Loaded") }</color>", "Locomotive state on server batabase"), _statusStyle, GUILayout.Width(85));
        EndHorizontal();
        LabelField("Associated with");
        if (model.serverData.appearenceVersion.Length < 7)
        {
            model.serverData.appearenceVersion += "0";
        }
        if (model.serverData.appearenceVersion.Length > 7)
        {
            model.serverData.appearenceVersion = model.serverData.appearenceVersion.Remove(6);
        }
        model.serverData.appearenceVersion = TextField(model.serverData.appearenceVersion.Insert(2, ".").Insert(5, ".")).Replace(".", "");
        LabelField(model.serverData.count + " instances is currently on server");
        if(GUILayout.Button("Edit upgrades recipes"))
        {
            Debug.Log("Recipe editor");
        }
        BeginHorizontal();
        if (GUILayout.Button(new GUIContent("UPD", "Update locomotive instance on server")))
        {
            Debug.Log("UPD");
        }
        if (GUILayout.Button(new GUIContent("REM", "Remove locomotive instance from server rotation")))
        {
            Debug.Log("REM");
        }
        if (GUILayout.Button(new GUIContent("DEL", "Delete locomotive instance from server and client")))
        {
            if (model.serverData.count > 0)
            {
                Debug.Log("Cant delete object");
            }
            else
            {
                Debug.Log("Deleted");
            }
        }
        EndHorizontal();
        EndVertical();
    }

    private void DrawOpenInfo(CarriageModel model)
    {
        BeginVertical("box");
        GUIStyle _statusStyle = new GUIStyle();
        _statusStyle.richText = true;
        BeginHorizontal();
        LabelField(new GUIContent($"Server status: <color={ (model.serverData != null ? "#009900ff" : "#cf0000ff") }>{ (model.serverData != null ? "Loaded" : "Not Loaded") }</color>", "Locomotive state on server batabase"), _statusStyle, GUILayout.Width(85));
        EndHorizontal();
        LabelField("Associated with");
        if (model.serverData.appearenceVersion.Length < 7)
        {
            model.serverData.appearenceVersion += "0";
        }
        if (model.serverData.appearenceVersion.Length > 7)
        {
            model.serverData.appearenceVersion = model.serverData.appearenceVersion.Remove(6);
        }
        model.serverData.appearenceVersion = TextField(model.serverData.appearenceVersion.Insert(2, ".").Insert(5, ".")).Replace(".", "");
        LabelField(model.serverData.count + " instances is currently on server");
        if (GUILayout.Button("Edit assembly recipe"))
        {
            Debug.Log("Recipe editor");
        }
        BeginHorizontal();
        if (GUILayout.Button(new GUIContent("UPD", "Update locomotive instance on server")))
        {
            Debug.Log("UPD");
        }
        if (GUILayout.Button(new GUIContent("REM", "Remove locomotive instance from server rotation")))
        {
            Debug.Log("REM");
        }
        if (GUILayout.Button(new GUIContent("DEL", "Delete locomotive instance from server and client")))
        {
            if (model.serverData.count > 0)
            {
                Debug.Log("Cant delete object");
            }
            else
            {
                Debug.Log("Deleted");
            }
        }
        EndHorizontal();
        EndVertical();
    }

    #endregion

    #region Network Methods

    private void BeginLoadLocomotives()
    {
        _selectedLocomotiveInstanceIndex = -1;
        _localLocomotiveData = new LocomotiveModel[0];
        _localLocomotiveDataFiltered = new LocomotiveModel[0];
        _locomotivePrefabsBuffer = null;
        _locomotiveLoaded = false;
        Thread.Sleep(2000);
        GetLocomotiveCallback(TestLocoJson, null);
        //EDITOR_Untility.GET("Get Locomotives route", GetLocomotiveCallback, _authToken);
    }

    private void BeginLoadCarriages()
    {
        _selectedCarriageInstanceIndex = -1;
        _localCarriageData = new CarriageModel[0];
        _localCarriageDataFiltered = new CarriageModel[0];
        _carriagePrefabsBuffer = null;
        _carriagesLoaded = false;
        Thread.Sleep(2000);
        GetCarriageCallback(TestCarriageJson, null);
        //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
    }

    private void BeginLoadItems()
    {
        _selectedItemInstanceIndex = -1;
        _localItemsData = new ItemModel[0];
        _localItemsDataFiltered = new ItemModel[0];
        _carriagePrefabsBuffer = null;
        _carriagesLoaded = false;
        Thread.Sleep(2000);
        GetCarriageCallback(TestCarriageJson, null);
        //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
    }

    #endregion

    #region Network CallBacks

    private void LoginCallback(string data, string error)
    {
        if (error == null)
        {
            _authToken = data;
            BeginLoadLocomotives();
        }
        else
        {
            Debug.LogError("Wrong Credentials");
        }
    }

    #region Locomotive

    private void GetLocomotiveCallback(string data, string error)
    {
        if(error == null)
        {
            _selectedLocomotiveInstanceIndex = -1;
            _locomotivePrefabsBuffer = (from _obj in Resources.LoadAll("Locomotive/Instances") let _locoObj = (GameObject)_obj select _locoObj).ToArray();
            _locomotiveServerDataBuffer = JSON.FromJSONArray<LocomotiveServerType>(data);
            _localLocomotiveData = AssociateModels(_locomotiveServerDataBuffer, _locomotivePrefabsBuffer);
            _locomotiveLoaded = true;
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    private void LocomotiveCreateCallBack(string data, string error)
    {
        if (error == null)
        {
            PrefabUtility.SaveAsPrefabAsset(_instanceBuffer, "Assets/Resources/Locomotive/Instances/" + _newName + ".prefab");
            _locomotivePrefabsBuffer = (from _obj in Resources.LoadAll("Locomotive/Instances") let _locoObj = (GameObject)_obj select _locoObj).ToArray();
            Array.Resize(ref _locomotiveServerDataBuffer, _locomotiveServerDataBuffer.Length + 1);
            _locomotiveServerDataBuffer[_locomotiveServerDataBuffer.Length - 1] = JSON.FromJSON<LocomotiveServerType>(data);
            _localLocomotiveData = AssociateModels(_locomotiveServerDataBuffer, _locomotivePrefabsBuffer);
            _selectedLocomotiveInstanceIndex = Array.IndexOf(_localLocomotiveData, (from _newObj in _localLocomotiveData where _newObj.prefab.name == _newName select _newObj).ToArray()[0]);
            DestroyImmediate(_instanceBuffer);
            AssetDatabase.OpenAsset(_localLocomotiveData[_selectedLocomotiveInstanceIndex].prefab);
            _locomotiveLoaded = true;
            _locomotiveCreateMode = false;
        }
        else
        {
            _locomotiveLoaded = false;
            _locomotiveCreateMode = false;
            _isError = true;
            _errorMessage = error;
        }  
    }

    #endregion

    #region Carriage

    private void GetCarriageCallback(string data, string error)
    {
        if (error == null)
        {
            _selectedCarriageInstanceIndex = -1;
            _carriagePrefabsBuffer = (from _obj in Resources.LoadAll("Carriages/Instances") let _locoObj = (GameObject)_obj select _locoObj).ToArray();
            _carriageServerDataBuffer = JSON.FromJSONArray<CarriageServerType>(data);
            _localCarriageData = AssociateModels(_carriageServerDataBuffer, _carriagePrefabsBuffer);
            _carriagesLoaded = true;
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    #endregion

    #region Item\]
    private void GetItemsCallback(string data, string error)
    {
        if (error == null)
        {
            _selectedCarriageInstanceIndex = -1;
            _itemsPrefabsBuffer = (from _obj in Resources.LoadAll("Carriages/Instances") let _locoObj = (GameObject)_obj select _locoObj).ToArray();
            _itemsServerDataBuffer = JSON.FromJSONArray<CarriageServerType>(data);
            _localitemData = AssociateModels(_carriageServerDataBuffer, _carriagePrefabsBuffer);
            _carriagesLoaded = true;
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    #endregion

    #endregion

    #region AssociationMethods

    private LocomotiveModel[] AssociateModels(LocomotiveServerType[] serverData, GameObject[] prefabsList)
    {
        LocomotiveModel[] _buffer = new LocomotiveModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
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
                if (prefabsList[prefabIndex].name == serverData[dataIndex].name)
                {
                    Array.Resize(ref _buffer, _buffer.Length + 1);
                    _buffer[_buffer.Length - 1] = new CarriageModel(serverData[dataIndex], prefabsList[prefabIndex]);
                    prefabsList[prefabIndex] = null;
                    serverData[dataIndex] = null;
                    break;
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

    private ItemModel[] AssociateModels(ItemServerType[] serverData, GameObject[] prefabsList)
    {
        ItemModel[] _buffer = new ItemModel[0];
        for (int prefabIndex = 0; prefabIndex < prefabsList.Length; prefabIndex++)
        {
            for (int dataIndex = 0; dataIndex < serverData.Length; dataIndex++)
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
        }
        foreach (GameObject prefab in prefabsList)
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
}

#region Models

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

[System.Serializable]
public class LocomotiveServerType
{
    public int id;
    public string name;
    public string appearenceVersion = "0000000";
    public int count;
    public LocomotiveUpgradeRecipe[] upgradesRecipes;
}

[System.Serializable]
public class LocomotiveUpgradeRecipe
{
    public LocomotiveUpgradeItem[] items;
}

[System.Serializable]
public class LocomotiveUpgradeItem
{
    public int itemID;
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

[System.Serializable]
public class CarriageServerType
{
    public int id;
    public string name;
    public string appearenceVersion = "0000000";
    public int buildingSlotsCount;
    public int count;
    public int storageCapacity;
    public CarriageAssemblyItem[] assemblyItems;
}

[System.Serializable]
public class CarriageAssemblyItem
{
    public int itemID;
    public int count;
}

#endregion

#region Item

public class ItemModel
{
    public ItemModel(ItemServerType serverData, GameObject prefab)
    {
        this.serverData = serverData;
        this.prefab = prefab;
    }

    public ItemServerType serverData;
    public GameObject prefab;
}

[System.Serializable]
public class ItemServerType
{
    public string name;
    public int maxCount;
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

[System.Serializable]
public class BuildingServerType
{
    public string name;
    public string appearenceVersion = "0000000";
    public int count;
}

#endregion

#endregion