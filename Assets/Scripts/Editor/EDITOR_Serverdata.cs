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
    private GUIContent[] _icons = new GUIContent[3];

    private string _instancesFilter = "";
    private int _selectedLocomotiveInstanceIndex = -1;
    private int _selectedCarriageInstanceIndex = -1;
    private int _selectedItemInstanceIndex = -1;

    private string _newName = "";
    private bool _locomotiveCreateMode = false;
    private bool _locomotiveUpgradesEdit = false;
    private bool _locomotiveUpgradeChoosed = false;
    private bool _locomotiveUpgradeLevelChoosed = false;
    private int _locomotiveUpgradeLevelChoosedIndex = -1;
    private int _locomotiveUpgradeChoosedIndex = -1;

    private bool _carriageCreateMode = false;
    private bool _carriageUpgradesEdit = false;
    private bool _carriageUpgradeChoosed = false;
    private int _carriageUpgradeChoosedIndex = -1;

    private bool _itemsCreateMode = false;

    private bool _locomotiveLoaded = false;
    private bool _carriagesLoaded = false;
    private bool _itemsLoaded = false;

    private string _login = "";
    private string _password = "";

    private string _authToken = "";

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
    private ItemObject[] _itemsPrefabsBuffer;
    private ItemServerType[] _itemsServerDataBuffer;

    #endregion

    private string TestLocoJson = "{\"items\":[{\"id\":\"1\",\"name\":\"Coffin\",\"appearenceVersion\":\"0000001\",\"count\":\"10\",\"upgradesRecipes\":[{\"items\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]},{\"items\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]},{\"items\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]},{\"items\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]},{\"items\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]}]}]}";
    private string TestCarriageJson = "{\"items\":[{\"name\":\"passenger\",\"appearenceVersion\":\"0000001\",\"count\":\"10\",\"storageCapacity\":\"25\", \"assemblyItems\":[{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"},{\"itemId\":\"1\",\"count\":\"1\"}]}]}";
    private string TestItemsJson = "{\"items\":[{\"id\":\"1\",\"name\":\"Test\",\"appearenceVersion\":\"0000001\",\"count\":\"1\",\"maxCount\":\"10\",\"type\":\"0\"},{\"id\":\"2\",\"name\":\"Test2\",\"appearenceVersion\":\"0000001\",\"count\":\"1\",\"maxCount\":\"50\",\"type\":\"0\"}]}";

    [MenuItem("Editors/Serverdata")]
    public static void ShowLocomotiveEditorWindow()
    {
        EDITOR_ServerData window = GetWindow<EDITOR_ServerData>("Server Data");
        window.LoadResouces();
    }

    public void LoadResouces()
    {
        _icons[0] = new GUIContent((Texture)EditorGUIUtility.Load("icons/locomotive.png"), "Locomotives");
        _icons[1] = new GUIContent((Texture)EditorGUIUtility.Load("icons/carriage.png"), "Carriages");
        _icons[2] = new GUIContent((Texture)EditorGUIUtility.Load("icons/item.png"), "Items");
        //_icons[3] = new GUIContent((Texture)EditorGUIUtility.Load("icons/building.png"), "Buildings");
        //_icons[4] = new GUIContent((Texture)EditorGUIUtility.Load("icons/recipe.png"), "Recipes");
        //_icons[5] = new GUIContent((Texture)EditorGUIUtility.Load("icons/map.png"), "Map");
        //_icons[6] = new GUIContent((Texture)EditorGUIUtility.Load("icons/constants.png"), "Constants");
    }

    void OnGUI()
    {
        BeginVertical("box");
        try
        {
            if (!_isError)
            {
                if (_authToken != "")
                {
                    _currentSelectedModels = GUILayout.Toolbar(_currentSelectedModels, _icons, GUILayout.Height(20f));
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
                            if (_lastSelectedModels != _currentSelectedModels)
                            {
                                BeginLoadItems();
                            }
                            RenderItems();
                        break;

                        case 3:
                            RenderBuildings();
                        break;

                        case 4:
                            RenderRecipes();
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

    #region Locomotive

    private void RenderLocomotives()
    {
        if(!_locomotiveCreateMode && !_locomotiveUpgradesEdit && !_locomotiveUpgradeChoosed)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Locomotives route", GetLocomotiveCallback, _authToken);
            }
        }
        if(_locomotiveUpgradeChoosed)
        {
            LocomotiveUpgradeItemChooseView();
        }
        else
        {
            if (_locomotiveLoaded)
            {
                if (_locomotiveUpgradeLevelChoosed)
                {
                    LocomotiveUpgradeView();
                }
                else
                {
                    if (_locomotiveUpgradesEdit)
                    {
                        LocomotiveUpgradesListView();
                    }
                    else
                    {
                        if (_locomotiveCreateMode)
                        {
                            LocomotiveCreateView();
                        }
                        else
                        {
                            LocomotiveListView();
                        }
                    }
                }
            }
            else
            {
                LabelField("Loading...");
            }
        }
    }

    private void LocomotiveCreateView()
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

    private void LocomotiveListView()
    {
        _instancesFilter = TextField(_instancesFilter);
        _instancesViewPos = BeginScrollView(_instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
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

    private void LocomotiveUpgradesListView()
    {
        BeginVertical();
        for (int i = 0; i < 5; i++)
        {
            if (GUILayout.Button($"Level {i + 1}"))
            {
                _locomotiveUpgradeLevelChoosed = true;
                _locomotiveUpgradeLevelChoosedIndex = i;
            }
        }
        Space();
        Space();
        if (GUILayout.Button("Back"))
        {
            _locomotiveUpgradesEdit = false;
        }
        EndVertical();
    }

    private void LocomotiveUpgradeView()
    {  
        LocomotiveUpgradeRecipe[] _buffer = _localLocomotiveData[_selectedLocomotiveInstanceIndex].serverData.upgradesRecipes;
        BeginVertical();
        for (int i = 0; i < 8; i++)
        {
            ItemModel _itemBuffer = (from item in _localItemsData where item.serverData.id == _buffer[_locomotiveUpgradeLevelChoosedIndex].items[i].itemId select item).ToArray()[0];
            BeginVertical("box");
            if (GUILayout.Button(_itemBuffer.prefab.name))
            {
                _locomotiveUpgradeChoosed = true;
                _locomotiveUpgradeChoosedIndex = i;
            }
            int _maxCount = _itemBuffer.serverData.maxCount;
            _buffer[_locomotiveUpgradeLevelChoosedIndex].items[i].count = IntSlider(_buffer[_locomotiveUpgradeLevelChoosedIndex].items[i].count, 1, _maxCount);
            EndVertical();
        }
        Space();
        Space();
        if (GUILayout.Button("Back"))
        {
            _locomotiveUpgradeLevelChoosed = false;
            _locomotiveUpgradeLevelChoosedIndex = -1;
        }
        EndVertical();
    }

    private void LocomotiveUpgradeItemChooseView()
    {
        _instancesFilter = TextField(_instancesFilter);
        _instancesViewPos = BeginScrollView(_instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        if(_instancesFilter.Length > 0)
        {
            _localItemsDataFiltered = (from _obj in _localItemsData where _obj.prefab.name.ToLower().Contains(_instancesFilter.ToLower()) select _obj).ToArray();
            for (int _itemIndex = 0; _itemIndex < _localItemsDataFiltered.Length; _itemIndex++)
            {
                BeginVertical();
                if (GUILayout.Button(_localItemsDataFiltered[_itemIndex].prefab.name))
                {
                    _localLocomotiveData[_selectedLocomotiveInstanceIndex].serverData.upgradesRecipes[_locomotiveUpgradeLevelChoosedIndex].items[_locomotiveUpgradeChoosedIndex].itemId = _localItemsDataFiltered[_itemIndex].serverData.id;
                    _locomotiveUpgradeChoosed = false;
                    _locomotiveUpgradeChoosedIndex = -1;
                }
                EndVertical();
            }
        }
        else
        {
            for (int _itemIndex = 0; _itemIndex < _localItemsData.Length; _itemIndex++)
            {
                BeginVertical();
                if (GUILayout.Button(_localItemsData[_itemIndex].prefab.name))
                {
                    _localLocomotiveData[_selectedLocomotiveInstanceIndex].serverData.upgradesRecipes[_locomotiveUpgradeLevelChoosedIndex].items[_locomotiveUpgradeChoosedIndex].itemId = _localItemsData[_itemIndex].serverData.id;
                    _locomotiveUpgradeChoosed = false;
                    _locomotiveUpgradeChoosedIndex = -1;
                }
                EndVertical();
            }
        }
        EndScrollView();
        Space();
        Space();
        if (GUILayout.Button("Back"))
        {
            _locomotiveUpgradeChoosed = false;
            _locomotiveUpgradeChoosedIndex = -1;
        }
    }

    #endregion

    #region Carriage

    private void RenderCarriages()
    {
        if(!_carriageCreateMode && !_carriageUpgradesEdit)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
            }
        }
        if(_carriageUpgradeChoosed)
        {
            CarriageUpgradeItemChooseVIew();
        }
        else
        {
            if(_carriageUpgradesEdit)
            {
                CarriageUpgradeView();
            }
            else
            {
                if (_carriagesLoaded)
                {
                    if (_carriageCreateMode)
                    {
                        CarriageCreateView();
                    }
                    else
                    {
                        CarriageListView();
                    }
                }
                else
                {
                    LabelField("Loading...");
                }
            }
        }
    }

    private void CarriageCreateView()
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

    private void CarriageListView()
    {
        _instancesFilter = TextField(_instancesFilter);
        _instancesViewPos = BeginScrollView(_instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
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

    private void CarriageUpgradeView()
    {
        CarriageAssemblyItem[] _buffer = _localCarriageData[_selectedCarriageInstanceIndex].serverData.assemblyItems;
        BeginVertical();
        for (int i = 0; i < 8; i++)
        {
            ItemModel _itemBuffer = (from item in _localItemsData where item.serverData.id == _buffer[i].itemId select item).First();
            BeginVertical("box");
            if (GUILayout.Button(_itemBuffer.prefab.name))
            {
                _carriageUpgradeChoosed = true;
                _carriageUpgradeChoosedIndex = i;
            }
            int _maxCount = _itemBuffer.serverData.maxCount;
            _buffer[i].count = IntSlider(_buffer[i].count, 1, _maxCount);
            EndVertical();
        }
        Space();
        Space();
        if (GUILayout.Button("Back"))
        {
            _carriageUpgradesEdit = false;
        }
        EndVertical();
    }

    private void CarriageUpgradeItemChooseVIew()
    {
        _instancesFilter = TextField(_instancesFilter);
        _instancesViewPos = BeginScrollView(_instancesViewPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        if (_instancesFilter.Length > 0)
        {
            _localItemsDataFiltered = (from _obj in _localItemsData where _obj.prefab.name.ToLower().Contains(_instancesFilter.ToLower()) select _obj).ToArray();
            for (int _itemIndex = 0; _itemIndex < _localItemsDataFiltered.Length; _itemIndex++)
            {
                BeginVertical();
                if (GUILayout.Button(_localItemsDataFiltered[_itemIndex].prefab.name))
                {
                    _localCarriageData[_selectedCarriageInstanceIndex].serverData.assemblyItems[_carriageUpgradeChoosedIndex].itemId = _localItemsDataFiltered[_itemIndex].serverData.id;
                    _carriageUpgradeChoosed = false;
                    _carriageUpgradeChoosedIndex = -1;
                    
                }
                EndVertical();
            }
        }
        else
        {
            for (int _itemIndex = 0; _itemIndex < _localItemsData.Length; _itemIndex++)
            {
                BeginVertical();
                if (GUILayout.Button(_localItemsData[_itemIndex].prefab.name))
                {
                    _localCarriageData[_selectedCarriageInstanceIndex].serverData.assemblyItems[_carriageUpgradeChoosedIndex].itemId = _localItemsData[_itemIndex].serverData.id;
                    _carriageUpgradeChoosed = false;
                    _carriageUpgradeChoosedIndex = -1;
                }
                EndVertical();
            }
        }
        EndScrollView();
        Space();
        Space();
        if (GUILayout.Button("Back"))
        {
            _carriageUpgradeChoosed = false;
            _carriageUpgradeChoosedIndex = -1;
        }
    }

    #endregion

    #region Item

    private void RenderItems()
    {
        if (!_itemsCreateMode)
        {
            if (GUILayout.Button("Refresh"))
            {
                //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
            }
        }
        if(_itemsLoaded)
        {
            
        }
    }

    private void ItemCreateView()
    {

    }

    private void ItemsListView()
    {

    }

    #endregion



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
            _locomotiveUpgradesEdit = true;
        }
        BeginHorizontal();
        if (GUILayout.Button(new GUIContent("UPD", "Update locomotive instance on server")))
        {
            UpdateLocomotiveModel(model.serverData);
        }
        if (GUILayout.Button(new GUIContent("REM", "Remove locomotive instance from server rotation")))
        {
            RemoveLocomotiveFromRotationModel(model.serverData);
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
        LabelField(new GUIContent($"Server status: <color={ (model.serverData != null ? "#009900ff" : "#cf0000ff") }>{ (model.serverData != null ? "Loaded" : "Not Loaded") }</color>", "Carriage state on server batabase"), _statusStyle, GUILayout.Width(85));
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
        model.serverData.storageCapacity = IntSlider(new GUIContent("Storage", "Capacity of carriage inventory"), model.serverData.storageCapacity, 20, 150);
        if (GUILayout.Button("Edit assembly recipe"))
        {
            _carriageUpgradesEdit = true;
        }
        if(GUILayout.Button("Edit building slots"))
        {

        }
        BeginHorizontal();
        if (GUILayout.Button(new GUIContent("UPD", "Update carriage instance on server")))
        {
            UpdateCarriageModel(model.serverData);
        }
        if (GUILayout.Button(new GUIContent("REM", "Remove carriage instance from server rotation")))
        {
            RemoveCarriageFromRotationModel(model.serverData);
        }
        if (GUILayout.Button(new GUIContent("DEL", "Delete carriage instance from server and client")))
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

    private void DrawOpenInfo(ItemModel model)
    {
        BeginVertical("box");
        GUIStyle _statusStyle = new GUIStyle();
        _statusStyle.richText = true;
        BeginHorizontal();
        LabelField(new GUIContent($"Server status: <color={ (model.serverData != null ? "#009900ff" : "#cf0000ff") }>{ (model.serverData != null ? "Loaded" : "Not Loaded") }</color>", "Item state on server batabase"), _statusStyle, GUILayout.Width(85));
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
        BeginHorizontal();
        if (GUILayout.Button(new GUIContent("UPD", "Update item instance on server")))
        {
            
        }
        if (GUILayout.Button(new GUIContent("REM", "Remove item instance from server rotation")))
        {
            
        }
        if (GUILayout.Button(new GUIContent("DEL", "Delete item instance from server and client")))
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

    #region Locomotive

    private void BeginLoadLocomotives()
    {
        _selectedLocomotiveInstanceIndex = -1;
        _localLocomotiveData = new LocomotiveModel[0];
        _localLocomotiveDataFiltered = new LocomotiveModel[0];
        _localItemsData = new ItemModel[0];
        _localItemsDataFiltered = new ItemModel[0];
        _locomotivePrefabsBuffer = null;
        _locomotiveLoaded = false;
        GetLocomotiveCallback(TestLocoJson, null);
        //EDITOR_Untility.GET("Get Locomotives route", GetLocomotiveCallback, _authToken);
    }

    private void UpdateLocomotiveModel(LocomotiveServerType model)
    {
        LocomotiveRUCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.PUT("UpdateLocomotiveRoute", JSON.ToJSON(model), LocomotiveRUDCallback, _authToken);
    }

    private void RemoveLocomotiveFromRotationModel(LocomotiveServerType model)
    {
        LocomotiveRUCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.PUT("LocomotiveRemoveFromRotationRoute", JSON.ToJSON(model), LocomotiveRUDCallback, _authToken);
    }

    private void DeleteLocomotiveModel(LocomotiveServerType model)
    {
        LocomotiveDeleteCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.DELETE("Delete locomotive model route", LocomotiveDeleteCallback, _authToken);
    }

    #endregion

    #region Carriage

    private void BeginLoadCarriages()
    {
        _selectedCarriageInstanceIndex = -1;
        _localCarriageData = new CarriageModel[0];
        _localCarriageDataFiltered = new CarriageModel[0];
        _carriagePrefabsBuffer = null;
        _carriagesLoaded = false;
        GetCarriageCallback(TestCarriageJson, null);
        //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
    }

    private void UpdateCarriageModel(CarriageServerType model)
    {
        CarriageRUCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.PUT("UpdateCarriageRoute", JSON.ToJSON(model), CarriageRUCallback, _authToken);
    }

    private void RemoveCarriageFromRotationModel(CarriageServerType model)
    {
        CarriageRUCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.PUT("CarriageRemoveFromRotationRoute", JSON.ToJSON(model), CarriageRUCallback, _authToken);
    }

    private void DeleteCarriageModel(CarriageServerType model)
    {
        CarriageDeleteCallback(JSON.ToJSON(model), null);
        //EDITOR_Untility.DELETE("Delete Carriage route", CarriageDeleteCallback, _authToken);
    }

    #endregion

    #region Item

    private void BeginLoadItems()
    {
        _selectedItemInstanceIndex = -1;
        _localItemsData = new ItemModel[0];
        _localItemsDataFiltered = new ItemModel[0];
        _carriagePrefabsBuffer = null;
        _carriagesLoaded = false;
        GetItemsCallback(TestItemsJson, null);
        //EDITOR_Untility.GET("Get Carriages route", GetCarriageCallback, _authToken);
    }

    #endregion

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
            //EDITOR_Untility.GET("Get Items route", GetItemsAfterLocomotiveLoadedCallback, _authToken);
            //For Test
            GetItemsAfterLocomotiveLoadedCallback(TestItemsJson, null);
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    private void GetItemsAfterLocomotiveLoadedCallback(string data, string error)
    {
        if (error == null)
        {
            _itemsPrefabsBuffer = (from _obj in Resources.LoadAll("Items") let _itemObj = (ItemObject)_obj select _itemObj).ToArray();
            _itemsServerDataBuffer = JSON.FromJSONArray<ItemServerType>(data);
            _localItemsData = AssociateModels(_itemsServerDataBuffer, _itemsPrefabsBuffer);
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
            _selectedLocomotiveInstanceIndex = Array.IndexOf(_localLocomotiveData, (from _newObj in _localLocomotiveData where _newObj.prefab.name == _newName select _newObj).First());
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

    private void LocomotiveRUCallback(string data, string error)
    {
        if(error == null)
        {
            Debug.Log(data);
            BeginLoadLocomotives();
        }
        else
        {
            Debug.Log(error);
        }
    }

    private void LocomotiveDeleteCallback(string data, string error)
    {
        if(error == null)
        {
            LocomotiveServerType _buffer = JSON.FromJSON<LocomotiveServerType>(data);
            GameObject _prefabBuffer = (from _obj in Resources.LoadAll("Locomotive/Instances") where _obj.name == _buffer.name let _pref = (GameObject)_obj select _pref).First();
            DestroyImmediate(_prefabBuffer, true);
            Debug.Log($"{_buffer.name} was destroyed successfuly");
            BeginLoadLocomotives();
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
            _carriagePrefabsBuffer = (from _obj in Resources.LoadAll("Carriages/Instances") let _carObj = (GameObject)_obj select _carObj).ToArray();
            _carriageServerDataBuffer = JSON.FromJSONArray<CarriageServerType>(data);
            _localCarriageData = AssociateModels(_carriageServerDataBuffer, _carriagePrefabsBuffer);
            //EDITOR_Untility.GET("Get Items route", GetItemsAfterLocomotiveLoadedCallback, _authToken);
            //For Test
            GetItemsAfterCarriagesLoadedCallback(TestItemsJson, null);
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    private void GetItemsAfterCarriagesLoadedCallback(string data, string error)
    {
        if (error == null)
        {
            _itemsPrefabsBuffer = (from _obj in Resources.LoadAll("Items") let _itemObj = (ItemObject)_obj select _itemObj).ToArray();
            _itemsServerDataBuffer = JSON.FromJSONArray<ItemServerType>(data);
            _localItemsData = AssociateModels(_itemsServerDataBuffer, _itemsPrefabsBuffer);
            _carriagesLoaded = true;
        }
        else
        {
            _isError = true;
            _errorMessage = error;
        }
    }

    private void CarriageCreateCallback(string data, string error)
    {
        if (error == null)
        {
            PrefabUtility.SaveAsPrefabAsset(_instanceBuffer, "Assets/Resources/Carriages/Instances/" + _newName + ".prefab");
            _carriagePrefabsBuffer = (from _obj in Resources.LoadAll("Carriages/Instances") let _carObj = (GameObject)_obj select _carObj).ToArray();
            Array.Resize(ref _carriageServerDataBuffer, _carriageServerDataBuffer.Length + 1);
            _carriageServerDataBuffer[_carriageServerDataBuffer.Length - 1] = JSON.FromJSON<CarriageServerType>(data);
            _localCarriageData = AssociateModels(_carriageServerDataBuffer, _carriagePrefabsBuffer);
            _selectedCarriageInstanceIndex = Array.IndexOf(_localCarriageData, (from _newObj in _localCarriageData where _newObj.prefab.name == _newName select _newObj).First());
            DestroyImmediate(_instanceBuffer);
            AssetDatabase.OpenAsset(_localCarriageData[_selectedCarriageInstanceIndex].prefab);
            _carriagesLoaded = true;
            _carriageCreateMode = false;
        }
        else
        {
            _carriagesLoaded = false;
            _carriageCreateMode = false;
            _isError = true;
            _errorMessage = error;
        }
    }

    private void CarriageRUCallback(string data, string error)
    {
        if (error == null)
        {
            Debug.Log(data);
            BeginLoadCarriages();
        }
        else
        {
            Debug.Log(error);
        }
    }

    private void CarriageDeleteCallback(string data, string error)
    {
        if (error == null)
        {
            CarriageServerType _buffer = JSON.FromJSON<CarriageServerType>(data);
            GameObject _prefabBuffer = (from _obj in Resources.LoadAll("Carriages/Instances") where _obj.name == _buffer.name let _pref = (GameObject)_obj select _pref).First();
            DestroyImmediate(_prefabBuffer, true);
            Debug.Log($"{_buffer.name} was destroyed successfuly");
            BeginLoadCarriages();
        }
        else
        {
            _carriagesLoaded = false;
            _carriageCreateMode = false;
            _isError = true;
            _errorMessage = error;
        }
    }

    #endregion

    #region Item

    private void GetItemsCallback(string data, string error)
    {
        if (error == null)
        {
            _selectedItemInstanceIndex = -1;
            _itemsPrefabsBuffer = (from _obj in Resources.LoadAll("Items") let _itemObj = (ItemObject)_obj select _itemObj).ToArray();
            _itemsServerDataBuffer = JSON.FromJSONArray<ItemServerType>(data);
            _localItemsData = AssociateModels(_itemsServerDataBuffer, _itemsPrefabsBuffer);
            _itemsLoaded = true;
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

[System.Serializable]
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

[System.Serializable]
public class CarriageAssemblyItem
{
    public int itemId;
    public int count;
}

[System.Serializable]
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

[System.Serializable]
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

[System.Serializable]
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

[System.Serializable]
public class RecipeServerType
{
    public int id;
    public RecipeItem[] items;
    public string appearenceVersion = "0000000";
    public int resultItemId;
    public int timeForUnit;
}

[System.Serializable]
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

[System.Serializable]
public class MapServerType
{
    public MapSector[] sectors;
}

[System.Serializable]
public class MapSector
{
    public int id;
    public int xPos;
    public int yPos;
    public string environmentName;
}

#endregion

#endregion